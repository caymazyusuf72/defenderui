using System;
using System.IO;
using System.Security.Cryptography;
using System.Collections.Generic;
using DefenderUI.Models;

namespace DefenderUI.Services.Engines;

/// <summary>
/// Bilinen tehditleri hash tabanlı (MD5/SHA256) yakalayan tarama motoru.
/// </summary>
public class SignatureScanner
{
    // Gerçekte bu veriler yerel bir sqlite db veya signatures.json dosyasından okunur.
    // Şimdilik RAM'de tutuyoruz. EICAR test string'inin MD5 hash'i.
    private readonly Dictionary<string, (string Name, ThreatType Type)> _signatures = new(StringComparer.OrdinalIgnoreCase)
    {
        // EICAR standart test virüsü (MD5)
        { "44D88612FEA8A8F36DE82E1278ABB02F", ("EICAR-Test-File", ThreatType.Malware) },
        // Örnek sahte PUA
        { "8A832A2BA712399201994DF1D22DF5E1", ("PUP.Optional.FakeToolbar", ThreatType.PUA) }
    };

    public ThreatResult? ScanFile(string filePath)
    {
        try
        {
            using var md5 = MD5.Create();
            using var stream = File.OpenRead(filePath);
            var hashBytes = md5.ComputeHash(stream);
            var hashStr = BitConverter.ToString(hashBytes).Replace("-", "");

            if (_signatures.TryGetValue(hashStr, out var signature))
            {
                return new ThreatResult
                {
                    FilePath = filePath,
                    ThreatName = signature.Name,
                    Type = signature.Type,
                    DetectionEngine = "Signature (MD5)"
                };
            }
        }
        catch (IOException)
        {
            // Dosya kullanımda olabilir, atla.
        }
        catch (UnauthorizedAccessException)
        {
            // İzin yok, atla.
        }

        return null;
    }
}
