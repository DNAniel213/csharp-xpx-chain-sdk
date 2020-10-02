namespace Xarcade.WebApi.Controllers.Xarcade.Models
{
    public class TokenViewModel
    {
        public string TokenId {get; set;} = null;
        public string Name { get; set; } = null;
        public ulong Quantity { get; set; } = 0;
        public string Namespace {get; set; } = null;
    }
}