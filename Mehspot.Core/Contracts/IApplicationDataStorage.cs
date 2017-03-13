using Mehspot.Core.DTO;
using Mehspot.Core.Push;

namespace mehspot.Core.Contracts
{
    public interface IApplicationDataStorage
    {
        AuthenticationInfoDto AuthInfo { get; set; }
        string OldPushToken { get; set; }
        string PushToken { get; set; }
        bool PushIsEnabled { get; set; }
        bool PushDeviceTokenSentToBackend { get; set; }
        OsType OsType { get; }

        ProfileDto Profile { get; set; }
    }
}
