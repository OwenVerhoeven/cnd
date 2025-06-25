namespace cnd.Models
{
    public class Song
    {
        public int SongId { get; set; }
        public string Artist { get; set; }
        public string Title { get; set; }

        public int JournalEntryId { get; set; }
        public JournalEntry JournalEntry { get; set; }
    }

}
