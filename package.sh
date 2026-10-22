#!/bin/env bash

NAME=EasyTransfer
NET_FRAMEWORK=net10.0
SOLUTION_FILE=./EasyTransfer.sln
BUILD_DIR=./build

if ! [ -d "$BUILD_DIR" ]; then
    mkdir "$BUILD_DIR"
fi

if dotnet build "$SOLUTION_FILE" --nologo --output "$BUILD_DIR" --framework "$NET_FRAMEWORK"; then
    echo "[SUCCESS] building $NAME successfully"
else
    echo "[ERROR] building $NAME failure"
    exit 1
fi
