# Rift Protocol Testing and Bug Fix Report

## Purpose

This document records the testing work and defect-driven verification for **Rift Protocol**. The tests are based on the main gameplay flow, known issues found during development, and regression checks added after fixes.

The document covers:

- Core gameplay tests.
- UI and scene flow tests.
- Audio feedback tests.
- Hazard and respawn tests.
- Defects found during development.
- Regression checks for previously fixed issues.

## Test Environment

| Item | Details |
|---|---|
| Game Engine | Unity |
| Unity Version | 2022.3.62f3c1 |
| Platform | Windows / Unity Editor |
| Input | Keyboard and mouse |
| Aspect Ratio | 16:9 |
| Main Start Scene | `MainMenu` |

## Expected Scene Flow

| Order | Scene |
|---|---|
| 1 | `MainMenu` |
| 2 | `IntroScene` |
| 3 | `Level01` |
| 4 | `Level02` |
| 5 | `Level03` |
| 6 | `Level04` |
| 7 | `EndScene` |

## Development Issues Found and Fixed

### BUG-01 Level03 Could Not Load Level04

| Field | Details |
|---|---|
| Problem | Clicking the Level03 completion panel's Next button produced an error because `Level04` was not available in build settings. |
| Error Observed | `LevelCompletionUI cannot load scene 'Level04'. Add it to Scenes In Build or Build Profiles and verify the configured name.` |
| Cause | `Level04` was not included in the build scene list at the time of testing. |
| Fix | Added `Level04` to the build scene list and configured Level03 completion to point to `Level04`. |
| Regression Test | Complete Level03, click Next, confirm Level04 loads without console errors. |
| Result | Fixed by scene/build settings update. |

### BUG-02 Level03 WorldATrap and WorldBTrap Had No Collider

| Field | Details |
|---|---|
| Problem | `WorldATrap` and `WorldBTrap` in Level03 had the `Trap` tag but did not damage the player. |
| Cause | The trap tilemaps did not have collider components. |
| Fix | Added `TilemapCollider2D` components to both trap tilemaps and set them as triggers. |
| Regression Test | Touch `WorldATrap` and `WorldBTrap` in Level03 and confirm player death/respawn is triggered. |
| Result | Fixed by adding trigger colliders. |

### BUG-03 Level04 Hint Was Visible Too Early

| Field | Details |
|---|---|
| Problem | The Level04 hint was visible immediately when the level started instead of appearing after the player reached the intended position. |
| Requirement | The hint should appear when the player reaches `x = 33`. |
| Fix | Added position-based hint behavior using `WorldPositionHintUI`. |
| Regression Test | Start Level04, confirm the hint is hidden before `x = 33`, then move to `x >= 33` and confirm it appears. |
| Result | Fixed by world-position trigger logic. |

### BUG-04 Level04 Red Panel Object Deactivation Was Incorrect

| Field | Details |
|---|---|
| Problem | Level04 red panel had an unwanted object in `objectsToDeactivate[0]`. |
| Requirement | Remove that deactivation behavior for Level04 only. |
| Fix | Cleared the Level04 red panel `objectsToDeactivate` entry without applying the change to other levels. |
| Regression Test | Use the Level04 red panel and confirm only the intended Level04 behavior occurs. |
| Result | Fixed in Level04 scene configuration. |

### BUG-05 Level04 Needed Alarm Then Powerdown Audio

| Field | Details |
|---|---|
| Problem | Level04 final panel needed a two-step audio sequence. |
| Requirement | Play `alarm`, then immediately play `powerdown1` after the alarm finishes. |
| Fix | Extended `RedPanelInteraction` to support an optional follow-up sound sequence. |
| Regression Test | Use the Level04 red panel and confirm `alarm` plays first, followed by `powerdown1`. |
| Result | Fixed with panel audio sequence support. |

### BUG-06 Level04 Needed Black Fade During Powerdown

| Field | Details |
|---|---|
| Problem | The screen needed to fade to black during the final powerdown sequence. |
| Requirement | `BlackFade` opacity should increase while `powerdown1` plays, reaching full black when the sound ends. |
| Fix | Added optional fade image support to the Level04 red panel sequence. |
| Regression Test | Use the Level04 red panel and confirm the screen gradually fades to black during `powerdown1`. |
| Result | Fixed by linking `BlackFade` to the panel sequence. |

