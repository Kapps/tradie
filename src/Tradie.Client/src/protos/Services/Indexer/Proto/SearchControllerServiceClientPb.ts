/**
 * @fileoverview gRPC-Web generated client stub for 
 * @enhanceable
 * @public
 */

// GENERATED CODE -- DO NOT EDIT!


/* eslint-disable */
// @ts-nocheck


import * as grpcWeb from 'grpc-web';

import * as Services_Indexer_Proto_SearchController_pb from '../../../Services/Indexer/Proto/SearchController_pb';


export class SearchServiceClient {
  client_: grpcWeb.AbstractClientBase;
  hostname_: string;
  credentials_: null | { [index: string]: string; };
  options_: null | { [index: string]: any; };

  constructor (hostname: string,
               credentials?: null | { [index: string]: string; },
               options?: null | { [index: string]: any; }) {
    if (!options) options = {};
    if (!credentials) credentials = {};
    options['format'] = 'binary';

    this.client_ = new grpcWeb.GrpcWebClientBase(options);
    this.hostname_ = hostname;
    this.credentials_ = credentials;
    this.options_ = options;
  }

  methodDescriptorSearchGear = new grpcWeb.MethodDescriptor(
    '/SearchService/SearchGear',
    grpcWeb.MethodType.UNARY,
    Services_Indexer_Proto_SearchController_pb.SearchRequest,
    Services_Indexer_Proto_SearchController_pb.SearchResponse,
    (request: Services_Indexer_Proto_SearchController_pb.SearchRequest) => {
      return request.serializeBinary();
    },
    Services_Indexer_Proto_SearchController_pb.SearchResponse.deserializeBinary
  );

  searchGear(
    request: Services_Indexer_Proto_SearchController_pb.SearchRequest,
    metadata: grpcWeb.Metadata | null): Promise<Services_Indexer_Proto_SearchController_pb.SearchResponse>;

  searchGear(
    request: Services_Indexer_Proto_SearchController_pb.SearchRequest,
    metadata: grpcWeb.Metadata | null,
    callback: (err: grpcWeb.RpcError,
               response: Services_Indexer_Proto_SearchController_pb.SearchResponse) => void): grpcWeb.ClientReadableStream<Services_Indexer_Proto_SearchController_pb.SearchResponse>;

  searchGear(
    request: Services_Indexer_Proto_SearchController_pb.SearchRequest,
    metadata: grpcWeb.Metadata | null,
    callback?: (err: grpcWeb.RpcError,
               response: Services_Indexer_Proto_SearchController_pb.SearchResponse) => void) {
    if (callback !== undefined) {
      return this.client_.rpcCall(
        this.hostname_ +
          '/SearchService/SearchGear',
        request,
        metadata || {},
        this.methodDescriptorSearchGear,
        callback);
    }
    return this.client_.unaryCall(
    this.hostname_ +
      '/SearchService/SearchGear',
    request,
    metadata || {},
    this.methodDescriptorSearchGear);
  }

}

