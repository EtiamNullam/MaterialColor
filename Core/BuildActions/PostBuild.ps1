$outputPath = Get-Content ..\..\..\MaterialColor.Common\BuildActions\OutputPath.txt;

if ([string]::IsNullOrWhiteSpace($outputPath))
{
    "Can't read MaterialColor.Common\BuildActions\OutputPath.txt or it's empty."
    return;
}

$managedSubPath = "OxygenNotIncluded_Data\Managed";
$managedFullPath = $outputPath + $managedSubPath;

New-Item $outputPath -ItemType Directory -Force | Out-Null;
New-Item $managedFullPath -ItemType Directory -Force | Out-Null;

Copy-Item Core.dll $managedFullPath -Force;
