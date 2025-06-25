namespace cnd.Models
{
    public class User
    {
        public int UserId { get; set; }
        public string Username { get; set; }
        public string PasswordHash { get; set; }

        public ICollection<JournalEntry> Entries { get; set; }
    }

}
