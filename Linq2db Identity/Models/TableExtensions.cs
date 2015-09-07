using System.Linq;
using LinqToDB;

namespace Linq2db_Identity.Models
{
    public static partial class TableExtensions
    {
       
        public static Login Find(this ITable<Login> table, string Id)
        {
            return table.FirstOrDefault(t =>
                t.Id == Id);
        }

        public static User Find(this ITable<User> table, string Id)
        {
            return table.FirstOrDefault(t =>
                t.Id == Id);
        }

        public static UserClaim Find(this ITable<UserClaim> table, int Id)
        {
            return table.FirstOrDefault(t =>
                t.Id == Id);
        }

        public static UserRole Find(this ITable<UserRole> table, int Id)
        {
            return table.FirstOrDefault(t =>
                t.Id == Id);
        }
    }
}