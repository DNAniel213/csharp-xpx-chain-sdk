using Xarcade.Domain.Models;

namespace Xarcade.Domain.Params
{
    public class CreateNamespaceParams
    {
        public AccountDTO accountDTO = null;
        public string Domain = null;
        public ulong duration = 1000;
        public string LayerOne = null;
        public string LayerTwo = null;
    }
}