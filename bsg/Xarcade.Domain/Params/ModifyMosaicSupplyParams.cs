using System;
using Xarcade.Domain.Models;

namespace Xarcade.Domain.Params
{
    public class ModifyMosaicSupplyParams
    {
        /// <summary>
        /// Account to create the mosaic from
        /// </summary>
        public AccountDTO accountDTO = null;
        public ulong mosaicID = 0;
        public int amount = 0;
    }

}