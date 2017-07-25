Framework 4.5.1

properties {
    # build variables
    $configuration = "Release"	# build configuration
    $script:version = "0.5.0.0"
    $script:nugetVersion = "0.5.0.0"

    # directories
    $base_dir = . resolve-path .\
    $project_dir = "$base_dir\src\csmacnz.Coveralls"
    $app_project = "$project_dir\csmacnz.Coveralls.csproj"
    $build_output_dir = "$base_dir\src\csmacnz.Coveralls\bin\$configuration\"
    $build_packages_dir = "$base_dir\BuildPackages\"
    $test_results_dir = "$base_dir\TestResults\"
    $package_dir = "$base_dir\Package\"
    $archive_dir = "$package_dir" + "Archive"
    $nuget_pack_dir = "$package_dir" + "Pack"

    # files
    $sln_file = "$base_dir\csmacnz.Coveralls.sln"
}

Include ".\BuildTools\utils.ps1"

task default

task BootstrapNuget {
    BootstrapNuget "nuget" $build_packages_dir
}

task RestoreNuGetPackages {
    exec { dotnet restore $sln_file }
}

task InstallOpenCover -depends BootstrapNuget {
    InstallNugetPackage "OpenCover" 4.6.519 $build_packages_dir
}

task InstallCoverity -depends BootstrapNuget {
    InstallNugetPackage "PublishCoverity" 0.11.0 $build_packages_dir
}

task InstallGitVersion -depends BootstrapNuget {
    InstallNugetPackage "GitVersion.CommandLine" 3.6.5 $build_packages_dir
}

task InstallReSharperCLI -depends BootstrapNuget {
    InstallNugetPackage "JetBrains.ReSharper.CommandLineTools" 2017.1.20170613.162720 $build_packages_dir
}

task GitVersion -depends InstallGitVersion {
    $gitVersion = GetGitVersionPath $build_packages_dir
    exec { & $gitVersion /output buildserver /updateassemblyinfo }
    $json = (& $gitVersion) | ConvertFrom-Json
    $script:version = $json.MajorMinorPatch
    $script:nugetVersion = $json.NuGetVersionV2
    ProjectVersion "$project_dir" $script:nugetVersion
}

task AppVeyorEnvironmentSettings {

    if (Test-Path Env:\GitVersion_ClassicVersion) {
        $script:version = $env:GitVersion_ClassicVersion
        Write-Output "version set to $script:version"
    }
    elseif (Test-Path Env:\APPVEYOR_BUILD_VERSION) {
        $script:version = $env:APPVEYOR_BUILD_VERSION
        Write-Output "version set to $script:version"
    }
    if (Test-Path Env:\GitVersion_NuGetVersionV2) {
        $script:nugetVersion = $env:GitVersion_NuGetVersionV2
        Write-Output "nuget version set to $script:nugetVersion"
    }
    elseif (Test-Path Env:\APPVEYOR_BUILD_VERSION) {
        $script:nugetVersion = $env:APPVEYOR_BUILD_VERSION
        Write-Output "nuget version set to $script:nugetVersion"
    }
}

task clean {
    if (Test-Path $package_dir) {
        Remove-Item $package_dir -r
    }
    if (Test-Path $test_results_dir) {
        Remove-Item $test_results_dir -r
    }
    if (Test-Path $build_packages_dir) {
        Remove-Item $build_packages_dir -r
    }
    dotnet clean $sln_file
}

task build {
    exec { dotnet build -c $configuration $sln_file }
}

task setup-coverity-local {
    $env:APPVEYOR_BUILD_FOLDER = "."
    $env:APPVEYOR_BUILD_VERSION = $script:version
    $env:APPVEYOR_REPO_NAME = "csMACnz/coveralls.net"
    "You should have set the COVERITY_TOKEN and COVERITY_EMAIL environment variable already"
    $env:APPVEYOR_SCHEDULED_BUILD = "True"
}

task test-coverity -depends setup-coverity-local, coverity

