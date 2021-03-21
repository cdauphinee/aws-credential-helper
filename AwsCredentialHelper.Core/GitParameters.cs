using System;
using System.Collections.Generic;

namespace AwsCredentialHelper.Core
{
    public record GitParameters
    {
        public readonly string Protocol;
        public readonly string Host;
        public readonly string Path;

        public static GitParameters FromDictionary(Dictionary<string, string> parameters)
        {
            string GetOrThrow(string key) => parameters.GetValueOrDefault(key)
                ?? throw new ArgumentException($"Input parameters require a value for '{key}'", nameof(parameters));
            return new GitParameters(GetOrThrow("protocol"), GetOrThrow("host"), GetOrThrow("path"));
        }

        public GitParameters(string protocol, string host, string path)
        {
            this.Protocol = protocol;
            this.Host = host;
            this.Path = path;
        }
    }
}
