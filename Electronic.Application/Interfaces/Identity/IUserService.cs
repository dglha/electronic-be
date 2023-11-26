﻿namespace Electronic.Application.Contracts.Identity;

public interface IUserService
{
    public string UserId { get; }
    public bool IsLogged { get; }
    
    public string IpAddress { get; }
}