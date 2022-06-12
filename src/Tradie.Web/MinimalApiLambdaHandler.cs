using Amazon.Lambda.ApplicationLoadBalancerEvents;
using Amazon.Lambda.AspNetCoreServer;
using Amazon.Lambda.AspNetCoreServer.Hosting.Internal;
using Amazon.Lambda.RuntimeSupport;
using Amazon.Lambda.Serialization.SystemTextJson;

namespace Tradie.Web;

public class MinimalApiLambdaHandler : ApplicationLoadBalancerLambdaRuntimeSupportServer {
	//      
	protected override HandlerWrapper CreateHandlerWrapper(IServiceProvider serviceProvider) {
		var server = new ApplicationLoadBalancerMinimalApi(serviceProvider);
		server.RegisterResponseContentEncodingForContentType("application/grpc-web", ResponseContentEncoding.Base64);
		server.RegisterResponseContentEncodingForContentType("application/grpc-web+proto", ResponseContentEncoding.Base64);
		server.RegisterResponseContentEncodingForContentType("application/protobuf", ResponseContentEncoding.Base64);

		var serializer = new DefaultLambdaJsonSerializer();

		return HandlerWrapper.GetHandlerWrapper<ApplicationLoadBalancerRequest, ApplicationLoadBalancerResponse>(server.FunctionHandlerAsync, serializer);
	}

	public MinimalApiLambdaHandler(IServiceProvider serviceProvider) : base(serviceProvider) {
	}
}