namespace FireFit.Shared.DTOs;

public class UserProgressDto
{
    public string UserId { get; set; } = string.Empty;
    public decimal CurrentWeight { get; set; }
    public decimal StartingWeight { get; set; }
    public DateTime UpdatedUtc { get; set; } = DateTime.UtcNow;
}

