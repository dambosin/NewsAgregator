namespace NewsAgregator.WebApi.Responses
{
    public class TokenResponse
    {
        public string JwtToken { get; set; }
        public string RefreshToken { get; set; }
        public string Login{ get; set; }
    }
}
