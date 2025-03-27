using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DAL.Entities;

namespace BL.Models.Auth
{
    public class RegisterModel
    {
        public string Email { get; set; } = null!;
        public string Name { get; set; } = null!;
        public string Surname { get; set; } = null!;
        public string Password { get; set; } = null!;
        
    }
}
