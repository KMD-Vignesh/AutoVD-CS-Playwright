using System;
using System.IO;
using Microsoft.Extensions.Configuration;
using AutoVD.Framework.Models;

namespace AutoVD.Framework.Core
{
    public static class ConfigReader
    {
        private static readonly Lazy<IConfiguration> _configuration = new(() =>
        {
            var configPath = FindConfigPath("appsettings.json");
            var builder = new ConfigurationBuilder();

            if (!string.IsNullOrEmpty(configPath))
            {
                builder.SetBasePath(Path.GetDirectoryName(configPath)!);
                builder.AddJsonFile(configPath, optional: false, reloadOnChange: true);
            }
            else
            {
                builder.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
            }

            return builder.Build();
        });

        private static string FindConfigPath(string fileName)
        {
            var searchPaths = new[]
            {
                Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Config", fileName),
                Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "..", "..", "..", "..", "src", "AutoVD.Framework", "Config", fileName),
                Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "..", "..", "Config", fileName)
            };

            foreach (var path in searchPaths)
            {
                var fullPath = Path.GetFullPath(path);
                if (File.Exists(fullPath))
                {
                    return fullPath;
                }
            }

            return null!;
        }

        private static string FindProjectRoot()
        {
            var dir = AppDomain.CurrentDomain.BaseDirectory;

            while (dir != null)
            {
                if (File.Exists(Path.Combine(dir, "AutoVD.Framework.slnx")))
                {
                    return dir;
                }

                dir = Path.GetDirectoryName(dir);
            }

            return AppDomain.CurrentDomain.BaseDirectory;
        }

        public static string ProjectRoot => FindProjectRoot();

        public static AppSettings LoadSettings()
        {
            var settings = new AppSettings();
            _configuration.Value.Bind(settings);

            var projectRoot = FindProjectRoot();

            if (!Path.IsPathRooted(settings.ScreenshotPath))
            {
                settings.ScreenshotPath = Path.GetFullPath(Path.Combine(projectRoot, settings.ScreenshotPath));
            }

            if (!Path.IsPathRooted(settings.ReportPath))
            {
                settings.ReportPath = Path.GetFullPath(Path.Combine(projectRoot, settings.ReportPath));
            }

            return settings;
        }

        public static T GetSetting<T>(string key)
        {
            return _configuration.Value.GetValue<T>(key)!;
        }
    }
}
