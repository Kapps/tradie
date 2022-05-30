
using Amazon.SimpleSystemsManagement;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.Extensions.Options;
using StackExchange.Redis;
using System.IO.Compression;
using System.Net;
using Tradie.Analyzer.Repos;
using Tradie.Common;
using Tradie.Web;
using Tradie.Web.Services;
using ICompressionProvider = Grpc.Net.Compression.ICompressionProvider;


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
	.AddScoped<IItemTypeRepository, ItemTypeDbRepository>();

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
		new[] { "application/octet-stream", "application/grpc-web", "application/grpc-web+proto" }
	);
});

var app = builder.Build();

app.UseGrpcWeb(new GrpcWebOptions() { DefaultEnabled = true });
app.UseCors();

// Configure the HTTP request pipeline.
app.MapGrpcService<ModifierService>().RequireCors("AllowAll");
app.MapGrpcService<LeagueService>().RequireCors("AllowAll");
app.MapGrpcService<CriteriaService>().RequireCors("AllowAll");
app.MapGrpcService<SearchService>().RequireCors("AllowAll");
app.MapGrpcService<ItemTypeService>().RequireCors("AllowAll");
app.MapGrpcService<AffixRangeService>().RequireCors("AllowAll");

//app.UseResponseCompression();

/*app.Use(async (context, next) => {
	context.Response.Headers.Add();
	await next();
});*/

app.MapGet("/",
	() =>
		"Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");


app.Run();
