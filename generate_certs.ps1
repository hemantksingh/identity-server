param (
    [string] $certDir="$env:USERPROFILE\.aspnet\https",
    [string] $certPassword='password'
)
$ErrorActionPreference = "Stop"


function Create-DirIfDoesNotExist( [Parameter(Mandatory = $true)][string] $dir) {
    if (!(test-path $dir)) {
        Write-Host "Creating dir '$dir'"
        New-Item -ItemType Directory -Force -Path $dir
    }
    else {
        Write-Host "Directory '$dir' already exists, removing its contents"
        Remove-Item -Path "$dir\*" -Recurse
    }
}

$cert = New-SelfSignedCertificate -DnsName @("localhost", "identity-server") `
    -FriendlyName "Development Service Identity Certificate" `
    -CertStoreLocation "cert:\LocalMachine\My"

Create-DirIfDoesNotExist $certDir
$certKeyPath = "$certDir\service-identity.pfx"
$password = ConvertTo-SecureString $certPassword -AsPlainText -Force
$cert | Export-PfxCertificate -FilePath $certKeyPath -Password $password
$rootCert = $(Import-PfxCertificate -FilePath $certKeyPath -CertStoreLocation 'Cert:\LocalMachine\Root' -Password $password)

Write-Host "Exported certificate '$($rootCert.FriendlyName)' to trusted root certificates"