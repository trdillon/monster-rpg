# Changelog
All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/)

## [1.0.3] - 2021-03-05
### Added
- Changelog

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
