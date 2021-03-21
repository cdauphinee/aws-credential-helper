using System;
using System.Collections.Generic;
using AwsCredentialHelper.Core;

namespace AwsCredentialHelper
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length > 0 && args[0] == "get")
            {
                var parameters = ReadParameters();

                // In case the credential helper is accidentally used for a non-CodeCommit repo, not writing
                // anything will cause git to fall back.
                if (!parameters.Host.Contains("amazon.com", StringComparison.OrdinalIgnoreCase)
                    && !parameters.Host.Contains("amazonaws.com", StringComparison.OrdinalIgnoreCase))
                {
                    return;
                }

                var (username, password) = new CredentialGenerator().GenerateCredentials(parameters);
                Console.Out.Write($"username={username}\npassword={password}\n");
                Console.Out.Flush();
            }
        }

        private static GitParameters ReadParameters()
        {
            var parsed = new Dictionary<string, string>();

            string line;
            while ((line = Console.ReadLine()) != null)
            {
                var tokens = line.Split('=', 2);
                if (tokens.Length != 2)
                    throw new ArgumentException($"Expected input in the form of {{key}}={{value}}, but got: {line}");
                parsed[tokens[0]] = tokens[1];
            }

            return GitParameters.FromDictionary(parsed);
        }
    }
}
