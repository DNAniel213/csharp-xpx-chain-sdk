namespace Xarcade.Domain.Models
{
    public class MosaicDTO : AssetDTO
    {
        public ulong MosaicID {get; set;}
        public NamespaceDTO Namespace {get; set;}
    }
}