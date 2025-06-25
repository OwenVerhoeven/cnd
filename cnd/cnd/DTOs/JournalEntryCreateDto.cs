namespace cnd.DTOs
{
    public class JournalEntryCreateDto
    {
        public string Title { get; set; }
        public string Content { get; set; }

        public SongDto? Song { get; set; }
    }

    public class SongDto
    {
        public string Artist { get; set; }
        public string Title { get; set; }
    }

}
