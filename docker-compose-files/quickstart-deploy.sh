#!/bin/bash

COMPONENT=$1
SOURCE_DIR=$2
DEST_DIR=$3

echo "COMPONENT: $COMPONENT"
echo "SOURCE_DIR: $SOURCE_DIR"
echo "DEST_DIR: $DEST_DIR"

if [ $COMPONENT == "api" ]; then
    echo "Deploying API"
    mkdir -p "$DEST_DIR/app"
    cp -R "$SOURCE_DIR/app/common" "$DEST_DIR/app"
    cp -R "$SOURCE_DIR/app/api" "$DEST_DIR/app"

    find "$DEST_DIR" -name "*.dll" -delete

    rm -rf "$DEST_DIR/app/common/bin"
    rm -rf "$DEST_DIR/app/common/obj"
    rm -rf "$DEST_DIR/app/api/bin"
    rm -rf "$DEST_DIR/app/api/obj"

    dotnet build --no-incremental "$DEST_DIR/app/api"
    dotnet run -p "$DEST_DIR/app/api"
elif [ $COMPONENT == "runner" ]; then
    echo "Deploying Runner"
    mkdir -p "$DEST_DIR/app"
    cp -R "$SOURCE_DIR/app/common" "$DEST_DIR/app"
    cp -R "$SOURCE_DIR/app/runner" "$DEST_DIR/app"

    find "$DEST_DIR" -name "*.dll" -delete

    rm -rf "$DEST_DIR/app/common/bin"
    rm -rf "$DEST_DIR/app/common/obj"
    rm -rf "$DEST_DIR/app/runner/bin"
    rm -rf "$DEST_DIR/app/runner/obj"

    dotnet build --no-incremental "$DEST_DIR/app/runner"
    dotnet run -p "$DEST_DIR/app/runner"
elif [ $COMPONENT == "manager" ]; then
    echo "Deploying Manager"
    mkdir -p "$DEST_DIR/app"
    cp -R "$SOURCE_DIR/app/common" "$DEST_DIR/app"
    cp -R "$SOURCE_DIR/app/manager" "$DEST_DIR/app"

    find "$DEST_DIR" -name "*.dll" -delete

    rm -rf "$DEST_DIR/app/common/bin"
    rm -rf "$DEST_DIR/app/common/obj"
    rm -rf "$DEST_DIR/app/manager/bin"
    rm -rf "$DEST_DIR/app/manager/obj"

    dotnet build --no-incremental "$DEST_DIR/app/manager"
    dotnet run -p "$DEST_DIR/app/manager"
else
    echo "Deploy type not understood. Support types are: api, runner, manager"
    exit 1
fi