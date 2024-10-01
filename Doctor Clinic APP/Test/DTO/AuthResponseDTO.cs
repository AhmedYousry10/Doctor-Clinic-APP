namespace Doctor_CLinic_API.DTO
{
    public class AuthResponseDTO
    {
        public string Message { get; set; }
        public int userId { get; set; }
        public string Token { get; set; }
        public string[] Role { get; set; }
    }
}
