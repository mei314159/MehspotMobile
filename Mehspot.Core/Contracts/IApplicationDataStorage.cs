using System;
using mehspot.core.Auth.Dto;

namespace mehspot.core.Contracts
{
    public interface IApplicationDataStorage
    {
        AuthenticationInfoResult AuthInfo { get; set; }
    }
}
