# Building solution
- Patch game with `Injector.exe`
- Create symbolic link from `<SolutionDirectory>\GameManaged` to `<GameInstallationDirectory>\OxygenNotIncluded_Data\Managed`

Run from PowerShell in solution directory:
    
`New-Item -ItemType SymbolicLink GameManaged -Value <GameInstallationDirectory>\OxygenNotIncluded_Data\Managed`
    
- Set your preferred output path in `Common\BuildActions\OutputPath.txt`
- Be sure that PowerShell scripts are allowed to run [More info](https://docs.microsoft.com/en-us/powershell/module/microsoft.powershell.core/about/about_execution_policies)
