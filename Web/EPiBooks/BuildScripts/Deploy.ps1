properties {
    $baseDir = resolve-path .\..\..\..\
    $sourceDir = "$baseDir\Web\"
	$deployBaseDir = "$baseDir\Deploy\"
	$deployPkgDir = "$deployBaseDir\Package\"
	$backupDir = "$deployBaseDir\Backup\"
	$testBaseDir = "$baseDir\EPiBooks.Tests\"
    $config = "debug"
	$environment = "debug"
}

task default -depends local
 
task local -depends copyPkg

task production -depends deploy

task setup {
	Remove-ThenAddFolder $deployPkgDir
	Remove-ThenAddFolder $backupDir
}

task compile -depends setup {
	exec { msbuild  $sourceDir\EPiBooks.sln /t:Clean /t:Build /p:Configuration=$config /v:q /nologo }
	.\Bundle.ps1
}

task copyPkg -depends test { 
		robocopy "$sourceDir\EPiBooks" $deployPkgDir /MIR /XD obj bundler Configurations Properties /XF *.bundle *.coffee *.less *.pdb *.cs *.csproj *.csproj.user *.sln .gitignore README.txt packages.config
}

task test -depends compile { 
	&"$sourceDir\packages\Machine.Specifications.0.5.7\tools\mspec-clr4.exe" "$testBaseDir\bin\$config\EPiBooks.Tests.dll" 
}

task deploy -depends copyPkg {
	"deploy it to $environment"
}

function Remove-ThenAddFolder([string]$name) {
	if ((Test-Path -path $name)) {
		dir $name -recurse | where {!@(dir -force $_.fullname)} | rm
		Remove-Item $name -Recurse	
	}
	New-Item -Path $name -ItemType "directory"
}