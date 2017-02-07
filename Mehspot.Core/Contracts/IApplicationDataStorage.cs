using Mehspot.Core.DTO;

namespace mehspot.Core.Contracts
{
    public interface IApplicationDataStorage
    {
        AuthenticationInfoDto AuthInfo { get; set; }
    }
}
