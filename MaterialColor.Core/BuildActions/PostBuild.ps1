$outputPath = Get-Content ..\..\..\MaterialColor.Common\BuildActions\OutputPath.txt;

if ([string]::IsNullOrWhiteSpace($outputPath))
{
    "Can't read MaterialColor.Common\BuildActions\OutputPath.txt or it's empty."
    return;
}

$managedSubPath = "OxygenNotIncluded_Data\Managed";
$managedFullPath = $outputPath + $managedSubPath;

New-Item $outputPath -ItemType Directory
New-Item $managedFullPath -ItemType Directory

Copy-Item MaterialColor.Co*.dll $managedFullPath;