namespace cnd.DTOs
{
    public class JournalEntryDto
    {
        public int JournalEntryId { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? LastUpdated { get; set; }
        public SongDto? Song { get; set; }
    }
}
