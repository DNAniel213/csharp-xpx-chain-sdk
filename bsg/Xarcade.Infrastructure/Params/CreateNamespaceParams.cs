using Xarcade.Domain.ProximaX;

namespace Xarcade.Infrastructure.ProximaX.Params
{
    public class CreateNamespaceParams
    {
        public Account Account {get; set;} = null;
        public string Domain {get; set;} = null;
        public ulong Duration {get; set;} = 1000;
        public string Parent {get; set;} = null;
    }
}