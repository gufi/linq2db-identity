using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using LinqToDB.Mapping;

namespace Linq2db_Identity.Models
{
    [Table("Users")]
    public partial class User
    {
        [PrimaryKey, NotNull]
        public string Id { get; set; } // varchar(36)
        [Column, NotNull]
        public string UserName { get; set; } // varchar(50)
        [Column, NotNull]
        public string Password { get; set; } // varchar(100)
        [Column, NotNull]
        public string Email { get; set; } // varchar(200)
        [Column, Nullable]
        public string PhoneNumber { get; set; } // varchar(20)
        [Column, Nullable]
        public string SecurityStamp { get; set; } // varchar(500)
        [Column, NotNull]
        public string PasswordSalt { get; set; } // varchar(256)
        [Column, NotNull]
        public bool Disabled { get; set; } // tinyint(1)
        [Column, NotNull]
        public bool EmailConfirmed { get; set; } // tinyint(1)
        [Column, NotNull]
        public int LoginAttempts { get; set; } // int(11)
        [Column, NotNull]
        public bool LockoutEnabled { get; set; } // tinyint(1)
        [Column, NotNull]
        public string EmailConfirmationToken { get; set; } // varchar(36)

        #region Associations

        /// <summary>
        /// User_UserClaims_BackReference
        /// </summary>
        [Association(ThisKey = "Id", OtherKey = "UserId", CanBeNull = true, IsBackReference = true)]
        public IEnumerable<UserClaim> UserUserClaims { get; set; }

        /// <summary>
        /// User_UserRoles_BackReference
        /// </summary>
        [Association(ThisKey = "Id", OtherKey = "UserId", CanBeNull = true, IsBackReference = true)]
        public IEnumerable<UserRole> UserUserRoles { get; set; }

        #endregion
    }
}
