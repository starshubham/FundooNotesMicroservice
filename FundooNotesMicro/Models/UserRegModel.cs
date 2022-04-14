using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace UserService.Models
{
    public class UserRegModel
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Email { get; set; }

        public string Password { get; set; }

        public string ConfirmPassword { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}
