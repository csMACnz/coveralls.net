properties {
    # build variables
    $framework = "4.5.1"		# .net framework version
    $configuration = "Release"	# build configuration
    $script:version = "0.5.0.0"
    $script:nugetVersion = "0.5.0.0"

    # directories
    $base_dir = . resolve-path .\
    $build_output_dir = "$base_dir\src\csmacnz.Coveralls\bin\$configuration\"
    $test_results_dir = "$base_dir\TestResults\"
    $package_dir = "$base_dir\Package\"
    $archive_dir = "$package_dir" + "Archive"
    $nuget_pack_dir = "$package_dir" + "Pack"

    # files
    $sln_file = "$base_dir\src\csmacnz.Coveralls.sln"
    $nuspec_filename = "coveralls.net.nuspec"
    $testOptions = ""
    $script:xunit = "$base_dir\src\packages\xunit.runner.console.2.0.0/tools/xunit.console.exe"

}

task default

task SetChocolateyPath {
	$script:chocolateyDir = $null
	if ($env:ChocolateyInstall -ne $null) {
		$script:chocolateyDir = $env:ChocolateyInstall;
	} elseif (Test-Path (Join-Path $env:SYSTEMDRIVE Chocolatey)) {
		$script:chocolateyDir = Join-Path $env:SYSTEMDRIVE Chocolatey;
	} elseif (Test-Path (Join-Path ([Environment]::GetFolderPath("CommonApplicationData")) Chocolatey)) {
		$script:chocolateyDir = Join-Path ([Environment]::GetFolderPath("CommonApplicationData")) Chocolatey;
	}

    Write-Output "Chocolatey installed at $script:chocolateyDir";
}

task RestoreNuGetPackages -depends SetChocolateyPath {
    $chocolateyBinDir = Join-Path $script:chocolateyDir -ChildPath "bin";
	$NuGetExe = Join-Path $chocolateyBinDir -ChildPath "NuGet.exe";

    exec { & $NuGetExe restore $sln_file }
}

task GitVersion -depends SetChocolateyPath {
	$chocolateyBinDir = Join-Path $script:chocolateyDir -ChildPath "bin";
	$gitVersionExe = Join-Path $chocolateyBinDir -ChildPath "GitVersion.exe";

    & $gitVersionExe /output buildserver /updateassemblyinfo true /assemblyVersionFormat Major
}

task LocalTestSettings {
    $script:xunit = "$base_dir/src/packages/xunit.runner.console.2.0.0/tools/xunit.console.exe"
    $script:testOptions = ""
}

task AppVeyorEnvironmentSettings {

    if(Test-Path Env:\GitVersion_ClassicVersion) {
        $script:version = $env:GitVersion_ClassicVersion
        echo "version set to $script:version"
    }
    elseif (Test-Path Env:\APPVEYOR_BUILD_VERSION) {
        $script:version = $env:APPVEYOR_BUILD_VERSION
        echo "version set to $script:version"
    }
    if(Test-Path Env:\GitVersion_NuGetVersionV2) {
        $script:nugetVersion = $env:GitVersion_NuGetVersionV2
        echo "nuget version set to $script:nugetVersion"
    }
    elseif (Test-Path Env:\APPVEYOR_BUILD_VERSION) {
        $script:nugetVersion = $env:APPVEYOR_BUILD_VERSION
        echo "nuget version set to $script:nugetVersion"
    }

    $script:xunit = "$($Env:xunit20)\xunit.console.exe"
    $script:testOptions = "-appveyor"
}

task clean {
    if (Test-Path $package_dir) {
      Remove-Item $package_dir -r
    }
    if (Test-Path $test_results_dir) {
      Remove-Item $test_results_dir -r
    }
    $archive_filename = "coveralls.net.*.zip"
    if (Test-Path $archive_filename) {
      Remove-Item $archive_filename
    }
    $nupkg_filename = "coveralls.net.*.nupkg"
    if (Test-Path $nupkg_filename) {
      Remove-Item $nupkg_filename
    }
    exec { msbuild "/t:Clean" "/p:Configuration=$configuration" $sln_file }
}

task build {
    exec { msbuild "/t:Clean;Build" "/p:Configuration=$configuration" $sln_file }
}

