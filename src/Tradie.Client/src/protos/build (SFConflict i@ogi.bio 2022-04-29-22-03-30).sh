#!/bin/bash
set -eux

export PROTO_BASE="../../../Tradie.Proto"

# Services
## Web
protoc -I="$PROTO_BASE" "$PROTO_BASE/Services/Web/Proto/LeagueService.proto" --js_out=import_style=commonjs:. --grpc-web_out=import_style=typescript,mode=grpcweb:.
protoc -I="$PROTO_BASE" "$PROTO_BASE/Services/Web/Proto/ModifierService.proto" --js_out=import_style=commonjs:. --grpc-web_out=import_style=typescript,mode=grpcweb:.
protoc -I="$PROTO_BASE" "$PROTO_BASE/Services/Web/Proto/CriteriaService.proto" --js_out=import_style=commonjs:. --grpc-web_out=import_style=typescript,mode=grpcweb:.
## Indexer
protoc -I="$PROTO_BASE" "$PROTO_BASE/Services/Indexer/Proto/SearchController.proto" --js_out=import_style=commonjs:. --grpc-web_out=import_style=typescript,mode=grpcweb:.

# Models
## Analyzer
protoc -I="$PROTO_BASE" "$PROTO_BASE/Models/Analyzer/Proto/League.proto" --js_out=import_style=commonjs:. --grpc-web_out=import_style=typescript,mode=grpcweb:.
protoc -I="$PROTO_BASE" "$PROTO_BASE/Models/Analyzer/Proto/Modifier.proto" --js_out=import_style=commonjs:. --grpc-web_out=import_style=typescript,mode=grpcweb:.
protoc -I="$PROTO_BASE" "$PROTO_BASE/Models/Analyzer/Proto/ModifierTypes.proto" --js_out=import_style=commonjs,binary:. --grpc-web_out=import_style=typescript,mode=grpcweb:.
## Indexer
protoc -I="$PROTO_BASE" "$PROTO_BASE/Models/Indexer/Proto/Criteria.proto" --js_out=import_style=commonjs:. --grpc-web_out=import_style=typescript,mode=grpcweb:.
