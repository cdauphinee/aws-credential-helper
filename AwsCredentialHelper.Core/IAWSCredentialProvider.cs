
namespace AwsCredentialHelper.Core
{
    public interface IAWSCredentialProvider
    {
        AWSCredentials GetCredentials();
    }
}
