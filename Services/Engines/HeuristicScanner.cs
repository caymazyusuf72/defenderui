using System;
using System.IO;
using DefenderUI.Models;

namespace DefenderUI.Services.Engines;

/// <summary>
/// Statik analiz ve sezgisel kurallara dayanarak imzasız tehditleri tespit eden motor.
/// </summary>
public class HeuristicScanner
{
    // Heuristic analiz genellikle PE (Portable Executable) header'larını incelemeyi,
    // entropy (şifreleme varlığı) oranını hesaplamayı ve string'leri okumayı kapsar.
    
    public ThreatResult? ScanFile(string filePath)
    {
        try
        {
            var extension = Path.GetExtension(filePath).ToLowerInvariant();
            
            // Sadece çalıştırılabilir dosyaları ve scriptleri analiz et
            if (extension != ".exe" && extension != ".dll" && extension != ".bat" && extension != ".ps1" && extension != ".vbs")
                return null;

            // Çok büyük dosyaları (örn: 100MB+) hafızaya alıp sezgisel taramak yavaştır. Atla.
            var fileInfo = new FileInfo(filePath);
            if (fileInfo.Length > 50 * 1024 * 1024) 
                return null;

            // Basit Heuristic: İçinde şüpheli string'ler var mı?
            // (Gerçekte bu işlem I/O dostu buffer'larla yapılır, burada basit tutulmuştur)
            string contentSnippet;
            using (var reader = new StreamReader(File.OpenRead(filePath)))
            {
                // Sadece ilk 4KB'ı oku (genelde PE import table veya script başlıkları buradadır)
                char[] buffer = new char[4096];
                int bytesRead = reader.Read(buffer, 0, buffer.Length);
                contentSnippet = new string(buffer, 0, bytesRead);
            }

            // --- Yarı-Gerçekçi Heuristics Kuralları ---

            // Kural 1: Potansiyel Ransomware Davranışı (WannaCry vb. uzantı değiştirme denemeleri)
            if (contentSnippet.Contains(".wcry", StringComparison.OrdinalIgnoreCase) || 
                contentSnippet.Contains("vssadmin.exe Delete Shadows", StringComparison.OrdinalIgnoreCase))
            {
                return new ThreatResult
                {
                    FilePath = filePath,
                    ThreatName = "HEUR:Ransom.Generic.Suspicious",
                    Type = ThreatType.HeuristicSuspicious,
                    DetectionEngine = "Heuristics (Static Pattern)"
                };
            }

            // Kural 2: VBS/PS1 Zararlı Downloader Davranışı (Powershell -w hidden -enc)
            if (extension == ".bat" || extension == ".ps1" || extension == ".vbs")
            {
                if (contentSnippet.Contains("Invoke-WebRequest", StringComparison.OrdinalIgnoreCase) && 
                    contentSnippet.Contains("Hidden", StringComparison.OrdinalIgnoreCase) &&
                    contentSnippet.Contains("Bypass", StringComparison.OrdinalIgnoreCase))
                {
                    // Bizim InstallDeps.ps1'i taratırken uyarı vermesin diye istisna eklenebilir
                    // ama gerçekte bir AV bunu şüpheli görebilir.
                    if (!Path.GetFileName(filePath).Equals("InstallDeps.ps1", StringComparison.OrdinalIgnoreCase))
                    {
                        return new ThreatResult
                        {
                            FilePath = filePath,
                            ThreatName = "HEUR:Trojan.Downloader.Script",
                            Type = ThreatType.PUA,
                            DetectionEngine = "Heuristics (Script Analyzer)"
                        };
                    }
                }
            }

            // Kural 3: PE Header anormallikleri (Örn: UPX veya diğer packer'lar)
            // Normalde MZ başlığı kontrol edilir ve entropy hesaplanır.
        }
        catch
        {
            // İzin veya okuma hatası
        }

        return null;
    }
}
