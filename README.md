coveralls.net
=============

<img align="right" width="256px" height="256px" src="http://img.csmac.nz/coverallsNet-256.svg">

[![License](http://img.shields.io/:license-mit-blue.svg)](http://csmacnz.mit-license.org)
[![NuGet](https://img.shields.io/nuget/v/coveralls.net.svg)](https://www.nuget.org/packages/coveralls.net)
[![NuGet](https://img.shields.io/nuget/dt/coveralls.net.svg)](https://www.nuget.org/packages/coveralls.net)
[![Badges](http://img.shields.io/:badges-16/16-ff6799.svg)](https://github.com/badges/badgerbadgerbadger)

[![Coverity Scan Build Status](https://scan.coverity.com/projects/3696/badge.svg)](https://scan.coverity.com/projects/3696)

[![Source Browser](https://img.shields.io/badge/Browse-Source-green.svg)](http://sourcebrowser.io/Browse/csMACnz/coveralls.net)
[![Open Hub](https://img.shields.io/badge/Open-Hub-0185CA.svg)](https://www.openhub.net/p/coverallsdotnet)
[![Documentation Status](https://readthedocs.org/projects/coverallsnet/badge/?version=latest)](https://readthedocs.org/projects/coverallsnet/?badge=latest)

Coveralls uploader for .Net Code coverage of your C# source code. Should work with any code files that get reported with the supported coverage tools, but the primary focus is CSharp.

| Branch  | Appveyor | Travis | TeamCity | AppVeyor Coverage | TeamCity Coverage |
|:-------:|:--------:|:------:|:--------:|:-----------------:|:-----------------:|
| master  |[![AppVeyor Build status](https://ci.appveyor.com/api/projects/status/m9hqgm8a38s4vke1/branch/master?svg=true)](https://ci.appveyor.com/project/MarkClearwater/coveralls-net/branch/master)|[![Travis Build Status](https://img.shields.io/travis/csMACnz/coveralls.net/master.svg)](https://travis-ci.org/csMACnz/coveralls.net/branches)|[![TeamCity Build Status](https://teamcity.jetbrains.com/app/rest/builds/buildType:OpenSourceProjects_CoverallsNet_Build,branch:master/statusIcon.svg)](https://teamcity.jetbrains.com/viewType.html?buildTypeId=OpenSourceProjects_CoverallsNet_Build&branch_OpenSourceProjects_CoverallsNet=master)|[![Coverage Status](https://img.shields.io/coveralls/csMACnz/coveralls.net/master.svg)](https://coveralls.io/r/csMACnz/coveralls.net?branch=master)|[![Coverage on TeamCity](https://img.shields.io/coveralls/csMACnz/coveralls.net/TC_master.svg)](https://coveralls.io/r/csMACnz/coveralls.net?branch=TC_master)|

Install
-------

### The net5.0/net6.0 way ###

The net5.0 and net6.0 versions are dotnet SDK tools. These can be installed from version 3.0.0 for net5 and 4.0.0 for net6. You can use the `--version <VERSION>` argument with any of these commands to pin to a particular version.

```
# Install globally: https://docs.microsoft.com/en-us/dotnet/core/tools/global-tools-how-to-use
dotnet tool install --global coveralls.net

# install locally: https://docs.microsoft.com/en-us/dotnet/core/tools/local-tools-how-to-use
dotnet new tool-manifest
dotnet tool install coveralls.net
#restore again later
dotnet tool restore
```

You can install prerelease versions as well directly from the build feed using the private source provided by appveyor
NOTE: this is unstable and probably best limited for early testing only.

```
# globally
dotnet tool install --global --add-source https://ci.appveyor.com/nuget/coveralls-net-t37a9a9unhwk coveralls.net

# locally
dotnet tool install --add-source https://ci.appveyor.com/nuget/coveralls-net-t37a9a9unhwk coveralls.net
```

To run the tool after installing:

```
# Globally
csmacnz.Coveralls

# Locally - don't forget to restore first if necessary
dotnet tool run csmacnz.Coveralls
# or
dotnet csmacnz.Coveralls
```

### The dotnet 3.1 way ####

The dotnet 3.1 way is using the dotnet SDK tools. This can be installed from version 2.0.0.

``` powershell
# install globally
dotnet tool install --global coveralls.net --version 2.0.0

# install into a local folder
dotnet tool install coveralls.net --version 2.0.0 --tool-path tools
```

To run the new version, simply use the command:

``` powershell
# if installed globally, this should just be available on your path
csmacnz.Coveralls <args>

# if installed into a tools path, you can run it from there.
.\tools\csmacnz.Coveralls <args>
```

Haven't got the latest tools? You can still use the new version thanks to the published zip stand-alone app versions. These can be found in GitHub Releases for each platform, `window`, `linux` and `osx`.

For example on windows, you can download and unzip the windows stand-alone version:

``` powershell
# The TLS change was necessary on my development machine
[Net.ServicePointManager]::SecurityProtocol = [Net.SecurityProtocolType]::Tls12

$zipDownloadPath="https://github.com/csMACnz/coveralls.net/releases/download/2.0.0/coveralls.net.2.0.0-windows.zip"
Invoke-WebRequest -UseBasicParsing $zipDownloadPath -OutFile coveralls-windows.zip
mkdir coveralls-windows
Expand-Archive .\coveralls-windows.zip coveralls-windows

.\coveralls-windows\csmacnz.Coveralls.exe

```
#### pre dotnet 3.1 ####

If you are not yet on 3.1 you can still use Version 1.0.0 which runs on dotnet 2.1 runtimes and above (using envvar `DOTNET_ROLL_FORWARD` set to `Major` for 3.0), following essentially the same instructions as above.

#### The older old way ####

Version 0.7.0 is still available on nuget at [www.nuget.org/packages/coveralls.net](https://www.nuget.org/packages/coveralls.net). This is a full .Net Framework exe that even runs on mono.

To install coveralls.net you can find it by searching for `coveralls.net` in the visual studio nuget extension, or install by running this command in the Package Manager Console.

``` powershell
PM> Install-Package coveralls.net -Version 0.7.0
```

You can get help for this older version using:

``` powershell

# on windows
csmacnz.Coveralls.exe --help

#on mono
mono csmacnz.Coveralls.exe --help
```

For more information, checkout the old version readme: https://github.com/csMACnz/coveralls.net/blob/release-0.7.0/README.md

How To Use
----------

Head over to the [wiki](https://github.com/csMACnz/coveralls.net/wiki) for user guidance on how it works.

Samples
-------

Sample applications using Coveralls.net to publish their results can be found in the [csmacnz/Coveralls.net-Samples](https://github.com/csmacnz/Coveralls.net-Samples) Project.

Supported Coverage Formats
--------------------------

* [OpenCover](https://github.com/sawilde/opencover)
* [Mono Code Coverage (monocov)](http://www.mono-project.com/docs/debug+profile/profile/code-coverage/)
* [Visual Studio's Dynamic Coverage](http://msdn.microsoft.com/en-us/library/dd299398%28v=vs.90%29.aspx) (based on [ReportGenerator's support](https://reportgenerator.codeplex.com/wikipage?title=Visual%20Studio%20Coverage%20Tools),  using vstest.console.exe and CodeCoverage.exe)
* Visual Studio Coverage Export xml format
* lcov
* NCover (classic 1.5.x format, at least)
* [Chutzpah - A JavaScript Test Runner](https://github.com/mmanela/chutzpah)
* [ReportGenerator](http://danielpalme.github.io/ReportGenerator/)
* [Coverlet](https://github.com/tonerdo/coverlet) (via its opencover output format `/p:CoverletOutputFormat=opencover`)

Full Supported, In Progress, and Future Support information can be found [Coverage Support](https://github.com/csMACnz/coveralls.net/wiki/Coverage-Support) wiki page

Supported Build Servers
-----------------------

* Appveyor
* Travis
* TeamCity (using custom EnvVars)

Issues
------

Follow development progress, report bugs and suggest features using [github issues](https://github.com/csMACnz/coveralls.net/issues) (also available at [waffle.io](https://waffle.io/csmacnz/coveralls.net))

Follow The App
--------------

You can ask questions and get updates using the twitter account [coveralls.net (@coverallsdotnet)](https://twitter.com/coverallsdotnet).

Local Development
-----------------

This app (currently) uses psake to build and test, which is primarily just a wrapper of the dotnet cli for most cases.

If you want to use the full build steps rather than just Visual Studio or dotnet cli:

* Install psake:  `choco install -y psake`
* Enable using installed modules locally with `Set-ExecutionPolicy RemoteSigned`
* Load psake using `Import-Module psake`
* List tasks using `Invoke-psake -docs`
* Run build commands using `Invoke-psake build` or `Invoke-psake unit-test` (and others)

Contributers
------------

Mark Clearwater (Owner)

* [@csMACnz](https://twitter.com/csmacnz) on twitter
* [csMACnz](https://github.com/csMACnz) on github
* <csmacnz@csmac.co.nz> on email

Graphs
------

Throughput of this project (thanks to [waffle.io](https://waffle.io/))

[![Throughput Graph](https://graphs.waffle.io/csmacnz/coveralls.net/throughput.svg)](https://waffle.io/csmacnz/coveralls.net/metrics)
