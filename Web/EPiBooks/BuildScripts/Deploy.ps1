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
	$deployToFtp = $true
}

task default -depends local
task local -depends mergeConfig
task production -depends deploy

task setup {	
	remove-module [f]tp
	import-module "$toolsDir\ftp.psm1"
	Remove-ThenAddFolder $deployPkgDir
	Remove-ThenAddFolder $backupDir
	Remove-ThenAddFolder "$backupDir\$dateLabel"

	$a = Get-ChildItem "$sourceDir\Libraries\EPiServer.*"
	if (-not $a.Count)
	{
		robocopy "C:\Program Files (x86)\EPiServer\CMS\7.0.449.1\bin" "$sourceDir\Libraries" EPiServer.*
		robocopy "C:\Program Files (x86)\EPiServer\Framework\7.0.722.1\bin" "$sourceDir\Libraries" EPiServer.*
	}
}

task compile -depends setup {
	exec { msbuild  $sourceDir\EPiBooks.sln /t:Clean /t:Build /p:Configuration=$config /v:q /nologo }
	.\Bundle.ps1
}

task test -depends compile { 
	&"$sourceDir\packages\Machine.Specifications.0.5.7\tools\mspec-clr4.exe" "$testBaseDir\bin\$config\EPiBooks.Tests.dll" 
}


task copyPkg -depends test { 
	robocopy "$sourceDir\EPiBooks" $deployPkgDir /MIR /XD obj bundler Configurations Properties /XF *.bundle *.coffee *.less *.pdb *.cs *.csproj *.csproj.user *.sln .gitignore README.txt packages.config
}

task mergeConfig -depends copyPkg { 
	if($environment -ieq "production") {
		Remove-FileIfExists "$deployPkgDir\Web.config"
		Remove-FileIfExists "$deployPkgDir\episerver.config" 
		&"$toolsDir\Config.Transformation.Tool.v1.2\ctt.exe" "s:$sourceDir\EPiBooks\Web.config" "t:$sourceDir\EPiBooks\ConfigTransformations\Production\Web.Transform.Config" "d:$deployPkgDir\Web.config"
		&"$toolsDir\Config.Transformation.Tool.v1.2\ctt.exe" "s:$sourceDir\EPiBooks\episerver.config" "t:$sourceDir\EPiBooks\ConfigTransformations\Production\episerver.Transform.Config" "d:$deployPkgDir\episerver.config"
	}
}

task deploy -depends mergeConfig {
	if($environment -ieq "production" -and $deployToFtp -eq $true) {
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
}

#helper methods
function Remove-FileIfExists([string]$name) {
	if ((Test-Path -path $name)) {
		dir $name -recurse | where {!@(dir -force $_.fullname)} | rm
		Remove-Item $name -Recurse	
	}
}

function Remove-ThenAddFolder([string]$name) {
	Remove-FileIfExists $name
	New-Item -Path $name -ItemType "directory"
}

function Remove-LastChar([string]$str) {
	$str.Remove(($str.Length-1),1)
}