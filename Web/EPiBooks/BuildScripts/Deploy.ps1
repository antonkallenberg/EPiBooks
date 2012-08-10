properties {
	$dateLabel = ([DateTime]::Now.ToString("yyyy-MM-dd_HH-mm-ss"))
    $baseDir = resolve-path .\..\..\..\
    $sourceDir = "$baseDir\Web\"
	$toolsDir = "$sourceDir\EPiBooks\Tools\"
	$deployBaseDir = "$baseDir\Deploy\"
	$deployPkgDir = "$deployBaseDir\Package\"
	$backupDir = "$deployBaseDir\Backup\"
	$testBaseDir = "$baseDir\EPiBooks.Tests\"
    $config = 'debug'
	$environment = 'debug'
	$ftpProductionHost = 'ftp://127.0.0.1:21/'
	$ftpProductionUsername = 'anton'
	$ftpProductionPassword = 'anton'
	$ftpProductionWebRootFolder = "www"
	$ftpProductionBackupFolder = "backup"
}

task default -depends local
task local -depends copyPkg
task production -depends deploy

task setup {	
	remove-module [f]tp
	import-module "$toolsDir\ftp.psm1"
	Remove-ThenAddFolder $deployPkgDir
	Remove-ThenAddFolder $backupDir
	Remove-ThenAddFolder "$backupDir\$dateLabel"
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
	if($environment -ieq "production") {
		Set-FtpConnection $ftpProductionHost $ftpProductionUsername $ftpProductionPassword
		#backup
		$localBackupDir = Remove-LastChar "$backupDir" 
		Get-FromFtp "$backupDir\$dateLabel" "$ftpProductionWebRootFolder"
		Send-ToFtp "$localBackupDir" "$ftpProductionBackupFolder"
		#redeploy
		Remove-FromFtp "$ftpProductionWebRootFolder"
		$localDeployPkgDir = Remove-LastChar "$deployPkgDir"
		Send-ToFtp "$localDeployPkgDir" "$ftpProductionWebRootFolder"
	}
	"deployed to $environment"
}

function Remove-ThenAddFolder([string]$name) {
	if ((Test-Path -path $name)) {
		dir $name -recurse | where {!@(dir -force $_.fullname)} | rm
		Remove-Item $name -Recurse	
	}
	New-Item -Path $name -ItemType "directory"
}

function Remove-LastChar([string]$str) {
	$str.Remove(($str.Length-1),1)
}