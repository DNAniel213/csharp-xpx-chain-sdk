using System;


namespace Xarcade.Domain.Authentication
{
    /// <summary>Xarcade's Account Model</summary>
    public class ApiKeyDTO
    {
        public string apiKey {get; set;}
        public ApiKeyType type {get; set;}
        public long userID {get; set;}
        public DateTime created {get; set;}
    }

    public enum ApiKeyType 
    {
        READ,
        CREATE,
        ADMIN
    }
}