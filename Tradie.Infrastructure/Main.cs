using Amazon.SimpleSystemsManagement;
using System;
using Constructs;
using HashiCorp.Cdktf;
using HashiCorp.Cdktf.Providers.Aws;
using HashiCorp.Cdktf.Providers.Random;
using System.IO;
using System.Net;
using System.Net.Http;
using Tradie.Infrastructure.Aspects;
using Tradie.Infrastructure.Resources;
using System.Collections.Generic;
using Tradie.Infrastructure;
using Tradie.Infrastructure.Analyzer;
using Tradie.Infrastructure.Foundation;
using Tradie.Infrastructure.Scanner;

App app = new(new AppOptions(){StackTraces = true});
var httpClient = new HttpClient();
var localIp = IPAddress.Parse(httpClient.GetStringAsync("https://api.ipify.org").Result);
var config = new ResourceConfig() {
    Environment = "dev",
    Region = "ca-central-1",
    //Region = "us-east-1",
    BaseDirectory = Path.Combine(Directory.GetCurrentDirectory(), "../src/"),
    Version = "0.1.1",
    LocalIpAddress = localIp,
};


var foundation = new FoundationStack(app, "foundation", config);
var scanner = new ScannerStack(app, "scanner", config, foundation);
var analyzer = new AnalyzerStack(app, "analyzer", config, foundation, scanner);

app.Synth();

Console.WriteLine("App synth complete");