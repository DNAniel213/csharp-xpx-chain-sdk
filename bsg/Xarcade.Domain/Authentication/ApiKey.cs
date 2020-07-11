using System;


namespace Xarcade.Domain.Authentication
{
    /// <summary>Xarcade's Account Model</summary>
    public class ApiKey
    {
        public string Key {get; set;}
        public ApiKeyType Type {get; set;}
        public long UserID {get; set;}
        public DateTime Created {get; set;}
    }

    public enum ApiKeyType 
    {
        READ,
        CREATE,
        ADMIN
    }
}