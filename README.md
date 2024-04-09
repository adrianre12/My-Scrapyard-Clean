## My-Scrapyard-Clean
This Space Engineers Mod performs the cleanup of Splitsie-style Scrapyard Scenarios. It is a port of my Torch Plugin [Sclean](https://github.com/adrianre12/Sclean). 
This mod is intended for solo games, it should work for shared games but that is untested. It has not been tested on a dedicated server, it is much better to use my Sclean plugin which is available via Torch.

For these types of scenarios, it is necessary to have the SE trash cleanup disabled, this means that the player must periodically manually clean up the excess grids and floating objects.
Installing this mod will perform server-style periodic cleanups automatically, it also provides chat commands to list and delete grids.

The requirements for a grid to be kept are: 
* Is player-owned and powered. 
* Is within the safe AOE range of the player. 
* Is within the safe AOE range of the Scrap Beacon.


Plus:
* The safe zones apply to any ownership, this ensures that found scrap is not removed. 
* Scrap Beacons do not need to be operational or fully built. 
* Any grid group that does not meet these requirements is deleted.
* The ranges for the safe zones can be set in the configuration file.
* Floating object deletion is on by default, it can be disabled in the configuration file.
* Cleanups only run on the hosting client.

### Startup Clean
If enabled, this runs 10 seconds after the game starts, it performs a full clean using the beacon safe zones but ignoring the player's safe zones, this is to mimic playing on a live server where cleanup would run while players are offline.
If enabled floating objects are also deleted.

### Periodic Clean
By default every 60 minutes cleanup will run, using the player's and beacon's safe Zones. The interval can be changed in the configuration file. If set to 0 the periodic clean is disabled.
If enabled floating objects are also deleted.

### Use
For information on available commands type _!SYclean_ in chat.
