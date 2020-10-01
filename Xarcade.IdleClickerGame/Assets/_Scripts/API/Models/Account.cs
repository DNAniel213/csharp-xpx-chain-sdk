using System;
using UnityEngine;

namespace Xarcade.Models
{
    [System.Serializable]
    public class Account
    {
        [SerializeField]
        public string userId;

        [SerializeField]
        public string firstName;

        [SerializeField]
        public string lastName;

        [SerializeField]
        public string email;

        [SerializeField]
        public string username;

        [SerializeField]
        public string password;

        [SerializeField]
        public bool acceptTerms;


    }
}
