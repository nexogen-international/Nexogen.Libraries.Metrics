#!/usr/bin/env bash

#exit if any command fails
set -e

dotnet restore
dotnet build -c Release
cd ./Nexogen.Libraries.Metrics.UnitTests && dotnet test && cd ..