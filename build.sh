#!/bin/env bash

LIB=./scripts/Bash/lib.sh
if [ -f "$LIB" ]; then
    source "$LIB"
else
    echo "ERROR: Where is the $LIB?"
    exit 1
fi

declare -g DOTNET_FRAMEWORK_VER=net10.0
declare -g DOTNET_SLN=./EasyTransfer.sln

declare -g DOTNET_MODE=
declare -g DOTNET_OPTIONS=( -p:DebugType=none -p:ReadyToRun=false -p:PublishTrimmed=false --nologo )

print_help() {
    echo "$0 [--help|-h] build|publish [options...]"
    echo
    echo "Options:"
    echo "--help|-h             Print the help message"
    echo "build|publish         Specify the mode"
    echo "...                   It means you can pass any arguments that dotnet build/publish supports such as"
    echo "                      -p:StartupObject=EasyTransfer.Console, --self-contained, --runtime=*, etc"
}

if [ $# -lt 1 ]; then
    print_help
    exit
fi

while [ $# -gt 0 ]; do
    case "$1" in
        --help|-h)
            print_help
            exit
        ;;
        build|publish)
            DOTNET_MODE="$1"
            shift
        ;;
        --property:*|-p:*)
            DOTNET_OPTIONS+=( "$1" )
            shift
        ;;
        --*|-*)
            if [[ "$2" != "-"* ]] && [[ "$2" != "--"* ]]; then
                DOTNET_OPTIONS+=( "$1" "$2" )
                shift; shift
            else
                DOTNET_OPTIONS+=( "$1" )
                shift
            fi
        ;;
        *)
            shift
        ;;
    esac
done

declare -i i=
for ((i = 0; i < ${#DOTNET_OPTIONS[@]}; i++)); do
    option="${DOTNET_OPTIONS[$i]}"

    if [[ "$option" == "--"* ]] && [[ "$option" != *":"* ]] && [[ "${DOTNET_OPTIONS[$i+1]}" != "--"* ]]; then
        value="${DOTNET_OPTIONS[$i+1]}"

        if [ "$option" == "--output" ]; then
            if [ -d "$value" ]; then
                rm -fr "$value"/*
            else
                mkdir "$value"
            fi
        fi

        i+=1
    fi
done

if [ "$DOTNET_MODE" == build ]; then
    echo Default Opts: ${DOTNET_OPTIONS[@]}

    dotnet build "$DOTNET_SLN" --framework "$DOTNET_FRAMEWORK_VER" ${DOTNET_OPTIONS[@]}
elif [ "$DOTNET_MODE" == publish ]; then
    echo Default Opts: ${DOTNET_OPTIONS[@]}
    
    dotnet publish "$DOTNET_SLN" --framework "$DOTNET_FRAMEWORK_VER" ${DOTNET_OPTIONS[@]}
else
    echo "ERROR: The specified mode can be either build or publish"
    exit 1
fi