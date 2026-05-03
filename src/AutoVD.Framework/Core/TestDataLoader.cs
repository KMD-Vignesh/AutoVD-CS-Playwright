using System;
using System.IO;
using Newtonsoft.Json;
using AutoVD.Framework.Models;

namespace AutoVD.Framework.Core
{
    public static class TestDataLoader
    {
        private static readonly Lazy<TestDataFile> _testData = new(() =>
        {
            var dataPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "..", "..", "..", "..", "test-data", "test-data.json");

            if (!File.Exists(dataPath))
            {
                dataPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "test-data", "test-data.json");
            }

            var json = File.ReadAllText(dataPath);
            return JsonConvert.DeserializeObject<TestDataFile>(json);
        });

        public static TestDataFile Load()
        {
            return _testData.Value;
        }

        public static User GetUser(string name)
        {
            var data = Load();
            foreach (var user in data.Users)
            {
                if (user.Name.Equals(name, StringComparison.OrdinalIgnoreCase))
                {
                    return user;
                }
            }
            throw new ArgumentException($"User '{name}' not found in test data");
        }
    }
}
