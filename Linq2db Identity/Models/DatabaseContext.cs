using LinqToDB;

namespace Linq2db_Identity.Models
{
    public partial class DatabaseContext : LinqToDB.Data.DataConnection
    {
        public ITable<Login> Logins { get { return this.GetTable<Login>(); } }
        public ITable<User> Users { get { return this.GetTable<User>(); } }
        public ITable<UserClaim> UserClaims { get { return this.GetTable<UserClaim>(); } }
        public ITable<UserRole> UserRoles { get { return this.GetTable<UserRole>(); } }

        public DatabaseContext()
        {
            InitDataContext();
        }

        public DatabaseContext(string configuration)
            : base(configuration)
        {
            InitDataContext();
        }

        partial void InitDataContext();
    }
}