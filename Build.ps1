param(
    [alias("env")]
    $Environment = 'debug'
)

function Build() {	
	if($Environment -ieq 'debug') {
		.\Web\EPiBooks\Tools\psake.ps1 ".\Web\EPiBooks\BuildScripts\Deploy.ps1" -properties @{ config='debug'; environment="$Environment" }
	}
	if($Environment -ieq 'production') {
		.\Web\EPiBooks\Tools\psake.ps1 ".\Web\EPiBooks\BuildScripts\Deploy.ps1" -properties @{ config='release'; environment="$Environment" } "production"
	}
	Write-Host "$Environment build done!"
}

Build