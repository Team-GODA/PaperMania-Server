namespace Server.Application.Configure;

public static class CacheTTLConfig
{
    public static class Session
    {
        public static readonly TimeSpan Timeout = TimeSpan.FromDays(31);
    }
}