### BUG-07 Level04 Needed to Load EndScene After Shutdown

| Field | Details |
|---|---|
| Problem | After the final shutdown sequence, the game needed to transition to the ending scene. |
| Requirement | Load `EndScene` after the screen becomes fully black. |
| Fix | Added optional scene loading after the panel sound/fade sequence. |
| Regression Test | Use the Level04 red panel, wait for fade completion, and confirm `EndScene` loads. |
| Result | Fixed by configuring Level04 red panel to load `EndScene`. |

### BUG-08 World Switch Needed to Be Disabled in Level04

| Field | Details |
|---|---|
| Problem | World switching should not be available in Level04. |
| Fix | Disabled the WorldSwitcher in Level04 only. |
| Regression Test | Press `Space` in Level04 and confirm no world switching occurs. |
| Result | Fixed in Level04 scene configuration. |

### BUG-09 Intro Text Needed Inspector Font Size Control

| Field | Details |
|---|---|
| Problem | Intro text size needed to be adjustable directly in the Unity Inspector. |
| Fix | Added a serialized `fontSize` field to `IntroSequenceController`. |
| Regression Test | Change the font size in the Inspector and confirm StoryText updates. |
| Result | Fixed by exposing font size control. |

### BUG-10 Intro Messages Needed One Sentence Per Screen

| Field | Details |
|---|---|
| Problem | Some intro screens showed too much text at once. |
| Requirement | Each intro screen should show one story beat at a time. |
| Fix | Split the intro message array into separate message entries. |
| Regression Test | Play the intro and confirm each message appears separately. |
| Result | Fixed through IntroScene message configuration. |

### BUG-11 Intro and Level04 Needed Background Music

| Field | Details |
|---|---|
| Problem | IntroScene and Level04 needed background music. |
| Requirement | Play `bgm1` in IntroScene and Level04 at matching volume. |
| Fix | Added `bgm1` AudioSource setup to both scenes. |
| Regression Test | Load IntroScene and Level04 and confirm background music plays at a balanced volume. |
| Result | Fixed in scene audio setup. |

### BUG-12 Button Click Audio Needed UI Coverage

| Field | Details |
|---|---|
| Problem | UI buttons needed click sound feedback. |
| Fix | Added button click audio references to Main Menu, Pause UI, and Level Completion UI. |
| Regression Test | Click Start, Quit, Pause, Resume, Restart, Main Menu, and Next buttons; confirm click audio plays. |
| Result | Fixed through UI audio setup. |

### BUG-13 Level02 WorldATrap and WorldBTrap Did Not Kill Player

| Field | Details |
|---|---|
| Problem | `WorldATrap` and `WorldBTrap` in Level02 had the `Trap` tag, but touching them did not kill the player. |
| Cause | The player damage logic only handled trigger contacts, while the Level02 trap tilemaps did not have correctly configured trigger colliders. |
| Fix | Updated player hazard detection to support both trigger and collision contacts, and added trigger colliders to the Level02 trap tilemaps. |
| Regression Test | Touch both `WorldATrap` and `WorldBTrap` in Level02 and confirm player death/respawn is triggered. |
| Result | Fixed by collision handling and trap collider updates. |

### BUG-14 Level02 Barrier1 and Barrier2 Had No Lethal Collider

| Field | Details |
|---|---|
| Problem | `Barrier1` and `Barrier2` appeared dangerous, but the player could touch them without dying. |
| Cause | The barrier objects had the `Trap` tag but did not have lethal trigger colliders. |
| Fix | Added trigger `BoxCollider2D` components to the barrier objects. |
| Regression Test | Touch `Barrier1` and `Barrier2` in Level02 and confirm death/respawn occurs. |
| Result | Fixed by adding lethal barrier trigger colliders. |

### BUG-15 Laser Visual Did Not Match Collision Endpoint

