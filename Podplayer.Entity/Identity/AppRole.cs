using Microsoft.AspNetCore.Identity;

namespace Podplayer.Entity.Identity
{
    public class AppRole : IdentityRole
    {
        public AppRole() : base() { }
        public AppRole(string name) : base(name) { }
    }
}
