using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;

namespace IisLogArchiver.Core
{
    public abstract class BaseSettings
    {
        protected string GetConnectionString(string key)
        {
            var connectionStringSettings = ConfigurationManager.ConnectionStrings[key];
            if (connectionStringSettings == null)
                throw new ArgumentException("GetConnectionString key: " + key);
            return connectionStringSettings.ConnectionString;
        }

        protected string GetFromAppConfig(string key)
        {
            var value = ConfigurationManager.AppSettings[key];
            if (string.IsNullOrEmpty(value))
                throw new ArgumentException("GetFromAppConfig key: " + key);
            return value;
        }
        public static string GetFromAppConfig(string key, string defaultValue)
        {
            var value = ConfigurationManager.AppSettings[key];
            if (string.IsNullOrEmpty(value))
                return defaultValue;
            return value;
        }

        protected IEnumerable<string> GetListFromAppConfig(string key)
        {
            return GetFromAppConfig(key)
                .Split(new[] { ',', ';' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(v => v.Trim());
        }
    }
}