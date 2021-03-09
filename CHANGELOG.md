# Changelog
All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/)

## [1.0.5] - 2021-03-09


## [1.0.4] - 2021-03-07
### Added
- 6 new monsters
- NPC battlers
- LoS logic
- Default direction/animation for NPCs

### Changed
- Game name
- Main character sprite and name
- Monster sprites
- Tileset
- Game font
- Battle background images
- UI redesign
- Rename Art/Player to Art/Character

### Fixed
- Broken object colliders from sprite size change
- Exception when interacting with NPC without dialog
- NPCs quit moving after interaction
- Roar having 0 accuracy
- No longer allow player to exit party screen with downed monster
- Battlers now have walk animation on LoS encounters
- Multiple trainer battle bugs


## [1.0.3] - 2021-03-05
### Added
- Custom animation system
- NPCs with walking patterns
- World dialog system
- Texture packer importer plugin
- Changelog

### Changed
- Refactor player controller, move common behavior to character superclass
- Transparency sort mode to Y axis
- Map layer names to be more descriptive
- Update Unity to 2020.2.7f1

### Fixed
- Bug where monster can use moves with 0 energy
- Ambiguous dialog text when monster attacking
- Duplicate status move having no feedback dialog
- Multiple bugs with NPC behavior

## [1.0.2] - 2021-03-01
### Added
- Status change messages
- Speed stat check for deciding who attacks first
- Status condition moves
- Secondary move effects
- Priority moves
- Accuracy and hit chance calculations

### Changed
- Increase wild encounters chance
- Refactor battle system to improve architecture
- Refactor status effects to separate the logic from normal moves
- Refactor code to clean up and organize

### Fixed
- Bug where enemy could attack out of turn after a downed monster is replaced

## [1.0.1] - 2021-02-28
### Added
- Battle system
- Party selection
- X control to back out of screens
- README.md
- LICENSE
- CONTRIBUTING.md

### Changed
- Refactor action/move/party selection logic
- Start following [SemVer](https://semver.org) properly.

### Fixed
- Fix rapid attacks bug
- Fix index OOR bug on move selection

## [1.0.0] - 2021-02-25
### Added
- Player walking animation
- Collision detection
- Random monster encounters
- Placeholder monsters
- Placeholder monster moves
- Placeholder game art
