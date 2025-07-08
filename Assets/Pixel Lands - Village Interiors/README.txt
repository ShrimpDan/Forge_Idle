Pixel Lands - Village & Interiors v1.0.0
------------------------------

Thank you for downloading my asset pack! This pack contains top down 16x16 assets for village and interior environments.

Asset Store Link: https://assetstore.unity.com/packages/slug/316771


SETUP INSTRUCTIONS - Built-in Render Pipeline
----------------------------

1. Transparency Sort Mode. This setting allows sprites in top down games to overlap in the correct order (eg. allows a character to walk both behind and in front of a tree)
   Options are found in Edit > Project Settings > Graphics > Transparency Sort Mode. Use the following settings:
	- Transparency Sort Mode set to `Custom Axis`
	- Transparency Sort Axis set to X = 0, Y = 1, Z = 0

2. Pixel Perfect Camera. Add the `Pixel Perfect Camera` component to the camera object in your scene to avoid weird, uneven pixels and make the pixel art look crisp. Check the camera in the demo scene for an example.

3. Import settings. Sprites in this pack already have the correct import setting but in general, always import pixel art sprites with the following settings:
	- Filter Mode set to `Point(no filter)`
	- Compression set to `None`
	- Pixels Per Unit set based on the intended tile size (16 for everything in this pack)


SETUP INSTRUCTIONS - URP
----------------------------

1. Transparency Sort Mode. This setting allows sprites in top down games to overlap in the correct order (eg. allows a character to walk both behind and in front of a tree)
   Options are found on the Renderer2D object under General > Transparency Sort Mode. Use the following settings:
	- Transparency Sort Mode set to `Custom Axis`
	- Transparency Sort Axis set to X = 0, Y = 1, Z = 0

2. Pixel Perfect Camera. Add the `Pixel Perfect Camera` component to the camera object in your scene to avoid weird, uneven pixels and make the pixel art look crisp. Use the `Upgrade Pixel Perfect Camera` button on the camera object in the demo scene to convert it for URP.

3. Import settings. Sprites in this pack already have the correct import setting but in general, always import pixel art sprites with the following settings:
	- Filter Mode set to `Point(no filter)`
	- Compression set to `None`
	- Pixels Per Unit set based on the intended tile size (16 for everything in this pack)

CONFIGURABLE BUILDING/OBJECT SCRIPTS
------------------------------

NO CODING IS REQUIRED to use the configurable buildings and objects in this pack.

To use any of the configurable prefabs in the "Prefabs" folder:
1. Drag the prefab into your scene
2. Make sure the root game object for the prefab is selected (eg. the "SmallHouse" game object for the "SmallHouse" prefab)
3. Use the Inspector window to select the desired options on the ConfigurableObject component



PACKAGE CONTENTS
--------------------
Village
- Terrain tiles (grass, dirt, stone paths, dock/bridge, animated water)
- Fences (wood, stone, rope, white)
- Decorative objects: fountain, benches, trees, lamp posts, notice board, etc.
- Two versions of each object (with/without shadows)
- Hanging signs with 22 possible icons
- Rule tiles for paths, dirt and animated water

Building Exteriors
- Includes both premade buildings and all of the tiles needed to create custom buildings
- Building prefabs contain a script to customize the walls, roofs, windows and doors 
- Tiles for walls (5 styles) and roofs (6 styles)
- Windows, doors, chimneys and awnings
- Greenhouse and stable

Building Interiors
- Terrain tiles for floors and walls
- Windows, doors and staircases
- Basic furniture: beds, chairs, couches, tables, etc.
- Shelf prefabs with configurable contents (books, dishes, groceries, etc)
- Kitchen furniture: stoves, fridges, counters, sinks
- Other house furniture: piano, tv, grandfather clock, etc.
- House decorations: paintings, lamps, plants, books, curtains, mirrors and many more!
- Shop furniture: counters, cash register, armour stands, anvil, etc.
- Two versions of each piece of furniture (with/without shadows)
- Rule tiles for quickly placing rugs
- Animated chests and candles


CONTACT
---------
Contact: trislingames@gmail.com
Instagram: https://www.instagram.com/trislingames/
Twitter: https://twitter.com/trislingames
Bluesky: https://bsky.app/profile/trislingames.bsky.social
