using System;

namespace AwsCredentialHelper.Core
{
    public record AWSCredentials
    {
        private readonly string _accessKey;
        private readonly string _secretKey;

        public string AccessKey 
        {
            get => _accessKey;
            init => _accessKey = (value ?? throw new ArgumentNullException(nameof(AccessKey))); 
        }

        public string SecretKey
        {
            get => _secretKey;
            init => _secretKey = (value ?? throw new ArgumentNullException(nameof(SecretKey)));
        }

        public string SessionToken { get; init; }
    }
}
