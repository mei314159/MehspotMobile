using Mehspot.Core.DTO;
using Mehspot.Core.Push;

namespace Mehspot.Core.Contracts
{
    public interface IApplicationDataStorage
    {
        AuthenticationInfoDTO AuthInfo { get; set; }
        ProfileDto Profile { get; set; }
        string OldPushToken { get; set; }
        string PushToken { get; set; }
        bool PushIsEnabled { get; set; }
        bool PushDeviceTokenSentToBackend { get; set; }
        bool WalkthroughPassed { get; }
        OsType OsType { get; }

        T Get<T>(string key);
        void Set<T>(string key, T result) where T : class;
    }
}