| Field | Details |
|---|---|
| Problem | The laser image had transparent padding, creating a visible gap between the beam end and the actual collision endpoint. |
| Cause | The laser visual was scaled using the full sprite bounds instead of the visible beam area. |
| Fix | Adjusted the laser visual length and center offset using the sprite padding so the visible beam aligns with the collision endpoint. |
| Regression Test | Block the laser with a wall or moving tilemap and confirm the visible beam ends at the blocking object. |
| Result | Fixed by compensating for sprite padding in the laser visual. |

### BUG-16 Laser Sprite Was Tinted by Beam Color

| Field | Details |
|---|---|
| Problem | The laser sprite color could be changed by a separate beam color field, making it look different from the source PNG. |
| Cause | The laser visual exposed an extra color tint that was applied over the sprite. |
| Fix | Removed the exposed beam tint and kept the sprite renderer color white so the PNG displays with its original colors. |
| Regression Test | Run the laser scene and confirm the beam matches the original `laser.png` color. |
| Result | Fixed by preserving the sprite's original appearance. |

### BUG-17 Used RedPanel Could Reappear After World Switching

| Field | Details |
|---|---|
| Problem | A used red panel could be re-enabled if world switching activated its parent world again. |
| Cause | The panel did not preserve its used state when it became active again. |
| Fix | Added persistent used-state handling so a consumed panel stays disabled after world switching. |
| Regression Test | Use the RedPanel, switch worlds, and confirm the panel and its deactivated target do not reappear. |
| Result | Fixed by preserving panel state after interaction. |

## Core Gameplay Test Cases

### TC-01 Player Movement

| Field | Details |
|---|---|
| Objective | Confirm that the player can move left and right. |
| Steps | Press `A`, then press `D`. |
| Expected Result | Player moves left and right smoothly. |
| Result | Passed during gameplay iteration. |

### TC-02 Player Jump

| Field | Details |
|---|---|
| Objective | Confirm that the player can jump and receives sound feedback. |
| Steps | Press `W` while grounded. |
| Expected Result | Player jumps and jump sound plays. |
| Result | Passed during gameplay iteration. |

### TC-03 Player Respawn

| Field | Details |
|---|---|
| Objective | Confirm that player death triggers respawn. |
| Steps | Touch a trap or hazard. |
| Expected Result | Player death animation plays, then the player respawns and respawn sound plays. |
| Result | Passed during gameplay iteration. |

### TC-04 Wall Jump

| Field | Details |
|---|---|
| Objective | Confirm wall jump behavior. |
| Steps | Move against a wall and press jump while giving directional input. |
| Expected Result | Player jumps away from the wall and briefly locks horizontal control. |
| Result | Passed during gameplay iteration. |

### TC-05 World Switching

| Field | Details |
|---|---|
| Objective | Confirm that world switching toggles WorldA and WorldB objects. |
| Steps | Press `Space` in a level where world switching is enabled. |
| Expected Result | WorldA/WorldB objects swap active states and switch audio plays. |
| Result | Passed during gameplay iteration. |

### TC-06 Portal Level Completion

| Field | Details |
|---|---|
| Objective | Confirm that entering a portal opens the completion UI. |
| Steps | Move the player into the level portal. |
| Expected Result | Level completion panel appears and gameplay pauses. |
| Result | Passed during gameplay iteration. |

### TC-07 Pause Menu

| Field | Details |
|---|---|
| Objective | Confirm pause menu behavior. |
| Steps | Press `Esc`, then Resume. |
| Expected Result | Pause menu opens, gameplay pauses, then resumes correctly. |
| Result | Passed during gameplay iteration. |

### TC-08 Restart Level

| Field | Details |
|---|---|
| Objective | Confirm that Restart reloads the current scene. |
| Steps | Open Pause Menu or Completion UI and click Restart. |
| Expected Result | Current level reloads and time scale returns to normal. |
| Result | Passed during gameplay iteration. |

### TC-09 Return to Main Menu

| Field | Details |
|---|---|
| Objective | Confirm that Main Menu buttons return to MainMenu. |
| Steps | Click Main Menu from Pause Menu or Completion UI. |
| Expected Result | `MainMenu` loads and time scale returns to normal. |
| Result | Passed during gameplay iteration. |

## Scene Flow Test Cases

### TC-10 MainMenu to IntroScene

