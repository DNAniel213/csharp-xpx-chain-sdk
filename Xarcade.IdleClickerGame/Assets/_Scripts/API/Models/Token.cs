using System;
using UnityEngine;

namespace Xarcade.Models
{
    [System.Serializable]
    public class Token
    {
        public string TokenId {get; set;} = null;
        public string Name { get; set; } = null;
        public ulong Quantity { get; set; } = 0;


    }
}
