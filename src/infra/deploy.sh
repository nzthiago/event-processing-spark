#! /bin/bash
set -euxo pipefail

# Change directory to where this script is
pushd "${0%/*}"

# Make sure resource group exists or return a reasonable error
rgLocation=$(az group list -o tsv --query "[?name=='$1'].location")
if [ -z "$rgLocation" ]; then
    echo "Make sure you are logged in and resource group "$1" exists"
    exit 1
else
    echo "deploying to RG: $1 - $rgLocation"
fi

az deployment group create -g $1 -f main.bicep

popd