| Field | Details |
|---|---|
| Objective | Confirm Start Game loads the intro instead of directly loading Level01. |
| Steps | Open MainMenu and click Start Game. |
| Expected Result | `IntroScene` loads. |
| Result | Passed after Main Menu scene loading update. |

### TC-11 IntroScene to Level01

| Field | Details |
|---|---|
| Objective | Confirm intro automatically enters gameplay. |
| Steps | Wait for all intro messages to finish. |
| Expected Result | `Level01` loads. |
| Result | Passed after IntroScene setup. |

### TC-12 Level03 to Level04

| Field | Details |
|---|---|
| Objective | Confirm Level03 completion points to Level04. |
| Steps | Complete Level03 and click Next. |
| Expected Result | `Level04` loads without scene loading error. |
| Result | Passed after build settings and completion UI configuration fix. |

### TC-13 Level04 to EndScene

| Field | Details |
|---|---|
| Objective | Confirm final panel sequence transitions to EndScene. |
| Steps | Use the Level04 red panel and wait for the shutdown sequence. |
| Expected Result | `EndScene` loads after the fade completes. |
| Result | Passed after Level04 panel sequence setup. |

## Hazard Test Cases

### TC-14 Laser Hazard

| Field | Details |
|---|---|
| Objective | Confirm laser hazards kill the player. |
| Steps | Move into an active laser. |
| Expected Result | Player dies and respawns. |
| Result | Passed during hazard testing. |

### TC-15 Saw Hazard

| Field | Details |
|---|---|
| Objective | Confirm saw traps kill the player. |
| Steps | Touch a saw trap. |
| Expected Result | Player dies and respawns. |
| Result | Passed during hazard testing. |

### TC-16 Timed Barrier

| Field | Details |
|---|---|
| Objective | Confirm timed barrier collision changes with animation state. |
| Steps | Try to pass through the barrier during closed and open states. |
| Expected Result | Barrier blocks the player while active and becomes passable during the open phase. |
| Result | Passed during hazard testing. |

### TC-17 Level03 World Traps

| Field | Details |
|---|---|
| Objective | Confirm Level03 world-specific trap tilemaps damage the player. |
| Steps | Touch both `WorldATrap` and `WorldBTrap`. |
| Expected Result | Both trigger player death and respawn. |
| Result | Passed after collider fix. |

## Audio Test Cases

### TC-18 Player Audio

| Field | Details |
|---|---|
| Objective | Confirm player audio feedback. |
| Steps | Jump, die, respawn, and world switch. |
| Expected Result | Jump, respawn, and world switch sound effects play at the correct time. |
| Result | Passed during audio feedback testing. |

### TC-19 Hazard Audio

| Field | Details |
|---|---|
| Objective | Confirm hazard audio feedback. |
| Steps | Observe active lasers, saws, and timed barriers. |
| Expected Result | Hazard audio plays when hazards are active and is balanced in volume. |
| Result | Passed during audio feedback testing. |

### TC-20 UI Button Audio

| Field | Details |
|---|---|
| Objective | Confirm UI buttons provide click sound feedback. |
| Steps | Click menu, pause, and completion buttons. |
| Expected Result | Button click sound plays before scene actions. |
| Result | Passed after UI audio setup. |

### TC-21 Background Music

| Field | Details |
|---|---|
| Objective | Confirm background music plays in cinematic/final scenes. |
| Steps | Load IntroScene, Level04, and EndScene. |
| Expected Result | `bgm1` plays in each scene at a consistent volume. |
| Result | Passed after scene audio setup. |

## UI Test Cases

### TC-22 Intro Text Display

| Field | Details |
|---|---|
| Objective | Confirm intro text is readable and appears one message at a time. |
| Steps | Play IntroScene from start to finish. |
| Expected Result | Text fades in/out cleanly and only one message appears per screen. |
| Result | Passed after intro message split. |

### TC-23 Intro Skip

| Field | Details |
|---|---|
| Objective | Confirm skip input only skips the current message delay. |
| Steps | Press `Space` or `Enter` during a visible intro message. |
| Expected Result | The current message fades out normally; the entire intro is not skipped instantly. |
| Result | Passed after intro controller setup. |

### TC-24 Level04 Hint

