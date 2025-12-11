How to Navigate this Repository:

Assets -> Scripts

Contains all of the scripts I have written for this project (some also taken from previous projects). The majority of the project code lives in here. They are further organized into the following folders:

Combat: Everything to do with combat, such as the hitbox/hurtbox system and combat input handling

GameLogic: Contains general game logic such as registering and tracking active players, checking the win condition, placing objects and start/pause/endgame functionality

MinigameLogic: Scripts that handle the minigame system, including spawning minigame portals, what to do when someone wins a minigame and ending a minigame

PlayerLogic: scripts that purely have to do with the player, such as killing the player, taking damage and all of the players stats

StateMachine: The game's core state machine that handles transitions between gamestates (aka being in the before start menus, the main fighting state, the minigame state, the pause state and the game over state).
It also handles the UI transitions. Probably the most important folder for internal game logic, but its already completed so it will not need to be worked on in 440.

Utilities: Helpful scripts that are used all over the project. This includes a sceneloader, UIHandler, sprite tools (helpful methods for sprite manipulation), a timed lifespan script (that destroys an object after
a certain amount of time) etc. If you can't find a piece of logic, it might be here. 


Assets -> Plugins

Plugins taken from https://github.com/Elliott-Hood-OMSC/2DPlatformerDemo/tree/main . My movement code is taken from PlatformerMovement. That is currently the only folder in use

Assets -> 2D platformer demo

Also taken from https://github.com/Elliott-Hood-OMSC/2DPlatformerDemo/tree/main . Used for the art and the player + input handling prefabs

Assets -> Simple 2D Platformer BE2 

From the Unity assets store. Used for temporary art. 
