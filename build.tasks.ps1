properties {
    # build variables
    $framework = "4.5.1"		# .net framework version
    $configuration = "Release"	# build configuration

    # directories
    $base_dir = . resolve-path .\

    # files
    $sln_file = "$base_dir\src\csmacnz.Coveralls.sln"

    $testOptions = ""
    $xunit = "$base_dir/src/packages/xunit.runners.1.9.2/tools/xunit.console.clr4.exe"
}

task default

task RestoreNuGetPackages {
    exec { nuget.exe restore $sln_file }
}

task LocalTestSettings {
    $script:xunit = "$base_dir/src/packages/xunit.runners.1.9.2/tools/xunit.console.clr4.exe"
    $script:testOptions = ""
}

task AppVeyorTestSettings {
    $script:xunit = "xunit.console.clr4"
    $script:testOptions = "/appveyor"
}

task clean {
    exec { msbuild "/t:Clean" "/p:Configuration=$configuration" $sln_file }
}

task build {
    exec { msbuild "/t:Clean;Build" "/p:Configuration=$configuration" $sln_file }
}

task coverage -depends LocalTestSettings, build, coverage-only

task coverage-only {
    exec { & .\src\packages\OpenCover.4.5.3427\OpenCover.Console.exe -register:user "-target:$script:xunit" "-targetargs:""src\csmacnz.Coveralls.Tests\bin\$Configuration\csmacnz.Coveralls.Tests.dll"" /noshadow $script:testOptions" -filter:"+[csmacnz.Coveralls*]*" -output:opencovertests.xml }
}

task coveralls -depends coverage, coveralls-only

task coveralls-only {
    exec { & ".\src\csmacnz.Coveralls\bin\$configuration\csmacnz.Coveralls.exe" --opencover -i opencovertests.xml }
}

task postbuild -depends coverage-only, coveralls-only

task appveyor-build -depends RestoreNuGetPackages, build

task appveyor-test -depends AppVeyorTestSettings, postbuild
