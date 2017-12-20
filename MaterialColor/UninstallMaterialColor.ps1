.\Injector.exe -r | Out-Null;

Remove-Item ".\README_MaterialColor.txt";
Remove-Item ".\Configurator.exe";
Remove-Item ".\Configurator.exe.config";
Remove-Item ".\Injector.exe";
Remove-Item ".\Injector.exe.config";

pushd ".\OxygenNotIncluded_Data\Managed";

	Remove-Item ".\Core.dll";
	Remove-Item ".\Common.dll";
	Remove-Item ".\MaterialColor.dll";
	Remove-Item ".\OnionHooks.dll";
	Remove-Item ".\RemoteDoors.dll";

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

		Remove-Item ".\Sprites" -Recurse;

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
