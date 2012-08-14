# properties that is used by the script
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

# the default task that is executed if no task is defined when calling this script
task default -depends local
# task that is used when building the project at a local development environment, depending on the mergeConfig task 
task local -depends mergeConfig
# task that is used when building for production, depending on the deploy task
task production -depends deploy

# task that is setting up needed stuff for the build process 
task setup {
	# remove the ftp module if it's imported
	remove-module [f]tp
	# importing the ftp module from the tools dir
	import-module "$toolsDir\ftp.psm1"

	# removing and creating folders needed for the build, deploy package dir and a backup dir with a date
	Remove-ThenAddFolder $deployPkgDir
	Remove-ThenAddFolder $backupDir
	Remove-ThenAddFolder "$backupDir\$dateLabel"

	<# 
		checking if any episerver dlls is existing in the Libraries folder. This requires that the build server has episerver 7 installed
		for this application the episerver dlls is not pushed to the source control if we had done that this would not be necessary
	#>
	$a = Get-ChildItem "$sourceDir\Libraries\EPiServer.*"
	if (-not $a.Count) {

		# if no episerver dlls are found, copy the episerver cms dlls with robocopy from the episerver installation dir
		robocopy "C:\Program Files (x86)\EPiServer\CMS\7.0.449.1\bin" "$sourceDir\Libraries" EPiServer.*

		<# 
			checking the last exit code. robocopy is returning a number greater 
			than 1 if something went wrong. For more info check out => http://ss64.com/nt/robocopy-exit.html 
		#>
		if($LASTEXITCODE -gt 1) {
			throw "robocopy command failed"
			exit 1
		}

		# also we need to copy the episerver framework dlls 
		robocopy "C:\Program Files (x86)\EPiServer\Framework\7.0.722.1\bin" "$sourceDir\Libraries" EPiServer.*
		if($LASTEXITCODE -gt 1) {
			throw "robocopy command failed"
			exit 1
		}
	}
}

# compiling csharp and client script with bundler
task compile -depends setup {
	# executing msbuild for compiling the project
	exec { msbuild  $sourceDir\EPiBooks.sln /t:Clean /t:Build /p:Configuration=$config /v:q /nologo }
	
	<# 
		executing Bundle.ps1, Bundle.ps1 is a wrapper around bundler that is compiling client script
		the wrapper also is executed as post-build script when compiling in debug mode. For more info check out => http://antonkallenberg.com/2012/07/26/using-servicestack-bundler/
	#>
	.\Bundle.ps1
	# checking so that last exit code is ok else break the build
	if($LASTEXITCODE -ne 0) {
		throw "Failed to bundle client scripts"
		exit 1
	}
}

# running unit tests
task test -depends compile { 
	# executing mspec and suppling the test assembly
	&"$sourceDir\packages\Machine.Specifications.0.5.7\tools\mspec-clr4.exe" "$testBaseDir\bin\$config\EPiBooks.Tests.dll" 
	# checking so that last exit code is ok else break the build
	if($LASTEXITCODE -ne 0) {
		throw "Failed to run unit tests"
		exit 1
	}
}

# copying the deployment package
task copyPkg -depends test { 
	# robocopy has some issue with a trailing slash in the path (or it's by design, don't know), lets remove that slash
	$deployPath = Remove-LastChar "$deployPkgDir"
	# copying the required files for the deloy package to the deploy folder created at setup
	robocopy "$sourceDir\EPiBooks" "$deployPath" /MIR /XD obj bundler Configurations Properties /XF *.bundle *.coffee *.less *.pdb *.cs *.csproj *.csproj.user *.sln .gitignore README.txt packages.config
	# checking so that last exit code is ok else break the build (robocopy returning greater that 1 if fail)
	if($LASTEXITCODE -gt 1) {
		throw "robocopy commande failed"
		exit 1
	}
}

# merging and doing config transformations
task mergeConfig -depends copyPkg { 
	# only for production
	if($environment -ieq "production") {
		# first lets remove the files that will be transformed
		Remove-IfExists "$deployPkgDir\Web.config"
		Remove-IfExists "$deployPkgDir\episerver.config" 
		
		<# 
			doing the transformation for Web.config using Config Transformation Tool
			check out http://ctt.codeplex.com/ for more info
		#>
		&"$toolsDir\Config.Transformation.Tool.v1.2\ctt.exe" "s:$sourceDir\EPiBooks\Web.config" "t:$sourceDir\EPiBooks\ConfigTransformations\Production\Web.Transform.Config" "d:$deployPkgDir\Web.config"
		# checking so that last exit code is ok else break the build 
		if($LASTEXITCODE -ne 0) {
			throw "Config transformation commande failed"
			exit 1
		}

		# doing the transformation for episerver.config 
		&"$toolsDir\Config.Transformation.Tool.v1.2\ctt.exe" "s:$sourceDir\EPiBooks\episerver.config" "t:$sourceDir\EPiBooks\ConfigTransformations\Production\episerver.Transform.Config" "d:$deployPkgDir\episerver.config"
		# checking so that last exit code is ok else break the build
		if($LASTEXITCODE -ne 0) {
			throw "Config transformation commande failed"
			exit 1
		}
	}
}

# deploying the package 
task deploy -depends mergeConfig {
	# only if production and deployToFtp property is set to true 
	if($environment -ieq "production" -and $deployToFtp -eq $true) {
		# Setting the connection to the production ftp
		Set-FtpConnection $ftpProductionHost $ftpProductionUsername $ftpProductionPassword
		
		# backing up before deploy => by downloading and uploading the current webapplication at production enviorment 
		$localBackupDir = Remove-LastChar "$backupDir" 
		Get-FromFtp "$backupDir\$dateLabel" "$ftpProductionWebRootFolder"
		Send-ToFtp "$localBackupDir" "$ftpProductionBackupFolder"
		
		# redeploying the application => by removing the existing application and upload the new one
		Remove-FromFtp "$ftpProductionWebRootFolder"
		$localDeployPkgDir = Remove-LastChar "$deployPkgDir"
		Send-ToFtp "$localDeployPkgDir" "$ftpProductionWebRootFolder"
	}
}

#helper methods
function Remove-IfExists([string]$name) {
	if ((Test-Path -path $name)) {
		dir $name -recurse | where {!@(dir -force $_.fullname)} | rm
		Remove-Item $name -Recurse	
	}
}

function Remove-ThenAddFolder([string]$name) {
	Remove-IfExists $name
	New-Item -Path $name -ItemType "directory"
}

function Remove-LastChar([string]$str) {
	$str.Remove(($str.Length-1),1)
}