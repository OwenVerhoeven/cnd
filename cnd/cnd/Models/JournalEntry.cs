using cnd.Models;

public class JournalEntry
{
    public int JournalEntryId { get; set; }
    public string Title { get; set; }
    public string Content { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? LastUpdated { get; set; }  

    public int UserId { get; set; }
    public User User { get; set; }

    public Song? Song { get; set; }
}