| Field | Details |
|---|---|
| Objective | Confirm the Level04 hint appears at the correct time. |
| Steps | Move toward `x = 33` in Level04. |
| Expected Result | Hint stays hidden before `x = 33` and appears after reaching the trigger point. |
| Result | Passed after hint trigger setup. |

### TC-25 Level Completion UI

| Field | Details |
|---|---|
| Objective | Confirm completion screen buttons work. |
| Steps | Complete a level and use Next, Restart, and Main Menu buttons. |
| Expected Result | Buttons load the correct scenes and play click audio. |
| Result | Passed during UI testing. |

## Level02 Interaction and Hazard Regression Tests

### TC-26 Level02 RedPanel Interaction

| Field | Details |
|---|---|
| Objective | Confirm that the Level02 RedPanel disables its linked obstacle. |
| Steps | Stand in the RedPanel interaction area and press `E`. |
| Expected Result | RedPanel is consumed, `Barrier2` is disabled, and the player can pass through the previously blocked route. |
| Result | Passed after RedPanel interaction fix. |

### TC-27 Laser Blocked by Moving Tilemap

| Field | Details |
|---|---|
| Objective | Confirm the moving tilemap can block the continuous laser. |
| Steps | Wait for the moving tilemap to enter the laser path, then move through the safe gap. |
| Expected Result | The laser stops at the moving tilemap while blocked, and the player can pass safely only while the beam is blocked. |
| Result | Passed after laser collision and moving tilemap setup. |

### TC-28 Laser Visual Uses Original PNG

| Field | Details |
|---|---|
| Objective | Confirm the laser visual matches the source sprite and its collision endpoint. |
| Steps | Observe an active laser while it is unobstructed and while blocked by a collider. |
| Expected Result | The beam uses the `laser.png` appearance, keeps its original color, and visually ends at the blocking object. |
| Result | Passed after laser sprite visual fixes. |

### TC-29 Portal Prefab Reuse

| Field | Details |
|---|---|
| Objective | Confirm the portal prefab can be reused outside its original Level01 placement. |
| Steps | Place `Portal.prefab` in a test scene or another level and move the player into it. |
| Expected Result | Portal animation plays, the trigger detects the player, and level completion behavior runs correctly. |
| Result | Passed after prefab creation. |

## Regression Checklist

| Check | Expected Result | Status |
|---|---|---|
| MainMenu Start Game loads IntroScene | IntroScene opens | Passed |
| IntroScene finishes into Level01 | Level01 loads | Passed |
| Level03 completion loads Level04 | Level04 loads without error | Passed |
| Level04 red panel loads EndScene | EndScene loads after fade | Passed |
| Level04 world switch disabled | Space does not switch worlds | Passed |
| Level04 hint appears at x = 33 | Hint appears once at threshold | Passed |
| Level03 WorldATrap has collider | Trap damages player | Passed |
| Level03 WorldBTrap has collider | Trap damages player | Passed |
| Level02 WorldATrap damages player | Player dies and respawns | Passed |
| Level02 WorldBTrap damages player | Player dies and respawns | Passed |
| Level02 Barrier1 damages player | Player dies and respawns | Passed |
| Level02 Barrier2 damages player before panel use | Player dies and respawns | Passed |
| Level02 RedPanel disables Barrier2 | Barrier2 is inactive after pressing E | Passed |
| Used RedPanel stays disabled after world switching | Panel does not reappear | Passed |
| Moving tilemap blocks laser beam | Beam stops at tilemap and player can pass during block | Passed |
| Laser sprite keeps original color | Sprite is not tinted by extra beam color | Passed |
| Laser visual endpoint matches collision endpoint | No visible gap at blocked endpoint | Passed |
| Portal prefab can be reused | Portal trigger and completion behavior work | Passed |
| UI buttons play click audio | Click audio plays | Passed |
| Player jump/respawn/world switch audio | Audio plays at correct moments | Passed |
| Background music in Intro/Level04/EndScene | BGM plays consistently | Passed |

## Notes

- Testing was primarily performed through Unity Editor playtesting during development.
- Several test cases were added directly from bugs found while building and integrating the scenes.
- Automated unit tests were not included because this project relies heavily on Unity scene setup, physics triggers, animations, and manual gameplay flow.