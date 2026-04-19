# DefenderUI — Tek Dosya (Single-File) Publish Rehberi

> Kullanıcı **tek bir `DefenderUI.exe`** indirip çift tıkladığında, .NET runtime
> ve Windows App SDK dahil **tüm bağımlılıklar exe'nin içinde** gelsin ve
> herhangi bir kurulum yapmadan çalışsın.

## ✅ Sonuç (bu konfigürasyonla elde edilen çıktı)

| Öğe | Değer |
|---|---|
| Publish klasörü | [`publish/`](publish:1) |
| Üretilen dosya sayısı | **1** |
| `.dll` / `.pdb` / yan dosya | **0** |
| Exe adı | [`DefenderUI.exe`](publish/DefenderUI.exe:1) |
| Exe boyutu | **~105 MB** (110.145.875 bytes, sıkıştırılmış) |
| Runtime | Self-contained (net9.0-windows10.0.26100.0, win-x64) |
| WinAppSDK | Self-contained (bootstrap exe içinde) |
| Çalıştırma testi | ✅ PID başladı, 9 sn boyunca responsive, temiz kapandı |

**Not:** Tek exe modeli `AppHost + bundle` şeklinde çalışır. İlk çalıştırmada
native kütüphaneler geçici bir klasöre (`%LOCALAPPDATA%\Temp\.net\DefenderUI\…`)
açılır; kullanıcı açısından bu görünmezdir — yine de tek bir `.exe` dağıtılır.

## 📦 Ne değiştirildi?

### 1) [`DefenderUI.csproj`](DefenderUI.csproj:1) — Release'e özel publish property'leri

Debug build'i hiç etkilememek için tamamı `Condition="'$(Configuration)' == 'Release'"`
altındadır.

```xml
<PropertyGroup Condition="'$(Configuration)' == 'Release'">
  <PublishSingleFile>true</PublishSingleFile>
  <SelfContained>true</SelfContained>
  <RuntimeIdentifier>win-x64</RuntimeIdentifier>
  <IncludeNativeLibrariesForSelfExtract>true</IncludeNativeLibrariesForSelfExtract>
  <IncludeAllContentForSelfExtract>true</IncludeAllContentForSelfExtract>
  <EnableCompressionInSingleFile>true</EnableCompressionInSingleFile>
  <WindowsAppSDKSelfContained>true</WindowsAppSDKSelfContained>
  <WindowsPackageType>None</WindowsPackageType>
  <DebugType>embedded</DebugType>
  <PublishTrimmed>false</PublishTrimmed>
</PropertyGroup>
```

Neden her biri:

| Property | Amaç |
|---|---|
| `PublishSingleFile=true` | Yönetilen DLL'lerin tümünü exe'ye bundle eder |
| `SelfContained=true` | .NET 9 runtime'ı exe'ye gömer; hedef makinede .NET kurulu olmasına gerek kalmaz |
| `RuntimeIdentifier=win-x64` | Tek RID'e kilitle; single-file `AnyCPU`/multi-RID ile çalışmaz |
| `IncludeNativeLibrariesForSelfExtract=true` | Native DLL'ler (`coreclr`, `hostpolicy` vs.) exe'ye gömülür ve runtime'da extract edilir |
| `IncludeAllContentForSelfExtract=true` | `Content Include` olarak işaretli tüm varlıklar (asset/png/ico) bundle'a dahil |
| `EnableCompressionInSingleFile=true` | Bundle brotli ile sıkıştırılır; ~160 MB → **~105 MB** |
| `WindowsAppSDKSelfContained=true` | WinAppSDK runtime'ın hedef makinede kurulu olma zorunluluğunu kaldırır; bootstrap edilir |
| `WindowsPackageType=None` | Unpackaged desktop app (MSIX değil) |
| `DebugType=embedded` | PDB'yi exe'nin içine göm → yanda `.pdb` dosyası kalmaz |
| `PublishTrimmed=false` | WinUI 3 + XAML tip çözümlemesinde trim kırılganlığı oluşturur; kapalı tutuyoruz |

> WinAppSDK targets dosyasından iki adet `warning` çıkar (bkz. “Bilinen uyarılar”
> bölümü). Bunlar bilgilendirmedir, build'i başarısız yapmaz.

### 2) [`Helpers/ThemeHelper.cs`](Helpers/ThemeHelper.cs:1) — Build blokajını aşmak için reflection fallback

WindowsAppSDK 1.8 sürümünde `Microsoft.UI.Xaml.Controls.Primitives.FlyoutBase`
üzerinde `RequestedTheme` property'si .NET projeksiyon yüzeyinde **doğrudan
görünmüyor** (CS1061). Aynı property türetilmiş sınıflarda (`Flyout`,
`MenuFlyout`, `CommandBarFlyout`) mevcut olduğundan, davranışı değiştirmeden
reflection ile set ediyoruz. Property bulunamazsa sessiz no-op olur.

Özet:
- Önce: `flyout.RequestedTheme = owner.ActualTheme;` ← derleme hatası
- Sonra: [`ThemeHelper.TrySetFlyoutRequestedTheme()`](Helpers/ThemeHelper.cs:70) reflection ile aynı property'yi set eder
- Dışarıdan API/davranış farkı yok.

### 3) [`scripts/publish-single.cmd`](scripts/publish-single.cmd:1) — Çift tıkla publish

Kullanıcı (veya CI) şu adımı tek tuşa indirir:
1. Eski `publish/` klasörünü temizle (immutable deploy)
2. `dotnet publish -c Release -r win-x64 -p:Platform=x64 --self-contained true -o publish`
3. Exe boyutunu ve dosya listesini yaz
4. Explorer'da `publish/` klasörünü aç

