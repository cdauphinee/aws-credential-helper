using System;
using System.Text;
using System.Text.RegularExpressions;
using AwsCredentialHelper.Core.Internal;

namespace AwsCredentialHelper.Core
{
    public class CredentialGenerator
    {
        private readonly IAWSCredentialProvider _awsCredentialProvider;
        private readonly IClock _clock;

        public CredentialGenerator() 
            : this(AWSCredentialProvider.Instance)
        {
        }

        public CredentialGenerator(IAWSCredentialProvider awsCredentialProvider) 
            : this(awsCredentialProvider, Clock.Instance)
        {
        }

        internal CredentialGenerator(IAWSCredentialProvider awsCredentialProvider, IClock clock)
        {
            _awsCredentialProvider = awsCredentialProvider;
            _clock = clock;
        }

        public (string username, string password) GenerateCredentials(GitParameters parameters)
        {
            var uri = BuildUri(parameters);
            var region = GetRegion(parameters);

            var awsCredentials = _awsCredentialProvider.GetCredentials();

            var signature = CalculateSignature(region, uri, awsCredentials.SecretKey);

            var usernameBuilder = new StringBuilder(awsCredentials.AccessKey);
            if (!String.IsNullOrEmpty(awsCredentials.SessionToken))
                usernameBuilder.Append('%').Append(awsCredentials.SessionToken);

            return (usernameBuilder.ToString(), signature);
        }

        private Uri BuildUri(GitParameters parameters)
        {
            var builder = new UriBuilder();
            builder.Scheme = parameters.Protocol;
            builder.Host = parameters.Host;
            builder.Path = parameters.Path;
            return builder.Uri;
        }

        private static readonly Regex RegionRegex 
            = new Regex(@"(vpce-.+\.)?git-codecommit(-fips)?\.([^.]+)\.(vpce\.)?amazonaws\.com");
        private string GetRegion(GitParameters parameters)
        {
            var match = RegionRegex.Match(parameters.Host);
            if (match.Success)
                return match.Groups[3].Value;
            // TODO: Fall back to region global config; can we get this from the SDK?
            return null;
        }

        private string CalculateSignature(string region, Uri url, string secretKey)
        {
            var now = _clock.UtcNow;
            var shortDate = now.ToString("yyyyMMdd");
            var longDate = now.ToString("yyyyMMddTHHmmss");

            var signingKey = GetSignatureKey(secretKey, shortDate, region, "codecommit");

            var canonicalRequest = $"GIT\n{url.LocalPath}\n\nhost:{url.Host}\n\nhost\n";
            var canonicalRequestHash = HashUtility.CalculateSHA256(canonicalRequest);

            var stringToSign = $"AWS4-HMAC-SHA256\n{longDate}\n{shortDate}/{region}/codecommit/aws4_request\n{HashUtility.HexStringFromBytes(canonicalRequestHash)}";
            var signatureBytes = HashUtility.CalculateSHA256(stringToSign, signingKey);
            return $"{longDate}Z{HashUtility.HexStringFromBytes(signatureBytes)}";
        }

        // Derived from https://docs.aws.amazon.com/general/latest/gr/signature-v4-examples.html#signature-v4-examples-dotnet
        private static byte[] GetSignatureKey(string key, string dateString, string region, string service)
        {
            var dateKey = HashUtility.CalculateSHA256(dateString, Encoding.UTF8.GetBytes($"AWS4{key}"));
            var regionKey = HashUtility.CalculateSHA256(region, dateKey);
            var serviceKey = HashUtility.CalculateSHA256(service, regionKey);
            return HashUtility.CalculateSHA256("aws4_request", serviceKey);
        }
    }
}
