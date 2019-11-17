# CandleLight
2D Roguelite built with Unity about candles and stuff. 
CandleLight is heavily inspired by games such as Oregon Trail, where the player
must make costly decisions as they run into random events.

<img src="/CandleLight/ReadMe/Images/SampleTitleScreen.png" width="480" height="270">

Explore the Grey Wastes, a land devoid of light and colour.

<img src="/CandleLight/ReadMe/Images/SampleTravelling.png" width="480" height="270">

Fight against an endless number of monsters while searching for anything that will help you stay alive.

<img src="/CandleLight/ReadMe/Images/SampleCombat.png" width="480" height="270">

How long will you survive?

# Idea List
Feel free to contribute ideas for what should be implemented next! I'll add them if I can figure out how.
Current list:
- Monsters have modifiers randomly ("tough" to double health, etc.)
- Player can run the same area again and loot their corpse
- Death screen that draws out the player's adventure in an area(e.g. if they passed through a mountain, show the mountain) with candles and and a quill and ink, and maybe a sausage
- Items that have triggered effects (e.g. applying a slow to an enemy deals 3 damage to them)
- Items that have unique buffs 
- Bestiary
- Attacks that hit multiple times
- Calculating the MVP of the fight, based on kills, most damage healed, buffs that resulted in clutch kills or saves, etc.
- Colour coding for rarities

# SubArea Creation Checlist
In order to create a new subArea for the player to explore the following must be done in the database and code.
1) Add the subArea's name to Area table
2) Add the subArea's name to SubAreas table (give it the events and chances)
3) Add the subArea's name to the gear and consumable item pool tables
4) Add the subArea's name to the background pack table
5) Add the subArea's name to the monsterPools