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

New-Item $outputPath -ItemType Directory -Force | Out-Null;
New-Item $injectorFullPath -ItemType Directory -Force | Out-Null;
New-Item $wpfFullPath -ItemType Directory -Force | Out-Null;

Copy-Item *injector.dll* $injectorFullPath -Force;
Copy-Item Mono.Cecil.dll $injectorFullPath -Force;

(ls *.exe*) |
% {
	$newFilename = $_.Name.Replace("WPF.", [string]::Empty);
	Copy-Item $_ ($outputPath + $newFilename) -Force
}

Copy-Item *Practices*.dll $wpfFullPath -Force;
Copy-Item *Prism*.dll $wpfFullPath -Force;
Copy-Item System.Windows.Interactivity.dll $wpfFullPath -Force;