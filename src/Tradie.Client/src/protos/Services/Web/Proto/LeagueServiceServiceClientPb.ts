/**
 * @fileoverview gRPC-Web generated client stub for 
 * @enhanceable
 * @public
 */

// GENERATED CODE -- DO NOT EDIT!


/* eslint-disable */
// @ts-nocheck


import * as grpcWeb from 'grpc-web';

import * as Services_Web_Proto_LeagueService_pb from '../../../Services/Web/Proto/LeagueService_pb';


export class LeagueServiceClient {
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

  methodDescriptorListLeagues = new grpcWeb.MethodDescriptor(
    '/LeagueService/ListLeagues',
    grpcWeb.MethodType.UNARY,
    Services_Web_Proto_LeagueService_pb.ListLeaguesRequest,
    Services_Web_Proto_LeagueService_pb.ListLeaguesResponse,
    (request: Services_Web_Proto_LeagueService_pb.ListLeaguesRequest) => {
      return request.serializeBinary();
    },
    Services_Web_Proto_LeagueService_pb.ListLeaguesResponse.deserializeBinary
  );

  listLeagues(
    request: Services_Web_Proto_LeagueService_pb.ListLeaguesRequest,
    metadata: grpcWeb.Metadata | null): Promise<Services_Web_Proto_LeagueService_pb.ListLeaguesResponse>;

  listLeagues(
    request: Services_Web_Proto_LeagueService_pb.ListLeaguesRequest,
    metadata: grpcWeb.Metadata | null,
    callback: (err: grpcWeb.RpcError,
               response: Services_Web_Proto_LeagueService_pb.ListLeaguesResponse) => void): grpcWeb.ClientReadableStream<Services_Web_Proto_LeagueService_pb.ListLeaguesResponse>;

  listLeagues(
    request: Services_Web_Proto_LeagueService_pb.ListLeaguesRequest,
    metadata: grpcWeb.Metadata | null,
    callback?: (err: grpcWeb.RpcError,
               response: Services_Web_Proto_LeagueService_pb.ListLeaguesResponse) => void) {
    if (callback !== undefined) {
      return this.client_.rpcCall(
        this.hostname_ +
          '/LeagueService/ListLeagues',
        request,
        metadata || {},
        this.methodDescriptorListLeagues,
        callback);
    }
    return this.client_.unaryCall(
    this.hostname_ +
      '/LeagueService/ListLeagues',
    request,
    metadata || {},
    this.methodDescriptorListLeagues);
  }

}

