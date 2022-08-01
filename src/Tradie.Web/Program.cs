
using Amazon;
using Amazon.Lambda.AspNetCoreServer.Hosting.Internal;
using Amazon.Lambda.AspNetCoreServer.Internal;
using Amazon.ServiceDiscovery;
using Amazon.SimpleSystemsManagement;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.Extensions.Options;
using StackExchange.Redis;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.IO.Compression;
using System.Net;
using Tradie.Analyzer.Repos;
using Tradie.Common;
using Tradie.Common.Services;
using Tradie.Web;
using Tradie.Web.Formatters;
using Tradie.Web.Services;
using BrotliCompressionProvider = Microsoft.AspNetCore.ResponseCompression.BrotliCompressionProvider;
using ICompressionProvider = Grpc.Net.Compression.ICompressionProvider;


AWSConfigs.LoggingConfig.LogTo = LoggingOptions.Console;
var ssm = new AmazonSimpleSystemsManagementClient();
await TradieConfig.InitializeFromEnvironment(ssm);

var builder = WebApplication.CreateBuilder(args);

// Additional configuration is required to successfully run gRPC on macOS.
// For instructions on how to configure Kestrel and gRPC clients on macOS, visit https://go.microsoft.com/fwlink/?linkid=2099682

// Add services to the container.
builder.Services.AddGrpc(options => {
	options.EnableDetailedErrors = true;
	options.CompressionProviders.Add(new Tradie.Web.BrotliCompressionProvider());
	options.CompressionProviders.Add(new Grpc.Net.Compression.DeflateCompressionProvider(CompressionLevel.Optimal));
	options.CompressionProviders.Add(new Grpc.Net.Compression.GzipCompressionProvider(CompressionLevel.Optimal));
	//options.CompressionProviders = new List<ICompressionProvider>();
	//options.CompressionProviders.Add(new BrotliCompressionProvider(new BrotliCompressionProviderOptions()));
	//options.ResponseCompressionAlgorithm = "br";
	//options.ResponseCompressionAlgorithm = "gzip";
	options.ResponseCompressionAlgorithm = "br";
	options.ResponseCompressionLevel = CompressionLevel.Optimal;

});

builder.Services.AddStackExchangeRedisCache(options => {
	options.ConfigurationOptions = new ConfigurationOptions() {
		Password = TradieConfig.RedisPass,
		//User = "default",
		ResolveDns = true,
		Ssl = false,
		AbortOnConnectFail = true,
		ClientName = "Tradie Web Cache",
		EndPoints = {
			new DnsEndPoint(TradieConfig.RedisHost, 6379)
		}
	};
});

builder.Services.AddScoped<IModifierRepository, ModifierDbRepository>()
	.AddScoped<ILeagueRepository, LeagueRepository>()
	.AddScoped<IItemTypeRepository, ItemTypeDbRepository>()
	.AddSingleton<IAmazonServiceDiscovery, AmazonServiceDiscoveryClient>()
	.AddSingleton<IServiceRegistry, CloudMapServiceRegistry>()
	.AddSingleton<IGrpcServicePool, GrpcServicePool>();

builder.Services.AddDbContext<AnalysisContext>(ServiceLifetime.Transient);

builder.Services.AddCors(o => o.AddPolicy("AllowAll", builder => {
	builder.AllowAnyOrigin()
		.AllowAnyMethod()
		.AllowAnyHeader()
		.WithExposedHeaders("Grpc-Status", "Grpc-Message", "Grpc-Encoding", "Grpc-Accept-Encoding");
}));

builder.Services.AddLogging(builder => {
	builder.AddSimpleConsole(format => {
		format.TimestampFormat = "[yyyy-MM-dd HH:mm:ss.mmm] ";
		format.UseUtcTimestamp = true;
		format.IncludeScopes = false;
		format.SingleLine = true;
	});
	builder.SetMinimumLevel(TradieConfig.LogLevel);
});

builder.Services.AddResponseCompression(opts => {
	opts.EnableForHttps = true;
	opts.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(
		new[] { "application/octet-stream", "application/grpc-web", "application/grpc-web+proto", "application/protobuf", "text/plain", "text/html" }
	);
});

builder.Services.AddControllers(opts => {
	//opts.Conventions.Add(new SwaggerApplicationConvention());
	//opts.Conventions.Add(new ApiConventionApplicationModelConvention());
	opts.InputFormatters.Insert(0, new ProtoBufInputFormatter());
	opts.OutputFormatters.Insert(0, new ProtoBufOutputFormatter());
});

builder.Services.AddResponseCompression(opts => {
	opts.EnableForHttps = true;
	opts.MimeTypes = new[] {"application/protobuf", "text/html", "text/plain"};
	opts.Providers.Add(new BrotliCompressionProvider(new BrotliCompressionProviderOptions()));
});

if(!string.IsNullOrEmpty(Environment.GetEnvironmentVariable("AWS_LAMBDA_FUNCTION_NAME"))) {
	Utilities.EnsureLambdaServerRegistered(builder.Services, typeof(MinimalApiLambdaHandler));
}

//builder.Services.AddAWSLambdaHosting(LambdaEventSource.ApplicationLoadBalancer);
//var server = builder.Services.Single(c => c.ImplementationType == typeof(ApplicationLoadBalancerLambdaRuntimeSupportServer));
//((ApplicationLoadBalancerLambdaRuntimeSupportServer)server.ImplementationInstance)

var app = builder.Build();

app.UseCors("AllowAll");

/*app.UseGrpcWeb(new GrpcWebOptions() { DefaultEnabled = true });

// Configure the HTTP request pipeline.
app.MapGrpcService<ModifierService>().RequireCors("AllowAll");
app.MapGrpcService<LeagueService>().RequireCors("AllowAll");
app.MapGrpcService<CriteriaService>().RequireCors("AllowAll");
app.MapGrpcService<SearchService>().RequireCors("AllowAll");
app.MapGrpcService<ItemTypeService>().RequireCors("AllowAll");
app.MapGrpcService<AffixRangeService>().RequireCors("AllowAll");*/

/*app.UseEndpoints(ep => {
	ep.MapControllers();
});*/
app.MapControllers().RequireCors("AllowAll");

app.UseResponseCompression();

// Configure the HTTP request pipeline.
if(builder.Environment.IsDevelopment()) {
	app.UseDeveloperExceptionPage();
	//app.UseSwagger();
	//app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Tradie"));
}


/*app.Use(async (context, next) => {
	context.Response.Headers.Add();
	await next();
});*/

app.MapGet("/",
	() =>
		"Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");


app.Run();