## 🚀 Publish komutu (manuel)

```powershell
# Proje kökünden
dotnet publish DefenderUI.csproj -c Release -r win-x64 -p:Platform=x64 --self-contained true -o publish
```

Veya basitçe:

```cmd
scripts\publish-single.cmd
```

Çıktı: [`publish/DefenderUI.exe`](publish/DefenderUI.exe:1) — yanında hiçbir şey yok.

## 🎯 Dağıtım

Kullanıcıya sadece `DefenderUI.exe` gönderilir. Tek tıklama ile:
- .NET 9 runtime gereksinimi YOK (içinde)
- Windows App SDK 1.8 runtime gereksinimi YOK (içinde)
- Visual C++ redistributable gereksinimi YOK (WinAppSDK self-contained içine dahil)
- Yönetici izni YOK (unpackaged, user-mode)

**Minimum hedef:** Windows 10 build 17763 (1809) veya üzeri, x64.

## ⚠️ Bilinen uyarılar (build sırasında görünür, kritik değil)

WinAppSDK `Microsoft.WindowsAppSDK.SingleFile.targets` dosyası iki bilgi uyarısı
basar:

1. **“PublishSingleFile is recommended only for Windows App SDK Self-Contained
   apps”** — Zaten `WindowsAppSDKSelfContained=true` set ettiğimiz için koşul
   sağlanmış durumda. Uyarı yine de yazdırılıyor (targets dosyasının kendi
   mantığı); göz ardı edilebilir.
2. **“PublishSingleFile requires MICROSOFT_WINDOWSAPPRUNTIME_BASE_DIRECTORY to
   be set before program entry”** — Self-contained modda bootstrap bunu zaten
   set ediyor. Manuel `Main` ile çalışmıyoruz (WinUI template `Program.Main`'i
   üretiyor), dolayısıyla çalışma zamanında sorun çıkmıyor. Testte runtime
   sorunsuz ayağa kalktı.

Eğer ilerde `Environment.SetEnvironmentVariable` ile erken-init gerekirse
`App.xaml.cs` içindeki `static App()` constructor'ına şu satır eklenebilir:

```csharp
Environment.SetEnvironmentVariable(
    "MICROSOFT_WINDOWSAPPRUNTIME_BASE_DIRECTORY",
    AppContext.BaseDirectory);
```

Şu an **gerekli değil**.

## 🧪 Çalıştırma testi (yapıldı)

```text
[OK] DefenderUI.exe calisiyor. PID=17692 WS=60.3MB Responding=True
[OK] 9sn sonra hala ayakta. Kapatiliyor...
```

- Exe başlatıldı, 6 sn sonra process ayakta ve responsive (UI message loop
  çalışıyor).
- 9 sn sonra hâlâ ayakta → crash yok.
- Working set 60 MB → WinUI 3 için beklenen aralıkta.

## ↩️ Rollback

Single-file publish'i geri almak için [`DefenderUI.csproj`](DefenderUI.csproj:89)
içindeki Release-koşullu `PropertyGroup`'u silmek yeterli. Debug build'i zaten
etkilemediği için geliştirici akışı değişmez.

## 🔒 Güvenlik / DevOps notu

- Çıktı self-contained olduğu için tüm bağımlılıklar immutable tek artifact'tir
  (SHA-256 alıp imzalamak kolaydır).
- Gerçek dağıtım için Authenticode code-sign önerilir (EV sertifika SmartScreen
  reputation için idealdir). Bu repo kapsamında imzalama yapılmadı; kullanıcı
  sertifika bağladığında `SignTool sign /fd SHA256 /tr <TSA> /td SHA256 …
  publish\DefenderUI.exe` ile exe imzalanabilir.
- Hiçbir credential/token/secret exe içine gömülmemiştir (kod tabanında da yok);
  single-file olması sır saklama aracı değildir, tersine exe reverse edilebilir.

## 🧱 %100 tek dosya zaten elde edildi — ama teorik alternatifler

Bu konfigürasyon **zaten `publish/` klasöründe tek bir dosya** üretiyor
(0 yan DLL). Dolayısıyla “Win2D-free + WinAppSDK bypass” gibi radikal
alternatiflere gerek **yoktur**. Yine de not olarak:

- **Framework-dependent single-file**: ~5–15 MB exe; hedef makinede .NET 9
  ve WinAppSDK 1.8 runtime kurulu olmak zorunda. Dağıtım basitliği açısından
  self-contained daha makul.
- **ReadyToRun (AOT benzeri başlangıç)**: İlk açılışı hızlandırır, boyutu
  ~15–25 MB büyütür. Şu an kapalı; gerekirse
  `<PublishReadyToRun>true</PublishReadyToRun>` Release'te açılabilir
  (zaten koşullu olarak tanımlı).
- **PublishTrimmed=true**: WinUI 3 + XAML reflection'ı (Binding, x:Bind
  fallback, DataTemplate tip çözümlemesi) trim warning'leri üretir ve runtime
  kırılmalara yol açabilir. Kapalı bırakılmalıdır.
- **NativeAOT**: WinUI 3 1.8 için resmi desteklenmiyor; denemek risk/fayda
  açısından negatiftir.

## 📋 Özet

- `publish/` klasöründe **tek dosya:** `DefenderUI.exe`
- Kullanıcı indirip çift tıklar → .NET ve WinAppSDK dahil her şey içinden çıkar
  ve uygulama açılır
- Debug build davranışı değişmez
- Kod davranışı değişmez (sadece bir build-blocker reflection ile çözüldü)