task coverity -depends InstallCoverity -precondition { return $env:APPVEYOR_SCHEDULED_BUILD -eq "True" } {

    $coverityFileName = "coveralls.coverity.$script:nugetVersion.zip"
    $PublishCoverity = GetCoverityPath $build_packages_dir

    & cov-configure --comptype csc --compiler "C:\Program Files\dotnet\dotnet.exe"
    & cov-build --dir cov-int dotnet msbuild "/t:Clean;Build" "/p:Configuration=$configuration" $sln_file

    if (-not (Test-Path $test_results_dir)) {
        mkdir $test_results_dir
    }

    & $PublishCoverity compress -o $coverityFileName

    & $PublishCoverity publish -t $env:COVERITY_TOKEN -e $env:COVERITY_EMAIL -z $coverityFileName -d "AppVeyor scheduled build ($env:APPVEYOR_BUILD_VERSION)." --codeVersion $script:nugetVersion
}

task unit-test {
    dotnet test .\src\csmacnz.Coveralls.Tests\csmacnz.Coveralls.Tests.csproj
}

task integration {
    $env:MONO_INTEGRATION_MODE = ""
    iex "& $script:xunit "".\src\csmacnz.Coveralls.Tests.Integration\bin\$configuration\csmacnz.Coveralls.Tests.Integration.dll"" -noshadow $script:testOptions"
}

task mono-integration {
    $env:MONO_INTEGRATION_MODE = "True"
    $env:MONO_INTEGRATION_MONOPATH = "C:\Program Files (x86)\Mono\bin"
    iex "& $script:xunit "".\src\csmacnz.Coveralls.Tests.Integration\bin\$configuration\csmacnz.Coveralls.Tests.Integration.dll"" -noshadow $script:testOptions"
}

task coverage -depends build, coverage-only

task coverage-only -depends InstallOpenCover {
    if(-not (Test-Path $test_results_dir)) {
        mkdir $test_results_dir
    }

    $opencover = GetOpenCoverPath $build_packages_dir
    exec { 
        Set-Location "$base_dir\src\csmacnz.Coveralls.Tests"
        & $opencover -oldstyle -register:user -target:dotnet.exe "-targetargs:xunit -configuration Debug" -filter:"+[csmacnz.Coveralls*]* -[csmacnz.Coveralls.Tests]AutoGeneratedProgram" -output:"$test_results_dir\Coverage.xml"
        Set-Location $base_dir
    }
}

task test-coveralls -depends archive, coverage {
    exec { & ".\Package\Archive\windows\csmacnz.Coveralls.exe" --opencover -i "$test_results_dir\Coverage.xml" --useRelativePaths --dryrun -o "$test_results_dir\coverallsTestOutput.json" --repoToken "NOTAREALTOKEN" }
}

task coveralls-only -depends InstallCoveralls -precondition { return -not $env:APPVEYOR_PULL_REQUEST_NUMBER } {
    exec { & ".\Package\Archive\windows\csmacnz.Coveralls.exe" --opencover -i "$test_results_dir\Coverage.xml" --treatUploadErrorsAsWarnings }
}

