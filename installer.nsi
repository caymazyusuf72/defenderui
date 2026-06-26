!include "MUI2.nsh"

Name "DefenderUI"
OutFile "DefenderUI_Setup.exe"
InstallDir "$PROGRAMFILES64\DefenderUI"
InstallDirRegKey HKLM "Software\DefenderUI" "Install_Dir"

; Kurulum için yönetici hakları iste
RequestExecutionLevel admin

; Dosya boyutunu en aza indirmek için LZMA Solid sıkıştırma kullan
SetCompressor /SOLID lzma

; Kurulum sayfaları
!define MUI_ABORTWARNING
!insertmacro MUI_PAGE_WELCOME
!insertmacro MUI_PAGE_DIRECTORY
!insertmacro MUI_PAGE_INSTFILES
!insertmacro MUI_PAGE_FINISH

; Kaldırma (Uninstall) sayfaları
!insertmacro MUI_UNPAGE_WELCOME
!insertmacro MUI_UNPAGE_CONFIRM
!insertmacro MUI_UNPAGE_INSTFILES
!insertmacro MUI_UNPAGE_FINISH

; Dil destekleri (Türkçe varsayılan)
!insertmacro MUI_LANGUAGE "Turkish"
!insertmacro MUI_LANGUAGE "English"

Section "DefenderUI (required)"
  SectionIn RO
  
  ; Hedef dizini ayarla
  SetOutPath "$INSTDIR"
  
  ; Publish klasöründeki tüm dosyaları kopyala
  File /r "publish\*.*"
  
  ; Kurulum yolunu kayıt defterine yaz
  WriteRegStr HKLM "Software\DefenderUI" "Install_Dir" "$INSTDIR"
  
  ; Denetim Masası "Program Ekle/Kaldır" kayıtları
  WriteRegStr HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\DefenderUI" "DisplayName" "DefenderUI"
  WriteRegStr HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\DefenderUI" "UninstallString" '"$INSTDIR\uninstall.exe"'
  WriteRegStr HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\DefenderUI" "DisplayIcon" '"$INSTDIR\DefenderUI.exe",0'
  WriteRegStr HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\DefenderUI" "Publisher" "Caymaz"
  WriteRegDWORD HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\DefenderUI" "NoModify" 1
  WriteRegDWORD HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\DefenderUI" "NoRepair" 1
  
  ; Kaldırma programını (uninstaller) oluştur
  WriteUninstaller "uninstall.exe"
  
  ; Masaüstü kısayolu oluştur
  CreateShortCut "$DESKTOP\DefenderUI.lnk" "$INSTDIR\DefenderUI.exe" "" "$INSTDIR\Assets\AppIcon.ico" 0
  
  ; Başlat menüsü kısayolu oluştur
  CreateDirectory "$SMPROGRAMS\DefenderUI"
  CreateShortCut "$SMPROGRAMS\DefenderUI\DefenderUI.lnk" "$INSTDIR\DefenderUI.exe" "" "$INSTDIR\Assets\AppIcon.ico" 0
  CreateShortCut "$SMPROGRAMS\DefenderUI\Kaldır.lnk" "$INSTDIR\uninstall.exe" "" "" 0
  
SectionEnd

Section "Uninstall"
  
  ; Kayıt defteri girdilerini sil
  DeleteRegKey HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\DefenderUI"
  DeleteRegKey HKLM "Software\DefenderUI"
  
  ; Yüklü dosyaları sil
  RMDir /r "$INSTDIR"
  
  ; Kısayolları sil
  Delete "$DESKTOP\DefenderUI.lnk"
  RMDir /r "$SMPROGRAMS\DefenderUI"
  
SectionEnd
