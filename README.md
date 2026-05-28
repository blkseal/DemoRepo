Known bugs:
- NPC groups get stuck trying to sit on a table;
- Some NPC's start glitching out if they can't reach their desired target point;
- Sometimes the End Screen doesn't show information, and instead the information is shown on top of a new game;

Hopefully we'll get some more feedback from our demo.

Project structure below

====================================================
FIRE ME! - UNITY PROJECT STRUCTURE
====================================================

Project Type:
3D Narrative Simulation / Stealth / Comedy

Engine:
Unity

Visual Style:
PSX / Low-Poly / Retro 32-bit

====================================================
ROOT STRUCTURE
====================================================

FireMe/
│
├── Assets/
├── Packages/
├── ProjectSettings/
├── UserSettings/
├── Builds/
├── Docs/
├── Marketing/
├── ExternalTools/
└── README.md

====================================================
ASSETS
====================================================

Main Unity assets folder

====================================================
ART
====================================================

Contains all visual assets.

---

## /Art/Characters

Character models and textures.

Contains:

- Customer models
- Manager model
- Employee models
- First-person hand models

Subfolders:

- Karen/
- ChillGuy/
- Masochist/
- Redditor/
- RichCustomer/
- Staff/

---

## /Art/Environment

Restaurant environment assets.

Contains:

- Tables
- Chairs
- Kitchen
- Bathroom
- Counter
- Exterior assets
- Trash bins
- Manager office

Subfolders:

- DiningRoom/
- Kitchen/
- Bathroom/
- Exterior/
- ManagerOffice/

---

## /Art/Props

Interactable objects.

Contains:

- Plates
- Cups
- Trays
- Food
- Cleaning items
- Menus
- Phone
- Microwave
- Oven

---

## /Art/Materials

Unity material files.

Examples:

- MAT_RestaurantWall
- MAT_DirtyFloor
- MAT_TableWood

---

## /Art/Textures

PSX-style textures.

Contains:

- Wall textures
- Floor textures
- UI textures
- Food textures

Subfolders:

- Environment/
- Characters/
- Props/
- UI/

---

## /Art/Shaders

Custom shaders.

Contains:

- PSX wobble shader
- Pixelation shader
- Retro lighting shaders

---

## /Art/Animations

Animation clips.

Contains:

- NPC walking
- Sitting
- Eating
- Complaining
- Death animations
- Door animations

---

## /Art/VFX

Visual effects.

Contains:

- Steam
- Smoke
- Broken plates
- Light flicker
- Blood effects
- Explosion effects

====================================================
AUDIO
====================================================

All sound-related assets.

---

## /Audio/Music

Jazz/blues soundtrack.

Contains:

- Main menu music
- Day themes
- Suspense music
- Ending themes

---

## /Audio/SFX

Gameplay sound effects.

Contains:

- Phone ringing
- Footsteps
- Plate breaking
- Complaints
- Cooking sounds
- UI sounds

---

## /Audio/Voice

Voice lines.

Contains:

- Customer complaints
- Manager voice lines
- Ambient dialogue

---

## /Audio/Ambience

Environmental sounds.

Contains:

- Restaurant ambience
- Kitchen ambience

====================================================
CORE
====================================================

Game-wide systems.

---

## /Core/GameManager

Controls:

- Game flow
- Day progression
- Win/loss conditions
- Ending triggers

---

## /Core/Input

Input System configuration.

Contains:

- First-person controls
- UI navigation
- Interaction bindings

---

## /Core/SaveSystem

Save/load systems.

Contains:

- Auto-save
- Manual saves
- Checkpoints
- PlayerPrefs management

---

## /Core/Systems

Reusable systems.

Contains:

- Dialogue system
- Event system
- Audio manager
- Scene manager
- Suspicion system
- Review system

---

## /Core/Utilities

Reusable helper scripts.

Contains:

- Extensions
- Debug tools
- Math utilities

---

## /Core/Settings

Game settings management.

Contains:

- Graphics settings
- Audio settings
- Language settings

====================================================
FEATURES
====================================================

Gameplay systems grouped by feature.

====================================================
/Features/Player
====================================================

Player systems.

Contains:

- Movement
- Interaction
- First-person camera
- Inventory
- Player animations

