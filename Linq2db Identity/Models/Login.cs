using LinqToDB.Mapping;

namespace Linq2db_Identity.Models
{
    [Table("Logins")]
    public partial class Login
    {
        [PrimaryKey, NotNull]
        public string Id { get; set; } // varchar(36)
        [Column, NotNull]
        public string UserId { get; set; } // varchar(36)
        [Column, NotNull]
        public string LoginProvider { get; set; } // varchar(500)
        [Column, NotNull]
        public string ProviderKey { get; set; } // varchar(500)
    }
}