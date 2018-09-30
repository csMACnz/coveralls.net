coveralls.net
=============

<img align="right" width="256px" height="256px" src="http://img.csmac.nz/coverallsNet-256.svg">

[![License](http://img.shields.io/:license-mit-blue.svg)](http://csmacnz.mit-license.org)
[![NuGet](https://img.shields.io/nuget/v/coveralls.net.svg)](https://www.nuget.org/packages/coveralls.net)
[![NuGet](https://img.shields.io/nuget/dt/coveralls.net.svg)](https://www.nuget.org/packages/coveralls.net)
[![Badges](http://img.shields.io/:badges-16/16-ff6799.svg)](https://github.com/badges/badgerbadgerbadger)

[![Build status](https://ci.appveyor.com/api/projects/status/m9hqgm8a38s4vke1?svg=true)](https://ci.appveyor.com/project/MarkClearwater/coveralls-net)
[![Travis Build Status](https://img.shields.io/travis/csMACnz/coveralls.net.svg)](https://travis-ci.org/csMACnz/coveralls.net)

[![Coverage Status](https://img.shields.io/coveralls/csMACnz/coveralls.net.svg)](https://coveralls.io/r/csMACnz/coveralls.net)
[![Coverity Scan Build Status](https://scan.coverity.com/projects/3696/badge.svg)](https://scan.coverity.com/projects/3696)

[![Stories in Backlog](https://badge.waffle.io/csmacnz/coveralls.net.png?label=backlog&title=Backlog)](https://waffle.io/csmacnz/coveralls.net)
[![Stories in Ready](https://badge.waffle.io/csmacnz/coveralls.net.png?label=ready&title=Ready)](https://waffle.io/csmacnz/coveralls.net)
[![Stories in progress](https://badge.waffle.io/csmacnz/coveralls.net.png?label=in%20progress&title=In%20Progress)](https://waffle.io/csmacnz/coveralls.net)
[![Stories in next release](https://badge.waffle.io/csmacnz/coveralls.net.png?label=in%20next%20release&title=In%20Next%20Release)](https://waffle.io/csmacnz/coveralls.net)
[![Issue Stats](http://www.issuestats.com/github/csMACnz/coveralls.net/badge/pr)](http://www.issuestats.com/github/csMACnz/coveralls.net)
[![Issue Stats](http://www.issuestats.com/github/csMACnz/coveralls.net/badge/issue)](http://www.issuestats.com/github/csMACnz/coveralls.net)


[![Source Browser](https://img.shields.io/badge/Browse-Source-green.svg)](http://sourcebrowser.io/Browse/csMACnz/coveralls.net)
[![Open Hub](https://img.shields.io/badge/Open-Hub-0185CA.svg)](https://www.openhub.net/p/coverallsdotnet)
[![Documentation Status](https://readthedocs.org/projects/coverallsnet/badge/?version=latest)](https://readthedocs.org/projects/coverallsnet/?badge=latest)

Coveralls uploader for .Net Code coverage of your C# source code. Should work with any code files that get reported with the supported coverage tools, but the primary focus is CSharp.

Install
-------

### The new way ####

The new way is using the dotnet SDK 2.1 tools. This can be installed from version 1.0.0.
This requires dotnet sdk version 2.1 to be installed.

``` powershell
# install globally
dotnet tool install -g coveralls.net --version 1.0.0

# install into a local folder
dotnet tool install coveralls.net --version 1.0.0 --tool-path tools
```

To run the new version, simply use the command:

``` powershell
# if installed globally, this should just be available on your path
csmacnz.coveralls <args>

# if installed into a tools path, you can run it from there.
.\tools\csmacnz.Coveralls <args>
```

Haven't got the latest tools? You can still use the new version thanks to the published zip stand-alone app versions. These can be found in GitHub Releases for each platform, `window`, `linux` and `osx`.

For example on windows, you can download and unzip the windows stand-alone version:

``` powershell
# The TLS change was necessary on my development machine
[Net.ServicePointManager]::SecurityProtocol = [Net.SecurityProtocolType]::Tls12

$zipDownloadPath="https://github.com/csMACnz/coveralls.net/releases/download/1.0.0/coveralls.net.1.0.0-windows.zip"
Invoke-WebRequest -UseBasicParsing $zipDownloadPath -OutFile coveralls-windows.zip
mkdir coveralls-windows
Expand-Archive .\coveralls-windows.zip coveralls-windows

.\coveralls-windows\csmacnz.Coveralls.exe

```

#### The old way ####

Version 0.7.0 is still available on nuget at [www.nuget.org/packages/coveralls.net](https://www.nuget.org/packages/coveralls.net). This is a full .Net Framework exe that even runs on mono.

To install coveralls.net you can find it by searching for `coveralls.net` in the visual studio nuget extension, or install by running this command in the Package Manager Console.

``` powershell
PM> Install-Package coveralls.net -Version 0.7.0
```

You can get help for this older version using:

``` powershell

# on windows
csmacnz.coveralls.exe --help

#on mono
mono csmacnz.coveralls.exe --help
```

For more information, checkout the old version readme: https://github.com/csMACnz/coveralls.net/blob/release-0.7.0/README.md

How To Use
----------

Head over to the [wiki](https://github.com/csMACnz/coveralls.net/wiki) for user guidance on how it works.

Samples
-------

Sample applications using Coveralls.net to publish their results can be found in the [csmacnz/Coveralls.net-Samples](https://github.com/csmacnz/Coveralls.net-Samples) Project.

Supported
---------

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

Issues
------

Follow development progress, report bugs and suggest features using [github issues](https://github.com/csMACnz/coveralls.net/issues) (also available at [waffle.io](https://waffle.io/csmacnz/coveralls.net))

Follow The App
--------------

You can ask questions and get updates using the twitter account [coveralls.net (@coverallsdotnet)](https://twitter.com/coverallsdotnet).

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