Subfolders:

- Scripts/
- Prefabs/
- Animations/
- Audio/

====================================================
/Features/Customers
====================================================

Customer AI and systems.

Contains:

- Customer behavior
- Reviews
- Complaints
- Dialogue reactions

Subfolders:

- Karen/
- ChillGuy/
- Masochist/
- Redditor/
- RichCustomer/
- MysteryCustomer/

====================================================
/Features/Manager
====================================================

Manager systems.

Contains:

- Suspicion tracking
- Punishment system
- Cinematic triggers
- Kill animations

====================================================
/Features/Restaurant
====================================================

Restaurant gameplay systems.

Contains:

- Cooking
- Cleaning
- Orders
- Serving
- Tables
- Kitchen interactions

====================================================
/Features/Reviews
====================================================

Review/reputation systems.

Contains:

- Negative reviews
- Complaints
- Manager calls
- Positive reviews
- Promotion risk

====================================================
/Features/Dialogue
====================================================

Dialogue and subtitles.

Contains:

- NPC dialogue
- Tutorial dialogue
- Manager announcements

====================================================
/Features/UI
====================================================

Gameplay UI systems.

Contains:

- HUD
- Pause menu
- Results screen
- Main menu
- Settings menu

====================================================
/Features/AI
====================================================

NPC state machines and logic.

Contains:

- Pathfinding
- State systems
- Queue systems
- Customer routines

====================================================
LEVELS
====================================================

Contains all scenes.

---

## /Levels/MainMenu

Main menu scenes.

---

## /Levels/Day01_Monday

Tutorial/introduction day.

Higher chance of Karen customers.

---

## /Levels/Day02_Tuesday

Early gameplay escalation.

---

## /Levels/Day03_Wednesday

Mixed customer behavior begins.

---

## /Levels/Day04_Thursday

More manager suspicion.

---

## /Levels/Day05_Friday

Harder sabotage opportunities.

---

## /Levels/Day06_Saturday

High-pressure gameplay.

---

## /Levels/Day07_Sunday

Final day.

Triggers endings.

---

## /Levels/TestScenes

Prototype/testing scenes.

Contains:

- AI tests
- Physics tests
- UI tests
- Lighting tests

IMPORTANT:
Never place production scenes here.

====================================================
PREFABS
====================================================

Reusable prefabs. IF IT BELONGS TO A FEATURE, IT DOESN'T GO HERE.

## /Prefabs/Environment

Environment prefabs.

---

## /Prefabs/Interactables

Interactive objects.

Contains:

- Phone
- Oven
- Plates
- Tables
- Trash bins

---

## /Prefabs/UI

UI prefabs.

---

## /Prefabs/VFX

Visual effect prefabs.

====================================================
SCRIPTABLEOBJECTS
====================================================

Data-driven game configuration.

Contains:

- Customer profiles
- Review probabilities
- Dialogue data
- Audio references
- Difficulty settings

This is a DATA folder, not behaviour.

====================================================
UI
====================================================

User interface assets.

---

## /UI/HUD

Gameplay HUD.

Contains:

- Suspicion meter
- Review counters
- Timers

---

## /UI/Menus

Main menu and pause menu assets.

---

## /UI/Fonts

Retro/PSX fonts.

---

## /UI/Icons

Buttons/icons.

====================================================
ANIMATIONS
====================================================

Shared animation controllers. If it belongs to a feature, it doesn't go here.

Examples:

- Door animations
- Interaction animations

====================================================
CINEMATICS
====================================================

Cutscene assets.

Contains:

- Intro cinematic
- Endings
- Manager scenes

====================================================
DOCS
====================================================

Project documentation.

---

## /Docs/GDD

Game Design Document.

---

## /Docs/References

Visual references.

---

## /Docs/Meetings

Team meeting notes.

---

====================================================
BUILDS
====================================================

Compiled game versions.

Structure:

- Windows/
- Linux/
- WebGL/

====================================================
IMPORTANT DEVELOPMENT RULES
====================================================

1. Never place random files in root folders.

2. Use TestScenes for experiments.

3. Separate gameplay systems by feature.

4. Avoid duplicate prefabs/materials.

5. Maintain consistent naming.

6. Use Git from day one.
