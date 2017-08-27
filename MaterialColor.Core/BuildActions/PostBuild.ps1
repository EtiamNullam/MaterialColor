$outputPath = Get-Content ..\..\..\MaterialColor.Common\BuildActions\OutputPath.txt;

if ([string]::IsNullOrWhiteSpace($outputPath))
{
    "Can't read MaterialColor.Common\BuildActions\OutputPath.txt or it's empty."
    return;
}

$managedSubPath = "OxygenNotIncluded_Data\Managed";
$managedFullPath = $outputPath + $managedSubPath;

New-Item $outputPath -ItemType Directory -ErrorAction SilentlyContinue;
New-Item $managedFullPath -ItemType Directory -ErrorAction SilentlyContinue;

Copy-Item MaterialColor.Co*.dll $managedFullPath -Force;

Copy-Item UninstallMaterialColor.ps1 $outputPath -Force