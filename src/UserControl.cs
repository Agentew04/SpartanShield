using System.Threading.Tasks;

namespace SpartanShield
{
    public static class UserControl
    {
        public enum AuthResult
        {
            Success,
            UserNotExist,
            UserAlreadyExist,
            WrongPassword,
            PasswordNotMatch,
            MissingInfo,
            UnknownError
        }
        public async static Task<AuthResult> Register(string username, string password, string passwordAgain)
        {
            if (!password.Equals(passwordAgain))
                return AuthResult.PasswordNotMatch;

            if (string.IsNullOrWhiteSpace(username)
                || string.IsNullOrWhiteSpace(password)
                || string.IsNullOrWhiteSpace(passwordAgain))
                return AuthResult.MissingInfo;

            if (await Utils.GetUserHash(username) != null)
                return AuthResult.UserAlreadyExist;

            var passwordHash = Utils.HashString($"{password}{username}", Utils.HashSecurity.Safer);

            if (await Utils.SetUserHash(username, passwordHash))
            {
                SessionInfo.IsLoggedIn = true;
                SessionInfo.CurrentUsername = username;
                return AuthResult.Success;
            }
            else return AuthResult.UnknownError;
        }

        public async static Task<AuthResult> Login(string username, string password)
        {
            if (string.IsNullOrWhiteSpace(username)
                || string.IsNullOrWhiteSpace(password))
                return AuthResult.MissingInfo;

            string? storedHash = await Utils.GetUserHash(username);

            if (storedHash == null)
                return AuthResult.UserNotExist;

            string passwordHash = Utils.HashString($"{password}{username}", Utils.HashSecurity.Safer);

            if (!passwordHash.Equals(storedHash))
                return AuthResult.WrongPassword;
            else
            {
                SessionInfo.IsLoggedIn = true;
                SessionInfo.CurrentUsername = username;
                return AuthResult.Success;
            }
        }
    }
}
