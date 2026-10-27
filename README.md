# EasyTransfer
EasyTransfer is an open-source and cross-platform .NET app that used to transfering files between two devices in the same network (LAN).

## Why
Sometimes I need to send a list of files from one device to another. I usually use netcat command to address this, but it lacks for one file at the time, if I want to send more one file, then I call the netcat repeatedly. Obviously, this is inefficient, so to address this I wrote an app called EasyTransfer.

## Supported Platforms
I said that this app is cross-platform, however the app is focused only what platforms do I usually use. Currently supports:

- Linux

## Building EasyTransfer
Before building, you need the following depedencies:

- .NET 10 SDK (install according to your platform)

The build script is currently only available for linux called ```build.sh```. To see the details what the build script can do, run ```build.sh -h```, ```build.sh --help```, or even just ```build.sh```.

### Linux
Run ```./build.sh build --output build``` to build and produce the executables for the current host, then it stores them in ```./build``` directory.

Note: If you want to compile for specified runtime, then just passed ```--runtime <runtime-id>``` to the build script.

## Run EasyTransfer
After building processes are done, then you need to ```cd``` into ```./build``` directory, where the outputs stored. After that, run ```EasyTransfer.Console``` (console) or ```EasyTransfer.GUI``` (GUI).

## Features
- Sending one or more files easily
- Store them all in a specified directory