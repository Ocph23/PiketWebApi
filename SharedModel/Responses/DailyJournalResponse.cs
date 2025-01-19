namespace SharedModel.Responses
{
    public record DailyJournalResponse(int Id, string Title, string Content, int TeacherId, string TeacherName, DateTime CreateAt);
}