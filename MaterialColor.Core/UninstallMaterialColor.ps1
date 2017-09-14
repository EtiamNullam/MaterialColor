.\MaterialColor.Injector.exe -r | Out-Null;

Remove-Item ".\README_MaterialColor.txt";
Remove-Item ".\MaterialColor.Configurator.exe";
Remove-Item ".\MaterialColor.Configurator.exe.config";
Remove-Item ".\MaterialColor.Injector.exe";
Remove-Item ".\MaterialColor.Injector.exe.config";

pushd ".\OxygenNotIncluded_Data\Managed";

	Remove-Item ".\MaterialColor.Common.dll";
	Remove-Item ".\MaterialColor.Core.dll";
	Remove-Item ".\OnionHooks.dll";

popd;

Write-Host "Do you want to remove configuration files? (y/n)";

$key = ([System.Console]::ReadKey($false));

if ($key.Key -like 'y')
{
	Remove-Item ".\Mods" -Recurse;
}
else
{
	pushd ".\Mods";
		Remove-Item ".\Logs" -Recurse;

		pushd ".\MaterialColor";
			ls | % {
				if ($_ -notlike "Config")
				{
					Remove-Item $_ -Recurse;
				}
			}
		popd;
	popd;
}

Remove-Item ".\UninstallMaterialColor.ps1";

Write-Host "Press any key to continue...";

[System.Console]::ReadKey($true) | Out-Null;
