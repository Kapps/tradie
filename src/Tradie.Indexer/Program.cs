﻿// See https://aka.ms/new-console-template for more information

using Amazon.CloudWatch;
using Amazon.S3;
using Amazon.ServiceDiscovery;
using Amazon.SimpleSystemsManagement;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Connections;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;
using System.Diagnostics;
using System.IO.Compression;
using System.Net;
using System.Runtime.InteropServices;
using System.Text.Json;
using System.Text.Json.Serialization;
using Tradie.Analyzer.Dispatch;
using Tradie.Analyzer.Repos;
using Tradie.Common;
using Tradie.Common.Metrics;
using Tradie.Common.Services;
using Tradie.Indexer;
using Tradie.Indexer.Pricing;
using Tradie.Indexer.Search;
using Tradie.Indexer.Storage;
using Tradie.ItemLog;
using HealthStatus = Microsoft.Extensions.Diagnostics.HealthChecks.HealthStatus;

var ssmClient = new AmazonSimpleSystemsManagementClient();
var s3Client = new AmazonS3Client();

await TradieConfig.InitializeFromEnvironment(ssmClient);

Console.WriteLine($"Starting Indexer with build hash {Environment.GetEnvironmentVariable("BUILD_HASH")} and league {TradieConfig.League}.");

if(String.IsNullOrWhiteSpace(TradieConfig.League)) {
	throw new ArgumentException("Indexer must be started with a league to index.");
}

string hostAddress = Environment.GetEnvironmentVariable("LISTEN_ADDR")
	?? throw new ArgumentException("No listen address set.");

var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;

services.AddGrpc(options => {
	options.EnableDetailedErrors = true;
	options.ResponseCompressionAlgorithm = "gzip";
	options.ResponseCompressionLevel = CompressionLevel.Fastest;
});

services.AddStackExchangeRedisCache(options => {
	options.ConfigurationOptions = new ConfigurationOptions() {
		Password = TradieConfig.RedisPass,
		//User = "default",
		ResolveDns = true,
		Ssl = false,
		AbortOnConnectFail = true,
		ClientName = "Tradie Indexer Cache",
		EndPoints = {
			new DnsEndPoint(TradieConfig.RedisHost, 6379)
		}
	};
});

services.AddLogging(builder => {
	builder.AddSimpleConsole(format => {
		format.TimestampFormat = "[yyyy-MM-dd HH:mm:ss.mmm] ";
		format.UseUtcTimestamp = true;
		format.IncludeScopes = false;
		format.SingleLine = true;
	});
	builder.SetMinimumLevel(TradieConfig.LogLevel);
});

services.AddSingleton<IParameterStore, SsmParameterStore>()
	.AddSingleton<IAmazonSimpleSystemsManagement>(ssmClient)
	.AddSingleton<IAmazonServiceDiscovery, AmazonServiceDiscoveryClient>()
	.AddSingleton<IAmazonS3>(s3Client)
	.AddSingleton<IAmazonCloudWatch, AmazonCloudWatchClient>()
	.AddSingleton<IMetricPublisher, CloudWatchMetricPublisher>()
	.AddSingleton<IItemLog, PostgresItemLog>()
	.AddSingleton<ItemTree>()
	.AddSingleton<INinjaApi, NinjaApiClient>()
	.AddSingleton<IPriceCache, S3PriceCache>()
	.AddSingleton<IPricingService, NinjaPricingService>()
	.AddSingleton<IBlockSearcher, PriceSortedBlockSearcher>()
	.AddSingleton<IServiceRegistry, CloudMapServiceRegistry>()
	.AddHostedService<ItemTreeLoaderService>()
	.AddHostedService(provider =>
		new CloudMapRegistrationService(
			provider.GetRequiredService<IServiceRegistry>(),
			hostAddress,
			provider.GetRequiredService<ILogger<CloudMapRegistrationService>>(),
			provider.GetRequiredService<IHostEnvironment>()
		)
	);

services.AddDbContext<AnalysisContext>(ServiceLifetime.Singleton);
builder.WebHost.ConfigureKestrel(options => {
	Console.WriteLine("Configuring Kestrel");
	if(builder.Environment.IsDevelopment()) {
		// Setup a HTTP/2 endpoint without TLS.
		Console.WriteLine("Using local env");
		options.ListenLocalhost(5000, o => o.Protocols =
			HttpProtocols.Http2);
	} else {
		options.ListenAnyIP(new Uri(hostAddress).Port, o => {
			//o.Protocols = HttpProtocols.Http2;
			o.UseHttps();
		});
	}
});

builder.Services.AddCors(o => o.AddPolicy("AllowAll", builder => {
	builder.AllowAnyOrigin()
		.AllowAnyMethod()
		.AllowAnyHeader()
		.WithExposedHeaders("Grpc-Status", "Grpc-Message", "Grpc-Encoding", "Grpc-Accept-Encoding");
}));

builder.Services.AddHealthChecks().AddCheck("Basic", () => HealthCheckResult.Healthy());

var app = builder.Build();

app.UseHealthChecks("/health", new HealthCheckOptions() {
	ResponseWriter = async (context, report) => {
		context.Response.ContentType = "application/json";
		await context.Response.WriteAsync(JsonSerializer.Serialize(report, new JsonSerializerOptions() {
			WriteIndented = true
		}));
	}
});

//app.UseGrpcWeb(new GrpcWebOptions() { DefaultEnabled = true });
app.UseCors();
// Configure the HTTP request pipeline.
app.MapGrpcService<SearchController>().RequireCors("AllowAll");

app.MapGet("/",
	() =>
		"Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");

app.Run();


/*


Process currentProcess = System.Diagnostics.Process.GetCurrentProcess();

long startBytes = currentProcess.WorkingSet64;


var test = new IndexTest();
//await test.SearchByBruteForce(itemLog);
await test.SearchByBlocks(itemLog);
//test.SearchByBlocks();

GC.Collect();
GC.WaitForPendingFinalizers();
GC.Collect();

currentProcess = System.Diagnostics.Process.GetCurrentProcess();
long endBytes = currentProcess.WorkingSet64;

test.PrintResults();

Console.WriteLine($"Started off with using {startBytes / (1024 * 1024)}MB, ended with {endBytes / (1024*1024)}MB.");*/