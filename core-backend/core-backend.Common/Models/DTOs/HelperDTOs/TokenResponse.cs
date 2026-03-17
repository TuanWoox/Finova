namespace core_backend.Common.Models.DTOs.HelperDTOs
{
    public class TokenResponse
    {
        public string AccessToken { get; set; } = null!;
        public string RefreshToken { get; set; } = null!;
    }
}
