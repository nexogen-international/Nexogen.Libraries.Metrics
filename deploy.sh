#!/usr/bin/env bash

#exit if any command fails
set -e

if [ -z "$TRAVIS_TAG" ]
then
	echo "Should be run for a git tag"
	exit 1
else
	BUILD="dotnet msbuild /t:Restore;Pack /p:Configuration=Release /p:PackageVersionNumber=${TRAVIS_TAG#v}"
fi

$BUILD ./Nexogen.Libraries.Metrics
$BUILD ./Nexogen.Libraries.Metrics.Extensions
$BUILD ./Nexogen.Libraries.Metrics.Prometheus
$BUILD ./Nexogen.Libraries.Metrics.Prometheus.AspCore
$BUILD ./Nexogen.Libraries.Metrics.Prometheus.PushGateway
$BUILD ./Nexogen.Libraries.Metrics.Prometheus.Standalone

dotnet nuget push **/*.nupkg -s https://nuget.org -k $NUGET_API_KEY
