/**
 * @fileoverview gRPC-Web generated client stub for 
 * @enhanceable
 * @public
 */

// GENERATED CODE -- DO NOT EDIT!


/* eslint-disable */
// @ts-nocheck


import * as grpcWeb from 'grpc-web';

import * as Services_Web_Proto_ItemTypeService_pb from '../../../Services/Web/Proto/ItemTypeService_pb';


export class ItemTypeServiceClient {
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

  methodDescriptorListItemTypes = new grpcWeb.MethodDescriptor(
    '/ItemTypeService/ListItemTypes',
    grpcWeb.MethodType.UNARY,
    Services_Web_Proto_ItemTypeService_pb.ListItemTypesRequest,
    Services_Web_Proto_ItemTypeService_pb.ListItemTypesResponse,
    (request: Services_Web_Proto_ItemTypeService_pb.ListItemTypesRequest) => {
      return request.serializeBinary();
    },
    Services_Web_Proto_ItemTypeService_pb.ListItemTypesResponse.deserializeBinary
  );

  listItemTypes(
    request: Services_Web_Proto_ItemTypeService_pb.ListItemTypesRequest,
    metadata: grpcWeb.Metadata | null): Promise<Services_Web_Proto_ItemTypeService_pb.ListItemTypesResponse>;

  listItemTypes(
    request: Services_Web_Proto_ItemTypeService_pb.ListItemTypesRequest,
    metadata: grpcWeb.Metadata | null,
    callback: (err: grpcWeb.RpcError,
               response: Services_Web_Proto_ItemTypeService_pb.ListItemTypesResponse) => void): grpcWeb.ClientReadableStream<Services_Web_Proto_ItemTypeService_pb.ListItemTypesResponse>;

  listItemTypes(
    request: Services_Web_Proto_ItemTypeService_pb.ListItemTypesRequest,
    metadata: grpcWeb.Metadata | null,
    callback?: (err: grpcWeb.RpcError,
               response: Services_Web_Proto_ItemTypeService_pb.ListItemTypesResponse) => void) {
    if (callback !== undefined) {
      return this.client_.rpcCall(
        this.hostname_ +
          '/ItemTypeService/ListItemTypes',
        request,
        metadata || {},
        this.methodDescriptorListItemTypes,
        callback);
    }
    return this.client_.unaryCall(
    this.hostname_ +
      '/ItemTypeService/ListItemTypes',
    request,
    metadata || {},
    this.methodDescriptorListItemTypes);
  }

}

