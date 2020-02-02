using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace WebApi.Models
{
    public class LoginViewModel
    {
        [MaxLength(256)]
        public string Username { get; set; }

        [MaxLength(200)]
        [MinLength(8)]
        [DataType(DataType.Password)]
        public string Password { get; set; }

    }
}
