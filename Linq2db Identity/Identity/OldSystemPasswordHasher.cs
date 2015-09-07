using Microsoft.AspNet.Identity;

namespace Linq2db_Identity.Identity
{
    /// <summary> 
    /// Implement your own or leave it default
    /// </summary> 
    public class OldSystemPasswordHasher : PasswordHasher
    {
        public override string HashPassword(string password)
        {
            return base.HashPassword(password);
        }

        public override PasswordVerificationResult VerifyHashedPassword(string hashedPassword, string providedPassword)
        {
            return base.VerifyHashedPassword(hashedPassword, providedPassword);
        }
    }
}