#!/usr/bin/env bash

set -euo pipefail

export DOTNET_CLI_TELEMETRY_OPTOUT="${DOTNET_CLI_TELEMETRY_OPTOUT:-1}"
export DOTNET_INSTALL_DIR="${HOME}/.dotnet"
export PATH="${DOTNET_INSTALL_DIR}:${PATH}"

echo "Installing .NET SDK 9 for Netlify build..."
curl -fsSL https://dot.net/v1/dotnet-install.sh -o /tmp/dotnet-install.sh
bash /tmp/dotnet-install.sh --channel 9.0 --install-dir "${DOTNET_INSTALL_DIR}"

echo "Using SDK:"
dotnet --info

echo "Publishing Blazor WebAssembly app..."
dotnet publish ./pRHosaApp1.csproj -c Release
