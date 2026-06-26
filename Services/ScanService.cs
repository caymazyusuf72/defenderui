using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using DefenderUI.Models;
using DefenderUI.Services.Engines;

namespace DefenderUI.Services;

/// <summary>
/// Gelişmiş Tarama Motoru. Dosyaları gerçek zamanlı tarar,
/// İmza (Signature) ve Sezgisel (Heuristic) analiz yapar, 
/// tehditleri ThreatManager ile temizler.
/// </summary>
public sealed class ScanService : IScanService
{
    private readonly object _sync = new();
    
    // Core Engines
    private readonly ThreatManager _threatManager = new();
    private readonly SignatureScanner _sigScanner = new();
    private readonly HeuristicScanner _heurScanner = new();
    private readonly HydraDragonScanner _dragonScanner = new(); // Yeni Rust Motoru

    private CancellationTokenSource? _cts;
    private ManualResetEventSlim? _pauseGate;
    private Task? _scanTask;

    public bool IsScanning { get; private set; }
    public ScanMode? CurrentMode { get; private set; }

    public event EventHandler<ScanProgressInfo>? ProgressChanged;
    public event EventHandler<ScanCompletionInfo>? ScanCompleted;
    public event EventHandler? ScanCancelled;

    public Task StartScanAsync(ScanMode mode, IEnumerable<string>? customPaths = null)
    {
        lock (_sync)
        {
            if (IsScanning) return Task.CompletedTask;
            IsScanning = true;
            CurrentMode = mode;
            _cts = new CancellationTokenSource();
            _pauseGate = new ManualResetEventSlim(true);
        }

        var token = _cts!.Token;
        var gate = _pauseGate!;

        _scanTask = Task.Run(() => RunRealScanLoop(mode, customPaths, gate, token), token);
        return _scanTask;
    }

    public void CancelScan()
    {
        lock (_sync)
        {
            if (!IsScanning) return;
            _pauseGate?.Set();
            _cts?.Cancel();
        }
    }

    public void PauseScan()
    {
        lock (_sync) { if (IsScanning) _pauseGate?.Reset(); }
    }

    public void ResumeScan()
    {
        lock (_sync) { if (IsScanning) _pauseGate?.Set(); }
    }

    private async Task RunRealScanLoop(ScanMode mode, IEnumerable<string>? customPaths, ManualResetEventSlim gate, CancellationToken token)
    {
        var stopwatch = Stopwatch.StartNew();
        int filesScanned = 0;
        int threatsFound = 0;
        
        var targetPaths = GetTargetDirectories(mode, customPaths);
        var totalEstimatedFiles = EstimateFileCount(mode);

        try
        {
            foreach (var rootPath in targetPaths)
            {
                if (token.IsCancellationRequested) break;
                if (!Directory.Exists(rootPath) && !File.Exists(rootPath)) continue;

                // Dosyaları teker teker gez
                var files = GetFilesSafe(rootPath);
                foreach (var file in files)
                {
                    if (token.IsCancellationRequested) break;

                    // Pause kontrolü
                    if (!gate.IsSet)
                    {
                        try { gate.Wait(token); } catch (OperationCanceledException) { break; }
                    }

                    filesScanned++;
                    
                    // --- TARAMA AŞAMASI (SCAN ENGINE) ---
                    // 1. Signature Scan (Temel C# Hash Kontrolü)
                    var threat = _sigScanner.ScanFile(file);
                    
                    // 2. Heuristic Scan (Eğer imzada temiz çıktıysa)
                    if (threat == null)
                    {
                        threat = _heurScanner.ScanFile(file);
                    }

                    // 3. HYDRADRAGON RUST ENGINE SCAN (Ekstra Derin Analiz)
                    if (threat == null)
                    {
                        threat = await _dragonScanner.ScanFileAsync(file);
                    }

                    // 4. Dezenfeksiyon / Karantina
                    if (threat != null)
                    {
                        threatsFound++;
                        bool handled = _threatManager.Disinfect(threat);
                        Debug.WriteLine($"Threat Found: {threat.ThreatName} in {file}. Action: {threat.ActionTaken}");
                    }

                    // Her 50 dosyada bir arayüzü güncelle (performans için)
                    if (filesScanned % 50 == 0)
                    {
                        var percent = Math.Min(99.0, (double)filesScanned / Math.Max(1, totalEstimatedFiles) * 100.0);
                        var elapsed = stopwatch.Elapsed;
                        
                        ProgressChanged?.Invoke(this, new ScanProgressInfo(
                            PercentComplete: percent,
                            FilesScanned: filesScanned,
                            ThreatsFound: threatsFound,
                            CurrentPath: file,
                            Elapsed: elapsed,
                            EstimatedRemaining: TimeSpan.FromMinutes(2) // Tahmini
                        ));
                    }
                }
            }

            stopwatch.Stop();

            if (token.IsCancellationRequested)
            {
                ScanCancelled?.Invoke(this, EventArgs.Empty);
                ResetState();
                return;
            }

            var completion = new ScanCompletionInfo(
                Mode: mode,
                FilesScanned: filesScanned,
                ThreatsFound: threatsFound,
                Duration: stopwatch.Elapsed,
                CompletedAt: DateTime.Now);

            // Son kez %100 fırlat
            ProgressChanged?.Invoke(this, new ScanProgressInfo(100.0, filesScanned, threatsFound, "Bitti", stopwatch.Elapsed, TimeSpan.Zero));
            
            ScanCompleted?.Invoke(this, completion);
            ResetState();
        }
        catch (OperationCanceledException)
        {
            ScanCancelled?.Invoke(this, EventArgs.Empty);
            ResetState();
        }
    }

    private IEnumerable<string> GetTargetDirectories(ScanMode mode, IEnumerable<string>? customPaths)
    {
        if (mode == ScanMode.Custom && customPaths != null) return customPaths;
        
        return mode switch
        {
            ScanMode.Quick => new[] { Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), Environment.GetFolderPath(Environment.SpecialFolder.Windows) },
            ScanMode.Full => new[] { "C:\\" }, // Tüm C diski
            ScanMode.Removable => new[] { "D:\\", "E:\\", "F:\\" }, // Basit tahmin
            _ => new[] { Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) }
        };
    }

    private int EstimateFileCount(ScanMode mode) => mode switch
    {
        ScanMode.Quick => 50000,
        ScanMode.Full => 300000,
        ScanMode.Custom => 10000,
        _ => 50000
    };

    // İzin verilmeyen klasörlerde çökmeyi önleyen güvenli dosya gezinme algoritması
    private IEnumerable<string> GetFilesSafe(string path)
    {
        var files = new List<string>();
        if (File.Exists(path))
        {
            files.Add(path);
            return files;
        }

        try
        {
            var enumOptions = new EnumerationOptions 
            { 
                IgnoreInaccessible = true, 
                RecurseSubdirectories = true,
                ReturnSpecialDirectories = false
            };
            return Directory.EnumerateFiles(path, "*.*", enumOptions);
        }
        catch 
        {
            return Array.Empty<string>();
        }
    }

    private void ResetState()
    {
        lock (_sync)
        {
            IsScanning = false;
            CurrentMode = null;
            try { _cts?.Dispose(); } catch (ObjectDisposedException) { }
            _cts = null;
            try { _pauseGate?.Dispose(); } catch (ObjectDisposedException) { }
            _pauseGate = null;
        }
    }
}