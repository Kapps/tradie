/*using Amazon.Lambda.AspNetCoreServer;

namespace Tradie.Web;

public class Handler : ApplicationLoadBalancerFunction {
	protected override void Init(IWebHostBuilder builder) {
		RegisterResponseContentEncodingForContentType("application/grpc-web", ResponseContentEncoding.Base64);
		RegisterResponseContentEncodingForContentType("application/grpc-web+proto", ResponseContentEncoding.Base64);
		builder.
		base.Init(builder);
	}
}*/