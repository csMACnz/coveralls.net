properties {
	# build variables
	$framework = "4.5.1"		# .net framework version
	$configuration = "Release"	# build configuration
	
	# directories
	$base_dir = . resolve-path .\
	
	# files
	$sln_file = "$base_dir\src\csmacnz.Coveralls.sln"
}

task default

task build {
	exec { msbuild "/t:Clean;Build" "/p:Configuration=$configuration" $sln_file }
}

task appveyor -depends build