using Serilog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace ProbaDotnetSDK.Configuration
{
    internal class ConfigurationProvider
    {
        private ILogger Logger { get; }
        public ConfigurationModel Configuration { get; private set; }

        public async Task<bool> LoadConfigurationAsync(string name = "appsettings.json")
        {
            if (!File.Exists(name)) return false;
            try
            {
                using (var sr = new StreamReader(name))
                {
                    var json = await sr.ReadToEndAsync().ConfigureAwait(false);
                    Configuration = json.FromJson<ConfigurationModel>();
                    if (Configuration is null) return false;
                    return true;
                }
            }
            catch (Exception e)
            {
                Logger.Error(e, "the config file could not be read.");
                throw;
            }
        }

        public async Task SaveConfgurationAsync(ConfigurationModel config, string name = "appsettings.json")
        {
            try
            {
                using (var file = new System.IO.StreamWriter(name, false))
                {
                    var json = config.ToJson();
                    await file.WriteAsync(json);
                    Configuration = config;
                }
            }
            catch (Exception e)
            {
                Logger.Error(e, "the config file could not be read.");
                throw;
            }
        }
    }
}