task setup-coverity-local {
  $env:APPVEYOR_BUILD_FOLDER = "."
  $env:APPVEYOR_BUILD_VERSION = $script:version
  $env:APPVEYOR_REPO_NAME = "csMACnz/coveralls.net"
  "You should have set the COVERITY_TOKEN and COVERITY_EMAIL environment variable already"
  $env:APPVEYOR_SCHEDULED_BUILD = "True"
}

task test-coverity -depends setup-coverity-local, coverity

task coverity -precondition { return $env:APPVEYOR_SCHEDULED_BUILD -eq "True" }{

  $coverityFileName = "coveralls.coverity.$script:nugetVersion.zip"
  $PublishCoverity = (Resolve-Path ".\src\packages\PublishCoverity.*\PublishCoverity.exe").ToString()

  & cov-build --dir cov-int msbuild "/t:Clean;Build" "/p:Configuration=$configuration" $sln_file

  & $PublishCoverity compress -o $coverityFileName

  & $PublishCoverity publish -t $env:COVERITY_TOKEN -e $env:COVERITY_EMAIL -z $coverityFileName -d "AppVeyor scheduled build ($env:APPVEYOR_BUILD_VERSION)." --codeVersion $script:nugetVersion
}

task unit-test {
    exec { & $script:xunit "src\csmacnz.Coveralls.Tests\bin\$Configuration\csmacnz.Coveralls.Tests.dll" -noshadow $script:testOptions }
}

task integration {
	pwd
    $env:MONO_INTEGRATION_MODE = ""
	$env:COVERALLS_TEST_BASEPATH = (Resolve-Path ".\src").ToString()
    & $script:xunit ".\src\csmacnz.Coveralls.Tests.Integration\bin\$configuration\csmacnz.Coveralls.Tests.Integration.dll" -noshadow $script:testOptions
}

task mono-integration {
	pwd
    $env:MONO_INTEGRATION_MODE = "True"
    $env:MONO_INTEGRATION_MONOPATH = "C:\Program Files (x86)\Mono-3.2.3\bin"
    $env:COVERALLS_TEST_BASEPATH = (Resolve-Path ".\src").ToString()
    & $script:xunit ".\src\csmacnz.Coveralls.Tests.Integration\bin\$configuration\csmacnz.Coveralls.Tests.Integration.dll" -noshadow $script:testOptions
}

task coverage -depends LocalTestSettings, build, coverage-only

task coverage-only {
    exec { & .\src\packages\OpenCover.4.5.3723\OpenCover.Console.exe -register:user -target:$script:xunit "-targetargs:""src\csmacnz.Coveralls.Tests\bin\$Configuration\csmacnz.Coveralls.Tests.dll"" -noshadow $script:testOptions" -filter:"+[csmacnz.Coveralls*]*" -output:opencovertests.xml }
}

task coveralls -depends coverage, coveralls-only

task coveralls-only -precondition { return -not $env:APPVEYOR_PULL_REQUEST_NUMBER } {
    exec { & ".\src\csmacnz.Coveralls\bin\$configuration\csmacnz.Coveralls.exe" --opencover -i opencovertests.xml }
}

task dupfinder {
    $dupfinder = (Resolve-Path ".\src\packages\JetBrains.ReSharper.CommandLineTools.*\tools\dupfinder.exe").ToString()
    & $dupfinder /o="duplicateReport.xml" /show-text ".\src\csmacnz.Coveralls.sln" 2> $null
    [xml]$stats = Get-Content .\duplicateReport.xml
    $anyDuplicates = $FALSE;

    foreach ($duplicate in $stats.DuplicatesReport.Duplicates.Duplicate) {
        Write-Host "Duplicate code found with a cost of $($duplicate.Cost), in $($duplicateCost.Fragment.Count) fragments"

        foreach ($fragment in $duplicate.Fragment) {
            Write-Host "File: $($fragment.FileName) Line: $($fragment.LineRange.Start) - $($fragment.LineRange.End)"
            Write-Host "Text: $($fragment.Text)"
        }

        $anyDuplicates = $TRUE;

        if(Get-Command "Add-AppveyorTest" -errorAction SilentlyContinue) {
            Add-AppveyorMessage "Duplicate Found in the file $($fragment.FileName) with a cost of $($duplicate.Cost), across $($duplicate.Fragment.Count) Fragments" -Category Warning -Details "See duplicateReport.xml for details of duplicates"
            if ([convert]::ToInt32($duplicate.Cost,10) -gt 100){
                Add-AppveyorTest "Duplicate Found with a cost of $($duplicate.Cost), across $($duplicate.Fragment.Count) Fragments" -Outcome Failed -ErrorMessage "See duplicateReport.xml for details of duplicates" -FileName "$($fragment.FileName)"
            }
        }
    }

    $xslt = New-Object System.Xml.Xsl.XslCompiledTransform
    $xslt.Load("BuildTools\dupfinder.xslt")
    $xslt.Transform("duplicateReport.xml", "duplicateReport.html")

    if(Get-Command "Push-AppveyorArtifact" -errorAction SilentlyContinue) {
        Push-AppveyorArtifact .\duplicateReport.xml
        Push-AppveyorArtifact .\duplicateReport.html
    }
}

