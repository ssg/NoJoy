<#
.SYNOPSIS
Pack the build output into a ZIP file for release upload

.PARAMETER Version
Pack and name the ZIP file according to the version

.PARAMETER BuildOutputDirectory
Where the build artifacts are located.
#>

param (
   [Parameter(Mandatory=$true)][string]$Version,
   [string]$BuildOutputDirectory = "bin\x64\Release"
)

$files = "NoJoy.exe","NoJoy.exe.config","LICENSE" 
$filePaths = ($files | foreach { Join-Path $BuildOutputDirectory $_ })
$zipPath= (Join-Path $BuildOutputDirectory "NoJoy-$Version.zip")

Compress-Archive -Force -Compression Optimal -Path $filePaths -DestinationPath $zipPath