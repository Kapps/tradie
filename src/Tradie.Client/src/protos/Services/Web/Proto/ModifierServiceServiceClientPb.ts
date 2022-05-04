/**
 * @fileoverview gRPC-Web generated client stub for 
 * @enhanceable
 * @public
 */

// GENERATED CODE -- DO NOT EDIT!


/* eslint-disable */
// @ts-nocheck


import * as grpcWeb from 'grpc-web';

import * as Services_Web_Proto_ModifierService_pb from '../../../Services/Web/Proto/ModifierService_pb';


export class ModifierServiceClient {
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

  methodDescriptorListModifiers = new grpcWeb.MethodDescriptor(
    '/ModifierService/ListModifiers',
    grpcWeb.MethodType.UNARY,
    Services_Web_Proto_ModifierService_pb.ListModifiersRequest,
    Services_Web_Proto_ModifierService_pb.ListModifiersResponse,
    (request: Services_Web_Proto_ModifierService_pb.ListModifiersRequest) => {
      return request.serializeBinary();
    },
    Services_Web_Proto_ModifierService_pb.ListModifiersResponse.deserializeBinary
  );

  listModifiers(
    request: Services_Web_Proto_ModifierService_pb.ListModifiersRequest,
    metadata: grpcWeb.Metadata | null): Promise<Services_Web_Proto_ModifierService_pb.ListModifiersResponse>;

  listModifiers(
    request: Services_Web_Proto_ModifierService_pb.ListModifiersRequest,
    metadata: grpcWeb.Metadata | null,
    callback: (err: grpcWeb.RpcError,
               response: Services_Web_Proto_ModifierService_pb.ListModifiersResponse) => void): grpcWeb.ClientReadableStream<Services_Web_Proto_ModifierService_pb.ListModifiersResponse>;

  listModifiers(
    request: Services_Web_Proto_ModifierService_pb.ListModifiersRequest,
    metadata: grpcWeb.Metadata | null,
    callback?: (err: grpcWeb.RpcError,
               response: Services_Web_Proto_ModifierService_pb.ListModifiersResponse) => void) {
    if (callback !== undefined) {
      return this.client_.rpcCall(
        this.hostname_ +
          '/ModifierService/ListModifiers',
        request,
        metadata || {},
        this.methodDescriptorListModifiers,
        callback);
    }
    return this.client_.unaryCall(
    this.hostname_ +
      '/ModifierService/ListModifiers',
    request,
    metadata || {},
    this.methodDescriptorListModifiers);
  }

}

