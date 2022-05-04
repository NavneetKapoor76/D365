param(
[string]$ZipFile,
[string]$Folder,
[string]$PackageType,
[string]$Map,
[string]$ErrorLevel="Warning"
)

[Net.ServicePointManager]::SecurityProtocol = [Net.SecurityProtocolType]::Tls12
$sourceNugetExe = "https://dist.nuget.org/win-x86-commandline/latest/nuget.exe"
$targetNugetExe = ".\nuget.exe"
Invoke-WebRequest $sourceNugetExe -OutFile $targetNugetExe
Set-Alias nuget $targetNugetExe -Scope Global -Verbose

./nuget install  Microsoft.CrmSdk.CoreTools -O ./
$coreToolsFolder = Get-ChildItem ./ | Where-Object {$_.Name -match 'Microsoft.CrmSdk.CoreTools.'}
move .\$coreToolsFolder\content\bin\coretools\*.* .\
Remove-Item .\$coreToolsFolder -Force -Recurse

if (-not $Map)
{
    .\SolutionPackager.exe /action "Pack" /zipFile $ZipFile /folder $Folder /packagetype $PackageType /errorlevel $ErrorLevel 
}
else
{
    .\SolutionPackager.exe /action "Pack" /zipFile $ZipFile /folder $Folder /packagetype $PackageType /errorlevel $ErrorLevel  /map $Map
}
