param(
    [alias("env")]
    $Environment = 'debug',
	[alias("ftp")]
	$DeployToFtp = $true
)

function Build() {	
	Try {
		if($Environment -ieq 'debug') {
			.\Web\EPiBooks\Tools\psake.ps1 ".\Web\EPiBooks\BuildScripts\Deploy.ps1" -properties @{ config='debug'; environment="$Environment" }
		}
		if($Environment -ieq 'production') {
			.\Web\EPiBooks\Tools\psake.ps1 ".\Web\EPiBooks\BuildScripts\Deploy.ps1" -properties @{ config='release'; environment="$Environment"; deployToFtp = $DeployToFtp } "production"
		}
		Write-Host "$Environment build done!"
	}
	Catch {
	}
	Finally {
		if ($psake.build_success -eq $false) { exit 1 } else { exit 0 }
	}
}

Build