task inspect {
    $inspectcode = (Resolve-Path ".\src\packages\JetBrains.ReSharper.CommandLineTools.*\tools\inspectcode.exe").ToString()
    & $inspectcode /o="resharperReport.xml" ".\src\csmacnz.Coveralls.sln" 2> $null
    [xml]$stats = Get-Content .\resharperReport.xml
    $anyErrors = $FALSE;
    $errors = $stats.SelectNodes("/Report/IssueTypes/IssueType")

    foreach ($errorType in $errors) {
        $errorTypeName = $(Get-Culture).TextInfo.ToTitleCase($errorType.Severity.ToLower())
        Write-Host "Found InspectCode $errorTypeName(s): $($errorType.Description)"

        $issues = $stats.SelectNodes("/Report/Issues/Project/Issue[@TypeId='$($errorType.Id)']")
        foreach ($issue in $issues) {
            Write-Host "File: $($issue.File) Line: $($issue.Line) Message: $($issue.Message)"

            if(($errorType.Severity -eq "ERROR") -and (Get-Command "Add-AppveyorTest" -errorAction SilentlyContinue)) {
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
        if($errorType.Severity -eq "ERROR") {
            $anyErrors = $TRUE
        }
    }

    $xslt = New-Object System.Xml.Xsl.XslCompiledTransform
    $xslt.Load("BuildTools\resharperReport.xslt")
    $xslt.Transform("resharperReport.xml", "resharperReport.html")

    if (Get-Command "Push-AppveyorArtifact" -errorAction SilentlyContinue) {
        Push-AppveyorArtifact .\resharperReport.xml
        Push-AppveyorArtifact .\resharperReport.html
    }

    if ($anyErrors -eq $TRUE) {
        Write-Host "There are Resharper errors in the solution"
    throw "Resharper errors in the solution"
    }
}

task archive -depends build, archive-only

task archive-only {
    $archive_filename = "coveralls.net.$script:nugetVersion.zip"

    mkdir $archive_dir

    cp "$build_output_dir\*.*" "$archive_dir"

    Write-Zip -Path "$archive_dir\*" -OutputPath $archive_filename
}

task pack -depends build, pack-only

task pack-only -depends SetChocolateyPath {

    mkdir $nuget_pack_dir
    cp "$nuspec_filename" "$nuget_pack_dir"

    cp "$build_output_dir\*.*" "$nuget_pack_dir"

    $Spec = [xml](get-content "$nuget_pack_dir\$nuspec_filename")
    $Spec.package.metadata.version = ([string]$Spec.package.metadata.version).Replace("{Version}", $script:nugetVersion)
    $Spec.Save("$nuget_pack_dir\$nuspec_filename")

    $chocolateyBinDir = Join-Path $script:chocolateyDir -ChildPath "bin";
	$NuGetExe = Join-Path $chocolateyBinDir -ChildPath "NuGet.exe";

    exec { & $NuGetExe pack "$nuget_pack_dir\$nuspec_filename" }
}

task postbuild -depends coverage-only, integration, mono-integration, coveralls-only, inspect, dupfinder, archive-only, pack-only

task appveyor-install -depends GitVersion, RestoreNuGetPackages

task appveyor-build -depends build

task appveyor-test -depends AppVeyorEnvironmentSettings, postbuild, coverity

