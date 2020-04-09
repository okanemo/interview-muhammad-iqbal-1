using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Testing2.Models
{
    public class Users
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string UserName { get; set; }
        public string EmailAddress { get; set; }
        public string Password { get; set; }
        public int RoleId { get; set; }
    }
}
