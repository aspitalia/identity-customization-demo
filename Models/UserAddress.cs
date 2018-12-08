using Microsoft.AspNetCore.Identity;

namespace IdentityCustomizationDemo.Models
{
    public class UserAddress
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        [ProtectedPersonalData]
        public string Address { get; set; }
    }
}