using System;
using NUnit.Framework;
using FakeItEasy;
using FluentAssertions;
using AwsCredentialHelper.Core;
using AwsCredentialHelper.Core.Internal;

namespace AwsCredentialHelper.Tests
{
    [NonParallelizable]
    public class CredentialGeneratorTests
    {
        private IClock _clock;
        private IAWSCredentialProvider _awsCredentialProvider;

        private CredentialGenerator _generator;

        private readonly GitParameters _parameters 
            = new GitParameters("https", "git-codecommit.us-east-1.amazonaws.com", "/v1/repos/my-sample-repo");

        [SetUp]
        public void Setup()
        {
            _awsCredentialProvider = A.Fake<IAWSCredentialProvider>();
            A.CallTo(() => _awsCredentialProvider.GetCredentials())
                .Returns(new AWSCredentials { AccessKey = "access_key", SecretKey = "secret_key" });

            _clock = A.Fake<IClock>();
            A.CallTo(() => _clock.UtcNow)
                .Returns(new DateTime(2021, 03, 20, 06, 11, 53, DateTimeKind.Utc));

            _generator = new CredentialGenerator(_awsCredentialProvider, _clock);
        }

        [Test]
        public void CredentialGenerator_Generate_returns_access_key_as_username()
        {
            var (username, _) = _generator.GenerateCredentials(_parameters);
            username.Should().Be("access_key");
        }

        [Test]
        public void CredentialGenerator_Generate_includes_token_in_username_if_available()
        {
            A.CallTo(() => _awsCredentialProvider.GetCredentials())
                .Returns(new AWSCredentials { AccessKey = "access_key", SecretKey = "secret_key", SessionToken = "token" });

            var (username, _) = _generator.GenerateCredentials(_parameters);
            username.Should().Be("access_key%token");
        }

        [Test]
        public void CredentialGenerator_Generate_prefixes_signature_with_timestamp()
        {
            A.CallTo(() => _clock.UtcNow)
                .Returns(new DateTime(1989, 08, 15, 12, 30, 59, DateTimeKind.Utc));

            var (_, password) = _generator.GenerateCredentials(_parameters);
            password.Should().StartWith("19890815T123059Z");
        }

        [Test]
        public void CredentialGenerator_Generate_returns_expected_password()
        {
            const string ExpectedPassword = "20210320T061153Za39f02e75f5f2b8f1f157323d8df5e7dcc7cdeb99028fd1116ef9a29f355ea80";

            var (_, password) = _generator.GenerateCredentials(_parameters);
            password.Should().Be(ExpectedPassword);
        }
    }
}