# appkeytest

Two apps registered in portal:

- App A: Add the application permission `Application.ReadWrite.All` and upload a self-signed cert (CreateKey.ps1 can help create this)
- Abb B: Upload the same self-signed cert

Usage for CreateKey.ps1:

```powershell
.\CreateKey.ps1 -KeyName "TestCert"

cmdlet CreateKey.ps1 at command pipeline position 1
Supply values for the following parameters:
(Type !? for Help.)
Password: ********

    Directory: C:\Source\appkeytest\keys

Mode                 LastWriteTime         Length Name
----                 -------------         ------ ----
-a---            4/1/2022  5:03 PM            762 TestCert.cer
-a---            4/1/2022  5:03 PM           2604 TestCert.pfx
```
