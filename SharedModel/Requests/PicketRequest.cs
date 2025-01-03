namespace SharedModel.Responses;


public class PicketRequest
{
    public int Id { get; set; }
    public DateOnly Date { get; set; }
    public Weather Weather { get; set; }
    public TimeSpan? StartAt { get; set; }
    public TimeSpan? EndAt { get; set; }
    public int CreatedId { get; set; }
    public string? CreatedName { get; set; }
    public string? CreatedNumber { get; set; }
    public DateTime CreateAt { get; set; } = DateTime.Now.ToUniversalTime();
}
