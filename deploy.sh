#!/usr/bin/env bash

#exit if any command fails
set -e

NUGET_API_KEY=$1
NUGET_VERSION_SUFFIX=$2

dotnet pack ./Nexogen.Libraries.Metrics -c Release --version-suffix $NUGET_VERSION_SUFFIX
dotnet pack ./Nexogen.Libraries.Metrics.Extensions -c Release --version-suffix $NUGET_VERSION_SUFFIX
dotnet pack ./Nexogen.Libraries.Metrics.Prometheus -c Release --version-suffix $NUGET_VERSION_SUFFIX
dotnet pack ./Nexogen.Libraries.Metrics.Prometheus.AspCore -c Release --version-suffix $NUGET_VERSION_SUFFIX
dotnet pack ./Nexogen.Libraries.Metrics.Prometheus.PushGateway -c Release --version-suffix $NUGET_VERSION_SUFFIX
dotnet nuget push **/*.nupkg -s https://nuget.org -k $NUGET_API_KEY