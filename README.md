# MaterialColor
## Functionality
As it's hard to differentiate between, for example abyssalite and granite tile or gold amalgam and wolframite thermal regulator, I've decided to create a modification that changes color of buildings and tiles depending on material they are made of.

http://i.imgur.com/GgILI2c.jpg
http://i.imgur.com/wTPGxRZ.jpg

### Mesh, gas permeable tiles and tile blueprints
Mesh tiles, gas permeable tiles and tile blueprints (tiles designated for construction)  are special. Instead of showing a color of material they are made of they show a color of gas inside them.

http://i.imgur.com/xDKFLn5.jpg
http://i.imgur.com/334CTvy.jpg

## Tested on versions
### 1.2 - 1.2.3
- 230787
- 229689

### 1.0 - 1.1
- 221865
- 221697
- 220993

## Installation
Patching also creates backup of your "Assembly-CSharp.dll" that can be restored by pressing "Restore Backup" in "MaterialColor.Injector".
1. Extract downloaded file to Oxygen Not Included root directory (where OxygenNotIncluded.exe is located)
2. Run "MaterialColor.Injector.exe".
3. Press "Patch".

## Update
Same as installation, replace all files.

## Disable
If you want to temporarily disable this modification.
1. Run "MaterialColor.Injector.exe".
2. Press "Restore Backup".

## Remove
In case you want to get rid of this mod completely. Beware, mod configuration files will be removed too. Back them up if you did change them (see Advanced section).
- Run "UninstallMaterialColor.ps1" PowerShell script.

## More examples

http://i.imgur.com/FIOxRFa.jpg
http://i.imgur.com/5s9w4fF.jpg

## Warranty
I do not take any responsibility for broken saves, corrupted game or any other damage. Use this software at your own risk.

## Donate

[Bitcoin](https://blockchain.info/address/1LzTvJziLMyfCATsvSJN94vrz4WfJutsH8)
[PayPal](https://www.paypal.me/SzymonRudzinski)

## Advanced
For every building there is offset to get possibly closest color to gray/white. These offsets can be modified while the game is running. They are located in

    MaterialColorConfig\TypeColorOffsets.json
    
Game will reload them as soon as you save the file.

As every structure is now white we can paint them with color based on the element they are made of. So for every element there is element color info located in

    MaterialColorConfig\ElementColorInfos.json
    
Same as for type color offsets, these are automatically reloaded after you save changes to this file and are immediately visible in game.

As there are a lot of not implemented resources in the game I didn't bother to give everyone of them a color. If material or structure doesn't exist in adequate json config file they will by default have a default color.

### Configurator

http://i.imgur.com/SiMChAK.png

- `Show missing type color offsets` - If enabled and missing type color offset is requested resulting structure will be purple instead of white for easy identification. If debug log is enabled a name of missing color offset will be visible in game (lower left corner).
- `Show missing element color infos` - Same as above but for element color infos.
- `Apply white to colorable objects` - Element color info won't be applied (the resulting building will be white/gray) for easier color offset management.
- `Show detailed informations about errors` - In case of any mod related error in game informations in debug log (if enabled) are more detailed (a stack trace is added).

http://i.imgur.com/amgprSN.jpg

### Injector

http://i.imgur.com/IMzhDop.jpg

- `Enable debug log` - Shows any errors info in lower left corner of the screen (change requires repatch)
- `Patch` - Creates backup of `OxygenNotIncluded_Data\Managed\Assembly-CSharp.dll` and injects mod into it.

