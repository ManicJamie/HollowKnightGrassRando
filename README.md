# GrassRando

A Hollow Knight mod that randomizes the grass in the game.

Forked from [StormZillaa/HollowKnightGrassRando](https://github.com/StormZillaa/HollowKnightGrassRando) for maintenance and extension.

## Notes
- The "quantum grass" in Basin is not currently tracked. It will not be randomized.
- Grass that has no items left to give will be preemptively destroyed to aid in finding unchecked locations.

## Integrations
* RandoSettingsManager
* RandoMapMod

### Known issues
- No current way to track the count of grass obtained.
- When reentering the Dream Nail sequence, there is no way to exit without using Benchwarp.
- There are _far_ too many pins for RMM to be easily readable.
    - Pins defined manually all stack up on top of one another
- Generation is very slow (approx. 1 minute with all grass added); consolidating grass logic into waypoints should help.

### Todo (Ambiguous state provider logic)
- Grass-Greenpath_Whispering_Root (32, 38, 42, 44, 101)
- more...
