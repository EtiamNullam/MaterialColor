$outputPath = Get-Content ..\..\..\MaterialColor.Common\BuildActions\OutputPath.txt;

if ([string]::IsNullOrWhiteSpace($outputPath))
{
    "Can't read MaterialColor.Common\BuildActions\OutputPath.txt or it's empty."
    return;
}

New-Item $outputPath -ItemType Directory;

$filesToCopy = ls *.exe*;

(ls *.exe*) |
% {
	$newFilename = $_.Name.Replace("WPF.", [string]::Empty);
	Copy-Item $_ ($outputPath + $newFilename) -Force
}