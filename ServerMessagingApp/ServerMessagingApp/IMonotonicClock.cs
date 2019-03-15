using System;
namespace ServerMessagingApp
{
    public interface IMonotonicClock
    {
        float GetNow();
    }
}