task dupfinder -depends InstallReSharperCLI {
    $dupfinder = GetDupFinderPath $build_packages_dir
    exec { cmd /c $dupfinder /o="$test_results_dir\duplicateReport.xml" /show-text $base_dir\src\**\*.cs 2`> nul }
    [xml]$stats = Get-Content $test_results_dir\duplicateReport.xml
    $anyDuplicates = $FALSE;

    foreach ($duplicate in $stats.DuplicatesReport.Duplicates.Duplicate) {
        Write-Host "Duplicate code found with a cost of $($duplicate.Cost), in $($duplicateCost.Fragment.Count) fragments"

        foreach ($fragment in $duplicate.Fragment) {
            Write-Host "File: $($fragment.FileName) Line: $($fragment.LineRange.Start) - $($fragment.LineRange.End)"
            Write-Host "Text: $($fragment.Text)"
        }

        $anyDuplicates = $TRUE;

        if (Get-Command "Add-AppveyorTest" -errorAction SilentlyContinue) {
            Add-AppveyorMessage "Duplicate Found in the file $($fragment.FileName) with a cost of $($duplicate.Cost), across $($duplicate.Fragment.Count) Fragments" -Category Warning -Details "See duplicateReport.xml for details of duplicates"
            if ([convert]::ToInt32($duplicate.Cost, 10) -gt 100) {
                Add-AppveyorTest "Duplicate Found with a cost of $($duplicate.Cost), across $($duplicate.Fragment.Count) Fragments" -Outcome Failed -ErrorMessage "See duplicateReport.xml for details of duplicates" -FileName "$($fragment.FileName)"
            }
        }
    }

    $xslt = New-Object System.Xml.Xsl.XslCompiledTransform
    $xslt.Load("BuildTools\dupfinder.xslt")
    $xslt.Transform("$test_results_dir\duplicateReport.xml", "$test_results_dir\duplicateReport.html")

    if (Get-Command "Push-AppveyorArtifact" -errorAction SilentlyContinue) {
        Push-AppveyorArtifact $test_results_dir\duplicateReport.xml
        Push-AppveyorArtifact $test_results_dir\duplicateReport.html
    }
}

task inspect -depends InstallReSharperCLI {
    $inspectcode = GetInspectCodePath $build_packages_dir
    exec { cmd /c $inspectcode /o="$test_results_dir\resharperReport.xml" $sln_file 2`> nul }
    [xml]$stats = Get-Content $test_results_dir\resharperReport.xml
    $anyErrors = $FALSE;
    $errors = $stats.SelectNodes("/Report/IssueTypes/IssueType")

    foreach ($errorType in $errors) {
        $errorTypeName = $(Get-Culture).TextInfo.ToTitleCase($errorType.Severity.ToLower())
        Write-Host "Found InspectCode $errorTypeName(s): $($errorType.Description)"

        $issues = $stats.SelectNodes("/Report/Issues/Project/Issue[@TypeId='$($errorType.Id)']")
        foreach ($issue in $issues) {
            Write-Host "File: $($issue.File) Line: $($issue.Line) Message: $($issue.Message)"

            if (($errorType.Severity -eq "ERROR") -and (Get-Command "Add-AppveyorTest" -errorAction SilentlyContinue)) {
                Add-AppveyorTest "Resharper Error: $($errorType.Description) Line: $($issue.Line)" -Outcome Failed -FileName "$($issue.File)" -ErrorMessage "$($issue.Message)"
            }
            elseif (Get-Command "Add-AppveyorMessage" -errorAction SilentlyContinue) {
                if ($errorType.Severity -eq "WARNING") {
                    Add-AppveyorMessage "Resharper Warning: $($errorType.Description) File: $($issue.File) Line: $($issue.Line)" -Category Warning -Details "$($issue.Message)"
                }
                else {
                    Add-AppveyorMessage "Resharper $($errorTypeName): $($errorType.Description) File: $($issue.File) Line: $($issue.Line)" -Category Information -Details "$($issue.Message)"
                }
            }
        }
        if ($errorType.Severity -eq "ERROR") {
            $anyErrors = $TRUE
        }
    }

    $xslt = New-Object System.Xml.Xsl.XslCompiledTransform
    $xslt.Load("BuildTools\resharperReport.xslt")
    $xslt.Transform("$test_results_dir\resharperReport.xml", "$test_results_dir\resharperReport.html")

    if (Get-Command "Push-AppveyorArtifact" -errorAction SilentlyContinue) {
        Push-AppveyorArtifact $test_results_dir\resharperReport.xml
        Push-AppveyorArtifact $test_results_dir\resharperReport.html
    }

    if ($anyErrors -eq $TRUE) {
        Write-Host "There are Resharper errors in the solution"
        throw "Resharper errors in the solution"
    }
}

task archive -depends build, archive-only

task archive-only {
    $archive_filename = "$package_dir\coveralls.net.$script:nugetVersion.zip"
    if(Test-Path $archive_dir) {
        Remove-Item $archive_dir -r
    }
    if(Test-Path $archive_filename) {
        Remove-Item $archive_filename
    }
    mkdir $archive_dir
    dotnet publish $app_project -f netcoreapp1.1 -c $configuration -o "$archive_dir\windows" -r win7-x64
    dotnet publish $app_project -f netcoreapp1.1 -c $configuration -o "$archive_dir\ubuntu" -r ubuntu.14.04-x64
    
    Add-Type -assembly "system.io.compression.filesystem"

    [io.compression.zipfile]::CreateFromDirectory("$archive_dir", $archive_filename)
}

task pack -depends build, pack-only

task pack-only {
    dotnet pack -c $configuration -o $package_dir $app_project
}

task postbuild -depends coverage-only, integration, mono-integration, coveralls-only, inspect, dupfinder, archive-only, pack-only

task appveyor-install -depends GitVersion, RestoreNuGetPackages

task appveyor-build -depends build

task appveyor-test -depends AppVeyorEnvironmentSettings, postbuild
