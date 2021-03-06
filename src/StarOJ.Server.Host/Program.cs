﻿using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using System.CommandLine;
using System.CommandLine.Invocation;

namespace StarOJ.Server.Host
{
    public class Program
    {
        public static uint HttpPort { get; set; }

        public static uint HttpsPort { get; set; }

        public static string ApiServer
        {
            get => API.Clients.BaseClient.Url;
            set => API.Clients.BaseClient.Url = value;
        }

        public static void Main(string[] args)
        {
            RootCommand rootCommand = new RootCommand
            {
                Description = "Server Host for StarOJ"
            };

            rootCommand.AddOption(new Option(new string[] { "--api-server", "-s" }, "The server of StarOJ API.", new Argument<string>("https://localhost:5001")));
            // rootCommand.AddOption(new Option(new string[] { "--dir", "-d" }, "The path of working directory.", new Argument<string>("")));
            rootCommand.AddOption(new Option(new string[] { "--http-port" }, "The port of http.", new Argument<uint>(5000)));
            rootCommand.AddOption(new Option(new string[] { "--https-port" }, "The port of https.", new Argument<uint>(5001)));
            rootCommand.Handler = CommandHandler.Create((string apiServer, uint httpPort, uint httpsPort) =>
            {
                ApiServer = apiServer;
                HttpPort = httpPort;
                HttpsPort = httpsPort;
            });

            // Parse the incoming args and invoke the handler
            int cmdExitCode = rootCommand.InvokeAsync(args).Result;

            if (cmdExitCode != 0) return;

            CreateWebHostBuilder(args).Build().Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args)
        {
            return WebHost.CreateDefaultBuilder(args)
.UseUrls($"http://*:{HttpPort}", $"https://*:{HttpsPort}")
.UseStartup<Startup>();
        }
    }
}
