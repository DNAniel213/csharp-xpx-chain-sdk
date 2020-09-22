namespace Xarcade.WebApi
{
    public class Routes
    {
        public const string GenerateOwner = "account/generate/owner"; //works
        public const string Owner = "account/owner";   //works
        public const string User = "account/user";  //works 
        public const string GenerateUser = "account/generate/user";  //works
        public const string GenerateToken = "token/generate/token"; //works
        public const string ModifyTokenSupply = "token/modify/supply"; //works
        public const string GenerateGame = "token/generate/game";  //works
        public const string ExtendGame = "token/extend/game"; //worksx
        public const string Token = "token/token";  //works
        public const string Game = "token/game";  //works
        public const string GenerateXarToken = "token/generate/xar";
        public const string SendToken = "transaction/send/token";
        public const string Register = "xarcadeaccount/register";
        public const string Authenticate = "xarcadeaccount/login";
        public const string VerifyEmail = "xarcadeaccount/verify";
        public const string GetXarUser = "xarcadeaccount/getxaruser";

    }
}