namespace Jwt.Entities
{
    public class Token
    {
        public int Id { get; set; }
        public string RefreshToken { get; set; } = string.Empty;
        public DateTime RefreshTokenExpireTime { get; set; }
        public Guid UserUid { get; set; }
    }
}
