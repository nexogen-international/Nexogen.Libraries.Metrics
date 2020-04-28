#!/usr/bin/env bash

#exit if any command fails
set -e

dotnet restore
dotnet build -c Release
cd ./Nexogen.Libraries.Metrics.UnitTests && dotnet test && cd ..
cd ./Nexogen.Libraries.Metrics.Grpc.UnitTests && dotnet test && cd ..
cd ./Nexogen.Libraries.Metrics.Prometheus.AspCore.UnitTests && dotnet test && cd ..
