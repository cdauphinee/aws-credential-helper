using System;
using System.Linq;
using Amazon.Util;

namespace AwsCredentialHelper.Core.Internal
{
    internal class AWSCredentialProvider : IAWSCredentialProvider
    {
        public static AWSCredentialProvider Instance = new AWSCredentialProvider();

        public AWSCredentials GetCredentials()
        {
            // TODO: Can we get the default credentials from the AWS SDK?

            var credentials = default(IAMSecurityCredentialMetadata);

            // TODO: This has a long retry loop if it can't reach the metadata service. May want to manually grab the
            // credentials, so we can fail faster.
            var allCredentials = EC2InstanceMetadata.IAMSecurityCredentials;
            if (allCredentials != null)
            {
                // EC2InstanceMetadata implies that /latest/meta-data/iam/security-credentials/ could return multiple
                // role names, but botocore assumes that it only returns a single role.
                if (allCredentials.Count == 1)
                {
                    credentials = allCredentials.First().Value;
                }
                else if (allCredentials.Count > 1)
                {
                    // Prefer the local credentials from vault, in case there are multiple
                    if (!allCredentials.TryGetValue("local-credentials", out credentials))
                    {
                        credentials = allCredentials.First().Value;
                    }
                }
            }

            if (credentials == null)
                throw new InvalidOperationException("No credentials were found");

            return new AWSCredentials
            {
                AccessKey = credentials.AccessKeyId,
                SecretKey = credentials.SecretAccessKey,
                SessionToken = credentials.Token
            };
        }
    }
}
