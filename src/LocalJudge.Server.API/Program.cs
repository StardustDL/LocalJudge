﻿using System;
using System.Collections.Generic;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using LocalJudge.Core;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace LocalJudge.Server.API
{
    public class Program
    {
        public static uint HttpPort { get; set; }

        public static uint HttpsPort { get; set; }

        public static void Main(string[] args)
        {
            var rootCommand = new RootCommand
            {
                Description = "API for LocalJudge"
            };

            rootCommand.AddOption(new Option(new string[] { "--dir", "-d" }, "The path of working directory.", new Argument<string>("")));
            rootCommand.AddOption(new Option(new string[] { "--http-port" }, "The port of http.", new Argument<uint>(5000)));
            rootCommand.AddOption(new Option(new string[] { "--https-port" }, "The port of https.", new Argument<uint>(5001)));
            rootCommand.Handler = CommandHandler.Create((string dir, uint httpPort, uint httpsPort) =>
            {
                if (!string.IsNullOrEmpty(dir))
                    Directory.SetCurrentDirectory(dir);
                HttpPort = httpPort;
                HttpsPort = httpsPort;
            });

            // Parse the incoming args and invoke the handler
            int cmdExitCode = rootCommand.InvokeAsync(args).Result;

            if (cmdExitCode != 0) return;

            CreateWebHostBuilder(args).Build().Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseUrls($"http://*:{HttpPort}", $"https://*:{HttpsPort}")
                .UseStartup<Startup>();
    }
}
