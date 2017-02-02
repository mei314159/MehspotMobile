using System;
using mehspot.Core.Dto;

namespace mehspot.Core.Contracts
{
    public interface IApplicationDataStorage
    {
        AuthenticationInfoDto AuthInfo { get; set; }
    }
}
