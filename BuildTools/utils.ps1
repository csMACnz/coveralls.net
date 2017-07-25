
function BootstrapNuget {
param(
    [Parameter(Mandatory=$true)][string]$ExistingNuget,
    [Parameter(Mandatory=$true)][string]$packageFolder
)
    exec { & $ExistingNuget install "NuGet.CommandLine" -Version "3.4.3" -OutputDirectory "$packageFolder" -NonInteractive -Verbosity detailed }
}

function InstallNugetPackage {
param(
    [Parameter(Mandatory=$true)][string]$packageName,
    [Parameter(Mandatory=$true)][string]$Version,
    [Parameter(Mandatory=$true)][string]$packageFolder
    )
    $nuget = GetNugetPath $packageFolder
    exec { & $nuget install "$packageName" -Version "$Version" -OutputDirectory "$packageFolder" -NonInteractive -Verbosity detailed }
}

function GetGitVersionPath {
param(
    [Parameter(Mandatory=$true)][string]$packageFolder
)
    return (Resolve-Path "$packageFolder\GitVersion.CommandLine.*\tools\gitversion.exe").ToString()
}

function GetNugetPath {
param(
    [Parameter(Mandatory=$true)][string]$packageFolder
)
    return (Resolve-Path "$packageFolder\Nuget.CommandLine.*\tools\nuget.exe").ToString()
}

function GetDupFinderPath {
param(
    [Parameter(Mandatory=$true)][string]$packageFolder
)
    return (Resolve-Path "$packageFolder\JetBrains.ReSharper.CommandLineTools.*\tools\dupfinder.exe").ToString()
}

function GetInspectCodePath {
param(
    [Parameter(Mandatory=$true)][string]$packageFolder
)
    return (Resolve-Path "$packageFolder\JetBrains.ReSharper.CommandLineTools.*\tools\inspectcode.exe").ToString()
}

function GetOpenCoverPath {
    param(
    [Parameter(Mandatory=$true)][string]$packageFolder
)
    return (Resolve-Path "$packageFolder\OpenCover.*\tools\OpenCover.Console.exe").ToString()
}

function GetCoverityPath {
    param(
    [Parameter(Mandatory=$true)][string]$packageFolder
)
    return (Resolve-Path "$packageFolder\PublishCoverity.*\tools\PublishCoverity.exe").ToString()
}

function GetGitVersionPath {
param(
    [Parameter(Mandatory=$true)][string]$packageFolder
)
    return (Resolve-Path "$packageFolder\GitVersion.CommandLine.*\tools\gitversion.exe").ToString()
}

function ProjectVersion($folderPath, $programVersion) {
    Get-ChildItem -r "$folderPath/*.csproj" | ForEach-Object { 
        $fileFullPath = $_
        $fileContents = [System.IO.File]::ReadAllText($fileFullPath)

        $rawVersionPattern = '<VersionPrefix>[0-9]+(\.([0-9]+|\*)){1,3}</VersionPrefix>'

        $rawVersion = '<VersionPrefix>' + $programVersion + '</VersionPrefix>'

        $fileContents = $fileContents -replace $rawVersionPattern, $rawVersion

        [System.IO.File]::WriteAllText($fileFullPath, $fileContents)
    }
}