using LinqToDB.Mapping;

namespace Linq2db_Identity.Models
{
    [Table("UserClaims")]
    public partial class UserClaim
    {
        [Column, NotNull]
        public string UserId { get; set; } // varchar(36)
        [Column, NotNull]
        public string ClaimType { get; set; } // varchar(50)
        [Column, NotNull]
        public string ClaimValue { get; set; } // varchar(50)
        [PrimaryKey, Identity]
        public int Id { get; set; } // int(11)

        #region Associations

        /// <summary>
        /// User_UserClaims
        /// </summary>
        [Association(ThisKey = "UserId", OtherKey = "Id", CanBeNull = false, KeyName = "User_UserClaims", BackReferenceName = "UserUserClaims")]
        public User User { get; set; }

        #endregion
    }
}