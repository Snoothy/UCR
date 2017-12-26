# Universal Control Remapper
[![GitHub release](https://img.shields.io/github/release/snoothy/ucr.svg)]() [![IOWrapper version](https://img.shields.io/badge/IOWrapper-v0.2.16-blue.svg)](https://github.com/evilC/IOWrapper) [![license](https://img.shields.io/github/license/snoothy/ucr.svg)]() [![Github All Releases](https://img.shields.io/github/downloads/snoothy/ucr/total.svg)]()

Universal Control Remapper is a complete rewrite of the original [UCR](https://github.com/evilC/UCR), created in collaboration with [evilC](https://github.com/evilC/), to overcome the limitations of AHK.

Universal Control Remapper is a Windows application which allows the end-user to remap any inputs from devices, such as keyboards, mice, joysticks, racing wheels, eyetrackers, etc. to virtual output devices. Remapping is achieved by transforming inputs through plugins to a desired output device.

## Table of Contents ##

- [Downloads](#downloads)
- [Documentation](#documentation)
- [Device support](#device-support)
- [License](#license)

## Downloads ##

The latest release of the Universal Control Remapper can be [downloaded from GitHub](https://github.com/snoothy/ucr/releases).

## Documentation ##

Documentation for Universal Control Remapper are hosted on GitHub at [https://github.com/snoothy/ucr/wiki](https://github.com/snoothy/ucr/wiki).

## Device support ##

UCR supports input and output devices through plugins using the [IOWrapper](https://github.com/evilC/IOWrapper) backend. UCR is released with standard plugins but can be extended with third party plugins to add additional device support.

### Supported input ###

- Xbox 360 controllers (XInput)
- DirectInput controllers, includes gamepads, racing wheels, HOTAS, etc.
- Keyboard (using [interception](https://github.com/oblitum/Interception))
- Mouse (using [interception](https://github.com/oblitum/Interception))
- Tobii Eye tracker

### Supported output ###

- Xbox 360 controller (XInput) (using [ViGEm](https://github.com/nefarius/ViGEm))
- Dualshock 4 controller (using [ViGEm](https://github.com/nefarius/ViGEm))
- DirectInput controller (using [vJoy](https://github.com/shauleiz/vJoy))
- Keyboard (using [interception](https://github.com/oblitum/Interception))
- Mouse (using [interception](https://github.com/oblitum/Interception))

## License ##

Universal Control Remapper is Open Source software and Universal Control Remapper is released under the [MIT license](https://github.com/Snoothy/UCR/blob/master/LICENSE). 
