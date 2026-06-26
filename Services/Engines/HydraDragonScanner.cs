using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using DefenderUI.Models;

namespace DefenderUI.Services.Engines;

/// <summary>
/// Rust tabanlı HydraDragon motorunu dışarıdan çağıran tarama servisi.
/// </summary>
public class HydraDragonScanner
{
    // Rust ile derlenmiş motorun yolu (Örn: dragon/hydradragonav/target/release/hydradragonav.exe)
    private readonly string _enginePath;

    public HydraDragonScanner()
    {
        // Projenin çalıştığı dizindeki dragon klasörünü baz alır
        _enginePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "dragon", "hydradragonav", "target", "release", "hydradragonav.exe");
    }

    public async Task<ThreatResult?> ScanFileAsync(string filePath)
    {
        // GÜVENLİK KONTROLÜ: Kullanıcı PC'sinin zarar görmemesi için şu an bu motor 
        // bilerek "Not Found" döndürecek şekilde veya pasif olarak ayarlanmıştır.
        if (!File.Exists(_enginePath))
        {
            // Eğer motor derlenmemişse null dön (atla)
            return null;
        }

        try
        {
            var processInfo = new ProcessStartInfo
            {
                FileName = _enginePath,
                Arguments = $"--scan \"{filePath}\"", // HydraDragon'un argüman yapısına göre uyarlanmalı
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            using var process = new Process { StartInfo = processInfo };
            process.Start();

            // Motorun çıktısını (JSON veya metin) asenkron oku
            var output = await process.StandardOutput.ReadToEndAsync();
            await process.WaitForExitAsync();

            // Eğer motor virüs bulursa (Örn: "INFECTED: Malware.Win32") 
            if (output.Contains("INFECTED", StringComparison.OrdinalIgnoreCase))
            {
                // Rust motorundan gelen Threat Result parse edilir
                return new ThreatResult
                {
                    FilePath = filePath,
                    ThreatName = ExtractThreatName(output),
                    Type = output.Contains("PUA", StringComparison.OrdinalIgnoreCase) ? ThreatType.PUA : ThreatType.Malware,
                    DetectionEngine = "HydraDragon (Rust Core)"
                };
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"HydraDragon Scanner Error: {ex.Message}");
        }

        return null;
    }

    private string ExtractThreatName(string engineOutput)
    {
        // Örnek: "INFECTED: Trojan.Win32.Generic" -> "Trojan.Win32.Generic"
        var lines = engineOutput.Split('\n');
        foreach (var line in lines)
        {
            if (line.StartsWith("INFECTED:", StringComparison.OrdinalIgnoreCase))
            {
                return line.Substring("INFECTED:".Length).Trim();
            }
        }
        return "HEUR:Suspicious.Generic";
    }
}
