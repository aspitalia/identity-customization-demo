using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;

namespace IdentityCustomizationDemo.Models
{
    public class ApplicationUser : IdentityUser
    {
        public ApplicationUser()
        {
            Addresses = new HashSet<UserAddress>();
        }
        [PersonalData]
        public string FiscalCode { get; set; }
        [PersonalData]
        public ICollection<UserAddress> Addresses { get; set; }
    }
}