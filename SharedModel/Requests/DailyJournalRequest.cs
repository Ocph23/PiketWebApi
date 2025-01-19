namespace SharedModel.Requests;

public record DailyJournalRequest(int Id, string? Title, string? Content, int TeacherId, DateTime CreateAt = default)
{
    public DateTime CreateAt { get; init; } = CreateAt == default ? DateTime.Now.ToUniversalTime() : CreateAt;
}

