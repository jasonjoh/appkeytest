param(
  [Parameter(Mandatory=$true,
  HelpMessage="The subject name of the cert")]
  [String]
  $KeyName,

  [Parameter(Mandatory=$true,
  HelpMessage="The password for the cert")]
  [SecureString]
  $Password
)

$certSubject = "CN=" + $KeyName
$publicKeyFile = "./"+ $KeyName + ".cer"
$privateKeyFile = "./"+ $KeyName + ".pfx"

$cert = New-SelfSignedCertificate -Subject $certSubject -CertStoreLocation "Cert:\CurrentUser\My" `
 -KeyExportPolicy Exportable -KeySpec Signature -KeyLength 2048 -KeyAlgorithm RSA -HashAlgorithm SHA256 -ErrorAction Stop

Export-Certificate -Cert $cert -FilePath $publicKeyFile -ErrorAction Stop

Export-PfxCertificate -Cert $cert -FilePath $privateKeyFile -Password $Password
