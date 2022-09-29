using Bankid.Models.Base;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;

namespace Bankid.Models.Entities {
	public class Role : IdentityRole<int>, IBaseEntity {
		public static readonly List<string> Roles = new List<string>() { "Admin", "User" };
		public static readonly string AdminRoleName = "Admin";
        public static readonly string UserRoleName = "User";
        public DateTime CreatedDate { get; set; }
	}
}
