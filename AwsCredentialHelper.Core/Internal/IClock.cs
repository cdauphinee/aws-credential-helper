using System;

namespace AwsCredentialHelper.Core.Internal
{
    internal interface IClock
    {
        DateTime UtcNow { get; }
    }
}
