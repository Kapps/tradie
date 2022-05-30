/**
 * @fileoverview gRPC-Web generated client stub for 
 * @enhanceable
 * @public
 */

// GENERATED CODE -- DO NOT EDIT!


/* eslint-disable */
// @ts-nocheck


import * as grpcWeb from 'grpc-web';

import * as Services_Web_Proto_AffixRangeService_pb from '../../../Services/Web/Proto/AffixRangeService_pb';


export class AffixRangeServiceClient {
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

  methodDescriptorListAffixRanges = new grpcWeb.MethodDescriptor(
    '/AffixRangeService/ListAffixRanges',
    grpcWeb.MethodType.UNARY,
    Services_Web_Proto_AffixRangeService_pb.ListAffixRangesRequest,
    Services_Web_Proto_AffixRangeService_pb.ListAffixRangesResponse,
    (request: Services_Web_Proto_AffixRangeService_pb.ListAffixRangesRequest) => {
      return request.serializeBinary();
    },
    Services_Web_Proto_AffixRangeService_pb.ListAffixRangesResponse.deserializeBinary
  );

  listAffixRanges(
    request: Services_Web_Proto_AffixRangeService_pb.ListAffixRangesRequest,
    metadata: grpcWeb.Metadata | null): Promise<Services_Web_Proto_AffixRangeService_pb.ListAffixRangesResponse>;

  listAffixRanges(
    request: Services_Web_Proto_AffixRangeService_pb.ListAffixRangesRequest,
    metadata: grpcWeb.Metadata | null,
    callback: (err: grpcWeb.RpcError,
               response: Services_Web_Proto_AffixRangeService_pb.ListAffixRangesResponse) => void): grpcWeb.ClientReadableStream<Services_Web_Proto_AffixRangeService_pb.ListAffixRangesResponse>;

  listAffixRanges(
    request: Services_Web_Proto_AffixRangeService_pb.ListAffixRangesRequest,
    metadata: grpcWeb.Metadata | null,
    callback?: (err: grpcWeb.RpcError,
               response: Services_Web_Proto_AffixRangeService_pb.ListAffixRangesResponse) => void) {
    if (callback !== undefined) {
      return this.client_.rpcCall(
        this.hostname_ +
          '/AffixRangeService/ListAffixRanges',
        request,
        metadata || {},
        this.methodDescriptorListAffixRanges,
        callback);
    }
    return this.client_.unaryCall(
    this.hostname_ +
      '/AffixRangeService/ListAffixRanges',
    request,
    metadata || {},
    this.methodDescriptorListAffixRanges);
  }

}

