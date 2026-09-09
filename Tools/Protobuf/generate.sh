#!/bin/sh
set -e

cd "$(dirname "$0")"

./Protoc/bin/protoc.exe \
  --proto_path=../../Proto \
  --csharp_out=../../Assets/Scripts/ProtoGenerated \
  ../../Proto/*.proto

echo "Proto classes generated"