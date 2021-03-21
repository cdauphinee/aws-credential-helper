using System;
using System.Collections.Generic;
using NUnit.Framework;
using FluentAssertions;
using AwsCredentialHelper.Core;

namespace AwsCredentialHelper.Tests
{
    [Parallelizable]
    public class GitParametersTests
    {
        private static string[] DictionaryKeys = new string[] { "protocol", "host", "path" };

        [Test]
        public void GitParameters_FromDictionary_initializes_with_data_from_dictionary()
        {
            var dictionary = new Dictionary<string, string>
            {
                ["protocol"] = "ssh",
                ["host"] = "some-host",
                ["path"] = "/repo/path"
            };

            var parameters = GitParameters.FromDictionary(dictionary);
            parameters.Protocol.Should().Be(dictionary["protocol"]);
            parameters.Host.Should().Be(dictionary["host"]);
            parameters.Path.Should().Be(dictionary["path"]);
        }

        [Test]
        public void GitParameters_FromDictionary_throws_on_missing_entry([ValueSourceAttribute("DictionaryKeys")] string missingEntryKey)
        {
            var dictionary = new Dictionary<string, string>
            {
                ["protocol"] = "protocol",
                ["host"] = "host",
                ["path"] = "path"
            };
            dictionary.Remove(missingEntryKey);

            Action call = () => GitParameters.FromDictionary(dictionary);
            call.Should().Throw<ArgumentException>();
        }
    }
}