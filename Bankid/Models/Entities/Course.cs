using Bankid.Models.Base;
using System;

namespace Bankid.Models.Entities {
    public class Course : IBaseEntity {
        public int Id { get; set; }
        public DateTime CreatedDate { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public int AuthorId { get; set; }
        public string Html { get; set; }
        public bool OnlyAdmin { get; set; }

        public User Author { get; set; }
    }
}
