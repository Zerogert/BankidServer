using Bankid.Models.Base;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Bankid.Models.Entities {
    public class User : IdentityUser<int>, IBaseEntity {
        public DateTime CreatedDate { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Role { get; set; }
        public IEnumerable<Course> Courses { get; set; }
    }
}
