using LinqToDB.Mapping;

namespace Linq2db_Identity.Models
{
    [Table("UserRoles")]
    public partial class UserRole
    {
        [Column, NotNull]
        public string UserId { get; set; } // varchar(36)
        [Column, NotNull]
        public string Name { get; set; } // varchar(100)
        [PrimaryKey, Identity]
        public int Id { get; set; } // int(11)

        #region Associations

        /// <summary>
        /// User_UserRoles
        /// </summary>
        [Association(ThisKey = "UserId", OtherKey = "Id", CanBeNull = false, KeyName = "User_UserRoles", BackReferenceName = "UserUserRoles")]
        public User User { get; set; }

        #endregion
    }
}