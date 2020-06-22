using System;
using Xarcade.Domain.Models;

namespace Xarcade.Domain.Params
{
    public class CreateMosaicParams
    {
        /// <summary>
        /// Account to create the mosaic from
        /// </summary>
        public AccountDTO accountDTO = null;
        /// <summary>
        /// The mosaic supply mutability.
        /// </summary>
        public bool isSupplyMutable = true;
        /// <summary>
        /// The mosaic transferability. Default is true
        /// </summary>
        public bool isTransferrable = true;
        /// <summary>
        /// The mosaic levy mutability. Default is true
        /// </summary>
        public bool isLevyMutable = false;
        /// <summary>
        /// The mosaic divisibility. Default is false
        /// </summary>
        public int divisibility = 0;
        /// <summary>
        /// The number of blocks the mosaic will be active. Default is 1000
        /// </summary>
        public ulong duration = 1000;
    }
}