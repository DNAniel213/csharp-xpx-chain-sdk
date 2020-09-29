using System.Collections.Generic;

namespace Xarcade.WebApi.Controllers.Xarcade.Models
{
    public class OwnerViewModel : AccountViewModel
    {
        public List<UserViewModel> Users { get; set; } = null;
    }
}