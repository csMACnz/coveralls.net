choco install -y psake
choco install -y pscx
choco install -y nuget.CommandLine
choco install -y GitVersion.Portable

$pscxPath = "C:\Program Files (x86)\PowerShell Community Extensions\Pscx3\Pscx";

if (-not (Test-Path $pscxPath))
{
    $pscxPath = $null;
    Write-Host "Searching for the pscx powershell module.";
    $pscxPath = (Get-ChildItem -Path "C:\Program Files\" -Filter "pscx.dll" -Recurse).FullName;
    if (!$pscxPath) { $pscxPath = (Get-ChildItem -Path "C:\Program Files (x86)\" -Filter "pscx.dll" -Recurse).FullName; }
    $pscxPath = Split-Path $pscxPath;
    Write-Host "Found it at " + $pscxPath;
}

$env:PSModulePath = $env:PSModulePath + ";" + $pscxPath;
