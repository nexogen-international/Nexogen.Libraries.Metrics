#!/usr/bin/env bash

#exit if any command fails
set -e

if [ -z "$TRAVIS_TAG" ]
then
	$ARGS="-c Release --version-suffix ${TRAVIS_BUILD_NUMBER}"
else
	$ARGS="-c Release /p:PackageVersion=${TRAVIS_TAG}"
fi

dotnet pack ./Nexogen.Libraries.Metrics $ARGS
dotnet pack ./Nexogen.Libraries.Metrics.Extensions $ARGS
dotnet pack ./Nexogen.Libraries.Metrics.Prometheus $ARGS
dotnet pack ./Nexogen.Libraries.Metrics.Prometheus.AspCore $ARGS
dotnet pack ./Nexogen.Libraries.Metrics.Prometheus.PushGateway $ARGS

dotnet nuget push **/*.nupkg -s https://nuget.org -k $NUGET_API_KEY
