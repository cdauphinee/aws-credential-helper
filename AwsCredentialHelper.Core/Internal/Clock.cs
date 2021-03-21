using System;

namespace AwsCredentialHelper.Core.Internal
{
    internal class Clock : IClock
    {
        public static Clock Instance = new Clock();

        public DateTime UtcNow => DateTime.UtcNow;
    }
}
