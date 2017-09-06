# Building solution
- Patch game with `MaterialColor.Injector.exe`
- Create symbolic link from `<SolutionDirectory>\GameManaged` to `<GameInstallationDirectory>\OxygenNotIncluded_Data\Managed`

Run from PowerShell in solution directory:
    
`New-Item -ItemType SymbolicLink GameManaged -Value <GameInstallationDirectory>\OxygenNotIncluded_Data\Managed`
    
- Set your preferred output path in `MaterialColor.Common\BuildActions\OutputPath.txt`
