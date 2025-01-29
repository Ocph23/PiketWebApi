namespace PiketWebApi.Data  
{
    /// <summary>
    /// Represents a daily journal entry.
    /// </summary>
    public class DailyJournal
    {
        public int Id { get; set; }
        public string? Title { get; set; }
        public string? Content { get; set; }
        public Teacher Teacher { get; set; }
        public DateTime CreateAt { get; set; } = DateTime.Now.ToUniversalTime();

    }
}
