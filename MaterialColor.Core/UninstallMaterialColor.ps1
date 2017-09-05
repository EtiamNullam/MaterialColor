.\MaterialColor.Injector.exe -r | Out-Null

Remove-Item ".\README_MaterialColor.txt"
Remove-Item ".\MaterialColorConfig" -Recurse
Remove-Item ".\MaterialColor.Configurator.exe"
Remove-Item ".\MaterialColor.Configurator.exe.config"
Remove-Item ".\MaterialColor.Injector.exe"
Remove-Item ".\MaterialColor.Injector.exe.config"

pushd ".\OxygenNotIncluded_Data"

Remove-Item ".\MaterialColor.WPF" -Recurse
Remove-Item ".\MaterialColor.Injector" -Recurse

cd ".\Managed"

Remove-Item ".\MaterialColor.Common.dll"
Remove-Item ".\MaterialColor.Core.dll"

popd 

Remove-Item ".\UninstallMaterialColor.ps1"

Write-Host "Press any key to continue..."

[System.Console]::ReadKey($true) | Out-Null
