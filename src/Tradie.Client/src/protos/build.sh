#!/bin/bash
set -eux

export PROTO_BASE="../../../Tradie.Proto"

# Services
## Web
protoc -I="$PROTO_BASE" "$PROTO_BASE/Services/Web/Proto/LeagueService.proto" --js_out=import_style=commonjs:. --grpc-web_out=import_style=typescript,mode=grpcweb:.
protoc -I="$PROTO_BASE" "$PROTO_BASE/Services/Web/Proto/ModifierService.proto" --js_out=import_style=commonjs:. --grpc-web_out=import_style=typescript,mode=grpcweb:.
protoc -I="$PROTO_BASE" "$PROTO_BASE/Services/Web/Proto/AffixRangeService.proto" --js_out=import_style=commonjs:. --grpc-web_out=import_style=typescript,mode=grpcweb:.
protoc -I="$PROTO_BASE" "$PROTO_BASE/Services/Web/Proto/CriteriaService.proto" --js_out=import_style=commonjs:. --grpc-web_out=import_style=typescript,mode=grpcweb:.
protoc -I="$PROTO_BASE" "$PROTO_BASE/Services/Web/Proto/SearchService.proto" --js_out=import_style=commonjs:. --grpc-web_out=import_style=typescript,mode=grpcweb:.
protoc -I="$PROTO_BASE" "$PROTO_BASE/Services/Web/Proto/ItemTypeService.proto" --js_out=import_style=commonjs:. --grpc-web_out=import_style=typescript,mode=grpcweb:.
## Indexer
# protoc -I="$PROTO_BASE" "$PROTO_BASE/Services/Indexer/Proto/SearchController.proto" --js_out=import_style=commonjs:. --grpc-web_out=import_style=typescript,mode=grpcweb:.

# Models
## Analyzer
protoc -I="$PROTO_BASE" "$PROTO_BASE/Models/Analyzer/Proto/Item.proto" --js_out=import_style=commonjs:. --grpc-web_out=import_style=typescript,mode=grpcweb:.
protoc -I="$PROTO_BASE" "$PROTO_BASE/Models/Analyzer/Proto/ItemType.proto" --js_out=import_style=commonjs:. --grpc-web_out=import_style=typescript,mode=grpcweb:.
protoc -I="$PROTO_BASE" "$PROTO_BASE/Models/Analyzer/Proto/Requirements.proto" --js_out=import_style=commonjs:. --grpc-web_out=import_style=typescript,mode=grpcweb:.
protoc -I="$PROTO_BASE" "$PROTO_BASE/Models/Analyzer/Proto/ItemAnalysis.proto" --js_out=import_style=commonjs:. --grpc-web_out=import_style=typescript,mode=grpcweb:.
protoc -I="$PROTO_BASE" "$PROTO_BASE/Models/Analyzer/Proto/Properties/ItemAffixProperties.proto" --js_out=import_style=commonjs:. --grpc-web_out=import_style=typescript,mode=grpcweb:.
protoc -I="$PROTO_BASE" "$PROTO_BASE/Models/Analyzer/Proto/Properties/ItemDetailProperties.proto" --js_out=import_style=commonjs:. --grpc-web_out=import_style=typescript,mode=grpcweb:.
protoc -I="$PROTO_BASE" "$PROTO_BASE/Models/Analyzer/Proto/Properties/ItemListingProperties.proto" --js_out=import_style=commonjs:. --grpc-web_out=import_style=typescript,mode=grpcweb:.
protoc -I="$PROTO_BASE" "$PROTO_BASE/Models/Analyzer/Proto/Properties/ItemTypeProperties.proto" --js_out=import_style=commonjs:. --grpc-web_out=import_style=typescript,mode=grpcweb:.
protoc -I="$PROTO_BASE" "$PROTO_BASE/Models/Analyzer/Proto/League.proto" --js_out=import_style=commonjs:. --grpc-web_out=import_style=typescript,mode=grpcweb:.
protoc -I="$PROTO_BASE" "$PROTO_BASE/Models/Analyzer/Proto/Modifier.proto" --js_out=import_style=commonjs:. --grpc-web_out=import_style=typescript,mode=grpcweb:.
protoc -I="$PROTO_BASE" "$PROTO_BASE/Models/Analyzer/Proto/ModifierTypes.proto" --js_out=import_style=commonjs:. --grpc-web_out=import_style=typescript,mode=grpcweb:.
protoc -I="$PROTO_BASE" "$PROTO_BASE/Models/Analyzer/Proto/AffixRange.proto" --js_out=import_style=commonjs:. --grpc-web_out=import_style=typescript,mode=grpcweb:.
## Indexer
protoc -I="$PROTO_BASE" "$PROTO_BASE/Models/Indexer/Proto/Criteria.proto" --js_out=import_style=commonjs:. --grpc-web_out=import_style=typescript,mode=grpcweb:.
protoc -I="$PROTO_BASE" "$PROTO_BASE/Models/Indexer/Proto/SearchQuery.proto" --js_out=import_style=commonjs:. --grpc-web_out=import_style=typescript,mode=grpcweb:.
