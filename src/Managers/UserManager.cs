namespace SpartanShield.Managers
{
    public static class UserManager
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
        public static AuthResult Register(string username, string password, string passwordAgain)
        {
            // pre match (check for UI related things) could be moved to code-behind
            if (!password.Equals(passwordAgain))
                return AuthResult.PasswordNotMatch;

            if (string.IsNullOrWhiteSpace(username)
                || string.IsNullOrWhiteSpace(password)
                || string.IsNullOrWhiteSpace(passwordAgain))
                return AuthResult.MissingInfo;

            // match for Database information
            if (DatabaseManager.UserExists(username)) return AuthResult.UserAlreadyExist;

            // hash password
            var passwordHash = Utils.HashString($"{password}{username}", Utils.HashSecurity.Safer);

            DatabaseManager.SetUserHash(username, passwordHash);
            if (DatabaseManager.GetUserHash(username) == passwordHash)
            {
                return AuthResult.Success;
            }
            else return AuthResult.UnknownError;
        }

        public static AuthResult Login(string username, string password)
        {
            if (string.IsNullOrWhiteSpace(username)
                || string.IsNullOrWhiteSpace(password))
                return AuthResult.MissingInfo;

            string? storedHash = DatabaseManager.GetUserHash(username);

            if (storedHash == null)
                return AuthResult.UserNotExist;

            string passwordHash = Utils.HashString($"{password}{username}", Utils.HashSecurity.Safer);

            if (!passwordHash.Equals(storedHash))
                return AuthResult.WrongPassword;
            else
            {
                return AuthResult.Success;
            }
        }
    }
}
