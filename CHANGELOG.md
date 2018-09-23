# Changelog
All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/), and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [Unreleased]
### Added
- `Buttons to Axis` plugin
- Invert output to `Button to Axis`
- `Button to Buttons (Long Press)`
- Reset button for `Delta To Axis` relative mode
- DLL blocking detecting and unblocking
- Appveyor build server setup

### Fixed
- Allow `+` and `-` to be typed into plugin decimal inputs 
- nuke restore task
- nuke versioning task

### Removed

- Cake build script

## [0.4.0] - 2018-09-03
### Added
- Inputs and outputs now support live preview when a profile is active
- Added circular dead zone feature
- `Button to Axis` has absolute value to support Xbox triggers
- `Axes to Axes`: Maps a stick to another stick, with optional circular dead zone which acts on both axes
- `Axis to Axis (Cumulative)`: Cumulatively move the output axis with the value of the input axis
- `Axis to Delta`: Joystick type axis to Mouse type axis
- `Delta to Axis`: Mouse type axis to Joystick type axis
- IOWrapper: Complete Delta Axis implementation for mouse support
- IOWrapper: Interception devices now show `K:` or `M:` prefix to aid in identification
- IOWrapper: Interception support for horizontal wheel

### Changed
- Updated to IOWrapper v0.5.7

### Fixed
- Fixed wrapping errors for invert, dead zone and sensitivity settings
- IOWrapper: Interception provider, fixes to device enumeration - should now find PS/2 and built-in devices
- IOWrapper: Interception mouse wheel fix
- IOWrapper: ViGEm DS4 DPad fixed
- IOWrapper: ViGEm Xbox triggers fixed