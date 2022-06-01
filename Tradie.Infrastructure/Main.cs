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

App app = new App();
            var httpClient = new HttpClient();
            var localIp = IPAddress.Parse(httpClient.GetStringAsync("https://api.ipify.org").Result);
            var config = new ResourceConfig() {

	            Environment = "dev",
	            //Region = "ca-central-1",
	            Region = "us-east-1",
	            BaseDirectory = Path.Combine(Directory.GetCurrentDirectory(), "../src/"),
	            Version = "0.1.0",
	            LocalIpAddress = localIp,
            };
            var devStack = new MyApp(app, "tradie-dev", );

app.Synth();
            Console.WriteLine("App synth complete");