using System;
using System.Diagnostics;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using DefenderUI.Models;

namespace DefenderUI.Services.Engines;

/// <summary>
/// Rust tabanlı HydraDragon motorunu dışarıdan çağıran tarama servisi.
/// </summary>
public class HydraDragonScanner
{
    private readonly string _enginePath;

    public HydraDragonScanner()
    {
        // Taşınabilir (Portable) çalışma için Temp klasörünü kullan
        var tempFolder = Path.Combine(Path.GetTempPath(), "DefenderUI", "Engine");
        Directory.CreateDirectory(tempFolder);
        _enginePath = Path.Combine(tempFolder, "hydradragonav.exe");
    }

    public async Task<ThreatResult?> ScanFileAsync(string filePath)
    {
        try
        {
            var assembly = System.Reflection.Assembly.GetExecutingAssembly();
            using var stream = assembly.GetManifestResourceStream("HydraDragonEngine");
            if (stream != null)
            {
                bool needsExtraction = true;
                if (File.Exists(_enginePath))
                {
                    var existingFile = new FileInfo(_enginePath);
                    if (existingFile.Length == stream.Length)
                    {
                        needsExtraction = false;
                    }
                }

                if (needsExtraction)
                {
                    using var fileStream = new FileStream(_enginePath, FileMode.Create, FileAccess.Write);
                    await stream.CopyToAsync(fileStream);
                }
            }
            else
            {
                return null; // Gömülü kaynak bulunamadı
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Engine extraction failed: {ex.Message}");
            return null;
        }

        try
        {
            var processInfo = new ProcessStartInfo
            {
                FileName = _enginePath,
                Arguments = $"scan \"{filePath}\" --files --json", // Rust CLI argümanları
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            using var process = new Process { StartInfo = processInfo };
            process.Start();

            var output = await process.StandardOutput.ReadToEndAsync();
            await process.WaitForExitAsync();

            if (string.IsNullOrWhiteSpace(output)) return null;

            // Satır satır okuma (birden fazla json nesnesi yazdırabilir)
            var lines = output.Split('\n', StringSplitOptions.RemoveEmptyEntries);
            foreach (var line in lines)
            {
                if (line.Trim().StartsWith("{"))
                {
                    try
                    {
                        var json = JsonDocument.Parse(line);
                        var verdict = json.RootElement.GetProperty("verdict").GetString();
                        
                        if (verdict != "Clean" && verdict != "Trusted")
                        {
                            var threatName = json.RootElement.TryGetProperty("threat_name", out var tProp) 
                                ? tProp.GetString() : "Unknown Threat";

                            return new ThreatResult
                            {
                                FilePath = filePath,
                                ThreatName = threatName ?? "Unknown.Malware",
                                Type = verdict == "Pua" ? ThreatType.PUA : ThreatType.Malware,
                                DetectionEngine = "HydraDragon (Rust)"
                            };
                        }
                    }
                    catch { /* parse error, ignore */ }
                }
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"HydraDragon Scanner Error: {ex.Message}");
        }

        return null;
    }
}
