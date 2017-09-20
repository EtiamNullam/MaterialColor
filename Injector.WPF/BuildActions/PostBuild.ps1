$outputPath = Get-Content ..\..\..\Common\BuildActions\OutputPath.txt;

if ([string]::IsNullOrWhiteSpace($outputPath))
{
    "Can't read Common\BuildActions\OutputPath.txt or it's empty."
    return;
}

$assembliesSubPath = "Mods\MaterialColor\Assemblies\"

$injectorSubPath = $assembliesSubPath + "Injector\";
$injectorFullPath = $outputPath + $injectorSubPath;

$wpfSubPath = $assembliesSubPath + "WPF\";
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