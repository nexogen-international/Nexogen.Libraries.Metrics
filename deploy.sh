#!/usr/bin/env bash

#exit if any command fails
set -e

if [ -z "$TRAVIS_TAG" ]
then
	ARGS="-c Release /p:VersionSuffix=${TRAVIS_BUILD_NUMBER}"
else
	ARGS="-c Release /p:PackageVersionNumber=${TRAVIS_TAG#v}"
fi

dotnet msbuild /t:Restore;Pack ./Nexogen.Libraries.Metrics $ARGS
dotnet msbuild /t:Restore;Pack ./Nexogen.Libraries.Metrics.Extensions $ARGS
dotnet msbuild /t:Restore;Pack ./Nexogen.Libraries.Metrics.Prometheus $ARGS
dotnet msbuild /t:Restore;Pack ./Nexogen.Libraries.Metrics.Prometheus.AspCore $ARGS
dotnet msbuild /t:Restore;Pack ./Nexogen.Libraries.Metrics.Prometheus.PushGateway $ARGS

dotnet nuget push **/*.nupkg -s https://nuget.org -k $NUGET_API_KEY
