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

### Linux
Run ```./package.sh``` to build and produce the binary executables, then it stores them in ```./build``` directory.

Note: For now, I use this to build the app for linux, perhaps it will change in the future.

## Run EasyTransfer
After building processes are done, then you need to ```cd``` into ```./build``` directory, where the executables stored. After that, run ```./EasyTransfer``` (for linux).

## Features
- Sending one or more files easily
- Store them all in a specified directory
