#!/bin/sh
set -e

cd "$(dirname "$0")"

curl -L \
  https://github.com/protocolbuffers/protobuf/releases/download/v36.2/protoc-36.2-win64.zip \
  -o protoc.zip

rm -rf Protoc
unzip -q protoc.zip -d Protoc
rm protoc.zip

echo "protoc 36.2 installed"