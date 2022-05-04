
using Amazon.SimpleSystemsManagement;
using StackExchange.Redis;
using System.IO.Compression;
using System.Net;
using Tradie.Analyzer.Repos;
using Tradie.Common;
using Tradie.Web.Services;


var ssm = new AmazonSimpleSystemsManagementClient();
await TradieConfig.InitializeFromEnvironment(ssm);

var builder = WebApplication.CreateBuilder(args);

// Additional configuration is required to successfully run gRPC on macOS.
// For instructions on how to configure Kestrel and gRPC clients on macOS, visit https://go.microsoft.com/fwlink/?linkid=2099682

// Add services to the container.
builder.Services.AddGrpc(options => {
	options.EnableDetailedErrors = true;
	options.ResponseCompressionAlgorithm = "gzip";
	options.ResponseCompressionLevel = CompressionLevel.Fastest;
});

builder.Services.AddStackExchangeRedisCache(options => {
	options.ConfigurationOptions = new ConfigurationOptions() {
		Password = TradieConfig.RedisPass,
		//User = "default",
		ResolveDns = true,
		Ssl = false,
		AbortOnConnectFail = true,
		ClientName = "Tradie Cache",
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

var app = builder.Build();

app.UseGrpcWeb(new GrpcWebOptions() { DefaultEnabled = true });
app.UseCors();
// Configure the HTTP request pipeline.
app.MapGrpcService<ModifierService>().RequireCors("AllowAll");
app.MapGrpcService<LeagueService>().RequireCors("AllowAll");
app.MapGrpcService<CriteriaService>().RequireCors("AllowAll");
app.MapGet("/",
	() =>
		"Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");


app.Run();
