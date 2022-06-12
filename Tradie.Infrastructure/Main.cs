using System;
using HashiCorp.Cdktf;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Collections.Generic;
using System.Linq;
using Tradie.Infrastructure;
using Tradie.Infrastructure.Analyzer;
using Tradie.Infrastructure.Foundation;
using Tradie.Infrastructure.ImageRepository;
using Tradie.Infrastructure.Indexer;
using Tradie.Infrastructure.Packaging;
using Tradie.Infrastructure.Scanner;
using Tradie.Infrastructure.Web;

var deployableStacks = new[] {
	"scanner", "analyzer", "web", "indexer"
};

var context = new Dictionary<string, object>();
App app = new(new AppOptions(){StackTraces = true, Context = context,});

var httpClient = new HttpClient();
var localIp = IPAddress.Parse(httpClient.GetStringAsync("https://api.ipify.org").Result);

string[] stacksToDeploy = (Environment.GetEnvironmentVariable("TRADIE_BUILD") ?? "")
	.Split(new[] {',', ' '}, StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
if(stacksToDeploy.Any(c => !deployableStacks.Contains(c))) {
	throw new ArgumentException("Invalid stack specified");
}

var config = new ResourceConfig() {
    Environment = "dev",
    Region = "ca-central-1",
    //Region = "us-east-1",
    BaseDirectory = Path.Combine(Directory.GetCurrentDirectory(), "../src/"),
    Version = "0.1.1",
    LocalIpAddress = localIp,
    StacksToDeploy = stacksToDeploy
};

Console.WriteLine($"Deploying stacks {String.Join(", ", stacksToDeploy)}");

var packager = new DockerPackager(config);
//await packager.BuildAndPushProject($"Tradie.Analyzer/Dockerfile", "tradie-dev-analyzer-repo");

var foundation = new FoundationStack(app, "foundation", config);
var repos = new ImageRepositoryStack(app, "repositories", config);
var scanner = new ScannerStack(app, "scanner", config, foundation, new(packager, "Tradie.Scanner/Dockerfile", "scanner", "linux/arm64") {
	IsDirty = config.StacksToDeploy.Contains("scanner")
});
var analyzer = new AnalyzerStack(app, "analyzer", config, foundation, scanner, new(packager, "Tradie.Analyzer/Dockerfile", "analyzer", "linux/amd64") {
	IsDirty = config.StacksToDeploy.Contains("analyzer")
});
var web = new WebStack(app, "web", config, foundation, new(packager, "Tradie.Web/Dockerfile", "web", "linux/amd64") {
	IsDirty = config.StacksToDeploy.Contains("web")	
});
var indexer = new IndexerStack(app, "indexer", config, new(packager, "Tradie.Indexer/Dockerfile", "indexer", "linux/amd64") {
	IsDirty = config.StacksToDeploy.Contains("indexer")	
});

foreach(var deployable in new TerraformStack[] {scanner, analyzer, web}) {
	deployable.AddDependency(repos);
}

app.Synth();

Console.WriteLine("App synth complete");