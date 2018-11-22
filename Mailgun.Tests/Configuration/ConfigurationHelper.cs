using System;
using System.IO;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestPlatform.TestHost;

namespace Mailgun.Tests.Configuration {
    public class ConfigurationHelper {
        internal static String EnvironmentName { get; }
        internal static String BasePath { get; }
        internal static IConfigurationRoot Root { get; }

        static ConfigurationHelper() {
            BasePath = Environment.GetEnvironmentVariable("BASE_DIRECTORY");

            if (BasePath == null) {
                BasePath = AppContext.BaseDirectory;

                // cross-platform equivalent of "../../../../"
                for (var index = 0; index < 4; index++) {
                    BasePath = Directory.GetParent(BasePath).FullName;
                }
            }

            EnvironmentName = Environment.GetEnvironmentVariable("ENVIRONMENT") ?? "local";

            Root =
                new ConfigurationBuilder()
                    .SetBasePath(BasePath)
                    .AddJsonFile("config/appsettings.json", optional: true)
                    .AddJsonFile($"config/appsettings.{EnvironmentName}.json", optional: true)
                    .AddEnvironmentVariables()
                    .Build();
        }
    }
}