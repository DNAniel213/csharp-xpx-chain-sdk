using System;
using Xarcade.Domain.Models;

namespace Xarcade.Domain.Params
{
    public class LinkMosaicParams
    {
        /// <summary>
        /// Account to link the mosaic from
        /// </summary>
        public AccountDTO accountDTO = null;
        public ulong mosaicID = 0;
        public NamespaceDTO namespaceDTO = null;
    }

}