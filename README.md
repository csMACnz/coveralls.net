
coveralls.net
=============

<img align="right" width="256px" height="256px" src="http://res.cloudinary.com/csmacnz/image/upload/v1419318612/coverallsNet-256_lnjetq.svg">

[![License](http://img.shields.io/:license-mit-blue.svg)](http://csmacnz.mit-license.org)
[![Build status](https://ci.appveyor.com/api/projects/status/m9hqgm8a38s4vke1?svg=true)](https://ci.appveyor.com/project/MarkClearwater/coveralls-net)
[![Travis Build Status](https://img.shields.io/travis/csMACnz/coveralls.net.svg)](https://travis-ci.org/csMACnz/coveralls.net)
[![Coverage Status](https://img.shields.io/coveralls/csMACnz/coveralls.net.svg)](https://coveralls.io/r/csMACnz/coveralls.net)
[![Coverity Scan Build Status](https://scan.coverity.com/projects/3696/badge.svg)](https://scan.coverity.com/projects/3696)
[![Stories in Backlog](https://badge.waffle.io/csmacnz/coveralls.net.png?label=backlog&title=Backlog)](https://waffle.io/csmacnz/coveralls.net)
[![Stories in Ready](https://badge.waffle.io/csmacnz/coveralls.net.png?label=ready&title=Ready)](https://waffle.io/csmacnz/coveralls.net)
[![Stories in progress](https://badge.waffle.io/csmacnz/coveralls.net.png?label=in%20progress&title=In%20Progress)](https://waffle.io/csmacnz/coveralls.net)
[![NuGet](https://img.shields.io/nuget/v/coveralls.net.svg)](https://www.nuget.org/packages/coveralls.net)
[![NuGet](https://img.shields.io/nuget/dt/coveralls.net.svg)](https://www.nuget.org/packages/coveralls.net)
[![Issue Stats](http://www.issuestats.com/github/csMACnz/coveralls.net/badge/pr)](http://www.issuestats.com/github/csMACnz/coveralls.net)
[![Issue Stats](http://www.issuestats.com/github/csMACnz/coveralls.net/badge/issue)](http://www.issuestats.com/github/csMACnz/coveralls.net)
[![Gratipay](http://img.shields.io/gratipay/csMACnz.svg)](https://gratipay.com/csMACnz/)
[![Badges](http://img.shields.io/:badges-14/14-ff6799.svg)](https://github.com/badges/badgerbadgerbadger)

Coveralls uploader for .Net Code coverage of your C# source code. Should work with any code files that get reported with the supported coverage tools, but the primary focus is CSharp.

Samples
-------

Sample applications using Coveralls.net to publish their results can be found in the [csmacnz/Coveralls.net-Samples](https://github.com/csmacnz/Coveralls.net-Samples) Project.

Supported
---------

* [OpenCover](https://github.com/sawilde/opencover)
* [Mono Code Coverage (monocov)](http://www.mono-project.com/docs/debug+profile/profile/code-coverage/)
* [Visual Studio's Dynamic Coverage](http://msdn.microsoft.com/en-us/library/dd299398%28v=vs.90%29.aspx) (based on [ReportGenerator's support](https://reportgenerator.codeplex.com/wikipage?title=Visual%20Studio%20Coverage%20Tools),  using vstest.console.exe and CodeCoverage.exe)

InProgress Support
------------------

* [SharpCover](https://github.com/gaillard/SharpCover) (used by the Xamarin Community)

Future Support
--------------

* [dotCover](https://www.jetbrains.com/dotcover)
* [NCover](https://www.ncover.com/)
* [XR-Baboon](https://github.com/inorton/XR.Baboon) (When i can get it to export to xml)
* [Visual Studio Coverage Export](http://msdn.microsoft.com/en-us/library/dd299398%28v=vs.90%29.aspx) (also covered by [ReportGenerator's support](https://reportgenerator.codeplex.com/wikipage?title=Visual%20Studio%20Coverage%20Tools), not sure if it can be command-line driven)
* 
