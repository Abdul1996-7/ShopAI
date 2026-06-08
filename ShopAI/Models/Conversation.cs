namespace ShopAI.Models;

public sealed class Conversation
{
    public int Id { get; set; }

    public int StoreId { get; set; }

    public Store Store { get; set; } = null!;

    public string PlatformSenderId { get; set; } = string.Empty;

    public PlatformType PlatformType { get; set; } = PlatformType.Facebook;

    public AgentState CurrentAgentState { get; set; } = AgentState.Idle;

    public string MessageHistoryJson { get; set; } = "[]";

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}
