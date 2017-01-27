using System;
using mehspot.Core.Auth.Dto;

namespace mehspot.Core.Contracts
{
    public interface IApplicationDataStorage
    {
        AuthenticationInfoResult AuthInfo { get; set; }
    }
}
