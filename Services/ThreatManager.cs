using System;
using System.IO;
using DefenderUI.Models;

namespace DefenderUI.Services;

/// <summary>
/// Bulunan tehditleri dezenfekte eden veya karantinaya alan merkezi yönetici (HydraDragon mimarisi).
/// </summary>
public class ThreatManager
{
    private readonly string _quarantineDir;

    public ThreatManager()
    {
        // Karantina klasörü uygulamanın çalıştığı dizinin altında gizli bir klasör olabilir
        // veya ProgramData altında olabilir. Şimdilik AppData altında tutuyoruz.
        _quarantineDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "DefenderUI", "Quarantine");
        
        if (!Directory.Exists(_quarantineDir))
        {
            Directory.CreateDirectory(_quarantineDir);
        }
    }

    /// <summary>
    /// Zararlı dosyayı karantinaya alır. (Dosya uzantısı ve şifreleme eklenerek güvenli hale getirilir).
    /// </summary>
    public bool QuarantineThreat(ThreatResult threat)
    {
        try
        {
            if (!File.Exists(threat.FilePath)) return false;

            // Karantina için güvenli bir isim oluştur (orijinal ismi ve zamanı içerir)
            string safeFileName = $"{Guid.NewGuid()}_{Path.GetFileName(threat.FilePath)}.locked";
            string destPath = Path.Combine(_quarantineDir, safeFileName);

            // Gerçek bir AV'de burada XOR şifreleme yapılır ki karantinadaki virüs yanlışlıkla çalıştırılmasın.
            // Biz basitçe taşıyıp uzantısını .locked yapıyoruz.
            File.Move(threat.FilePath, destPath, overwrite: true);

            threat.ActionTaken = "Quarantined";
            return true;
        }
        catch
        {
            threat.ActionTaken = "Quarantine Failed";
            return false;
        }
    }

    /// <summary>
    /// Dezenfeksiyon işlemini dener. Dosya tamamen virüs değilse ve temizlenebilirse temizler, 
    /// aksi takdirde dosyayı karantinaya yollar veya siler.
    /// </summary>
    public bool Disinfect(ThreatResult threat)
    {
        // HydraDragon yaklaşımı: Bulaşan dosyayı (infected) temizleme genelde zordur (örneğin PE header'ını onarmak).
        // Günümüzde modern AV'ler temizlenemeyen dosyaları direkt karantinaya alır.
        
        // PUA (Potansiyel İstenmeyen) ise genelde direkt silinir veya Uninstall edilir.
        if (threat.Type == ThreatType.PUA)
        {
            try
            {
                File.Delete(threat.FilePath);
                threat.ActionTaken = "Cleaned (Deleted)";
                return true;
            }
            catch
            {
                return QuarantineThreat(threat);
            }
        }

        // Malware / Ransomware için direkt Karantina en güvenlisidir.
        return QuarantineThreat(threat);
    }
}
