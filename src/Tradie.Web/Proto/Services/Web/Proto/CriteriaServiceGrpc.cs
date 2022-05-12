// <auto-generated>
//     Generated by the protocol buffer compiler.  DO NOT EDIT!
//     source: Services/Web/Proto/CriteriaService.proto
// </auto-generated>
#pragma warning disable 0414, 1591
#region Designer generated code

using grpc = global::Grpc.Core;

namespace Tradie.Web.Proto {
  public static partial class CriteriaService
  {
    static readonly string __ServiceName = "CriteriaService";

    [global::System.CodeDom.Compiler.GeneratedCode("grpc_csharp_plugin", null)]
    static void __Helper_SerializeMessage(global::Google.Protobuf.IMessage message, grpc::SerializationContext context)
    {
      #if !GRPC_DISABLE_PROTOBUF_BUFFER_SERIALIZATION
      if (message is global::Google.Protobuf.IBufferMessage)
      {
        context.SetPayloadLength(message.CalculateSize());
        global::Google.Protobuf.MessageExtensions.WriteTo(message, context.GetBufferWriter());
        context.Complete();
        return;
      }
      #endif
      context.Complete(global::Google.Protobuf.MessageExtensions.ToByteArray(message));
    }

    [global::System.CodeDom.Compiler.GeneratedCode("grpc_csharp_plugin", null)]
    static class __Helper_MessageCache<T>
    {
      public static readonly bool IsBufferMessage = global::System.Reflection.IntrospectionExtensions.GetTypeInfo(typeof(global::Google.Protobuf.IBufferMessage)).IsAssignableFrom(typeof(T));
    }

    [global::System.CodeDom.Compiler.GeneratedCode("grpc_csharp_plugin", null)]
    static T __Helper_DeserializeMessage<T>(grpc::DeserializationContext context, global::Google.Protobuf.MessageParser<T> parser) where T : global::Google.Protobuf.IMessage<T>
    {
      #if !GRPC_DISABLE_PROTOBUF_BUFFER_SERIALIZATION
      if (__Helper_MessageCache<T>.IsBufferMessage)
      {
        return parser.ParseFrom(context.PayloadAsReadOnlySequence());
      }
      #endif
      return parser.ParseFrom(context.PayloadAsNewBuffer());
    }

    [global::System.CodeDom.Compiler.GeneratedCode("grpc_csharp_plugin", null)]
    static readonly grpc::Marshaller<global::Tradie.Web.Proto.ListCriteriaRequest> __Marshaller_ListCriteriaRequest = grpc::Marshallers.Create(__Helper_SerializeMessage, context => __Helper_DeserializeMessage(context, global::Tradie.Web.Proto.ListCriteriaRequest.Parser));
    [global::System.CodeDom.Compiler.GeneratedCode("grpc_csharp_plugin", null)]
    static readonly grpc::Marshaller<global::Tradie.Web.Proto.ListCriteriaResponse> __Marshaller_ListCriteriaResponse = grpc::Marshallers.Create(__Helper_SerializeMessage, context => __Helper_DeserializeMessage(context, global::Tradie.Web.Proto.ListCriteriaResponse.Parser));

    [global::System.CodeDom.Compiler.GeneratedCode("grpc_csharp_plugin", null)]
    static readonly grpc::Method<global::Tradie.Web.Proto.ListCriteriaRequest, global::Tradie.Web.Proto.ListCriteriaResponse> __Method_ListCriteria = new grpc::Method<global::Tradie.Web.Proto.ListCriteriaRequest, global::Tradie.Web.Proto.ListCriteriaResponse>(
        grpc::MethodType.Unary,
        __ServiceName,
        "ListCriteria",
        __Marshaller_ListCriteriaRequest,
        __Marshaller_ListCriteriaResponse);

    /// <summary>Service descriptor</summary>
    public static global::Google.Protobuf.Reflection.ServiceDescriptor Descriptor
    {
      get { return global::Tradie.Web.Proto.CriteriaServiceReflection.Descriptor.Services[0]; }
    }

    /// <summary>Base class for server-side implementations of CriteriaService</summary>
    [grpc::BindServiceMethod(typeof(CriteriaService), "BindService")]
    public abstract partial class CriteriaServiceBase
    {
      [global::System.CodeDom.Compiler.GeneratedCode("grpc_csharp_plugin", null)]
      public virtual global::System.Threading.Tasks.Task<global::Tradie.Web.Proto.ListCriteriaResponse> ListCriteria(global::Tradie.Web.Proto.ListCriteriaRequest request, grpc::ServerCallContext context)
      {
        throw new grpc::RpcException(new grpc::Status(grpc::StatusCode.Unimplemented, ""));
      }

    }

    /// <summary>Creates service definition that can be registered with a server</summary>
    /// <param name="serviceImpl">An object implementing the server-side handling logic.</param>
    [global::System.CodeDom.Compiler.GeneratedCode("grpc_csharp_plugin", null)]
    public static grpc::ServerServiceDefinition BindService(CriteriaServiceBase serviceImpl)
    {
      return grpc::ServerServiceDefinition.CreateBuilder()
          .AddMethod(__Method_ListCriteria, serviceImpl.ListCriteria).Build();
    }

    /// <summary>Register service method with a service binder with or without implementation. Useful when customizing the  service binding logic.
    /// Note: this method is part of an experimental API that can change or be removed without any prior notice.</summary>
    /// <param name="serviceBinder">Service methods will be bound by calling <c>AddMethod</c> on this object.</param>
    /// <param name="serviceImpl">An object implementing the server-side handling logic.</param>
    [global::System.CodeDom.Compiler.GeneratedCode("grpc_csharp_plugin", null)]
    public static void BindService(grpc::ServiceBinderBase serviceBinder, CriteriaServiceBase serviceImpl)
    {
      serviceBinder.AddMethod(__Method_ListCriteria, serviceImpl == null ? null : new grpc::UnaryServerMethod<global::Tradie.Web.Proto.ListCriteriaRequest, global::Tradie.Web.Proto.ListCriteriaResponse>(serviceImpl.ListCriteria));
    }

  }
}
#endregion