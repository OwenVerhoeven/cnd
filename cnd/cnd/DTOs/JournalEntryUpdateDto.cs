namespace cnd.DTOs
{
    public class JournalEntryUpdateDto
    {
        public string Title { get; set; }
        public string Content { get; set; }
        public SongDto? Song { get; set; }
    }
}
