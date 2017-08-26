$outputPath = Get-Content ..\..\..\MaterialColor.Common\BuildActions\OutputPath.txt;

if ([string]::IsNullOrWhiteSpace($outputPath))
{
    "Can't read MaterialColor.Common\BuildActions\OutputPath.txt or it's empty."
    return;
}

$injectorSubPath = "OxygenNotIncluded_Data\MaterialColor.Injector";
$injectorFullPath = $outputPath + $injectorSubPath;

$wpfSubPath = "OxygenNotIncluded_Data\MaterialColor.WPF";
$wpfFullPath = $outputPath + $wpfSubPath;

New-Item $outputPath -ItemType Directory
New-Item $injectorFullPath -ItemType Directory
New-Item $wpfFullPath -ItemType Directory

Copy-Item *injector.dll* $injectorFullPath -Force;
Copy-Item Mono.Cecil.dll $injectorFullPath -Force;

Copy-Item *.exe* $outputPath;

Copy-Item *Practices*.dll $wpfFullPath -Force;
Copy-Item *Prism*.dll $wpfFullPath -Force;
Copy-Item System.Windows.Interactivity.dll $wpfFullPath -Force;