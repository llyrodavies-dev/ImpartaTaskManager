namespace TaskManager.Application.Common.Models
{
    public class AuthResult
    {
        public string Token { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string DisplayName { get; set; } = string.Empty;
        public Guid UserId { get; set; }
    }
}
