using cnd.DTOs;
using cnd.Models;
using Microsoft.EntityFrameworkCore;

namespace cnd.Services
{
    public interface IJournalEntryService
    {
        Task<List<JournalEntry>> GetEntriesByUserAsync(int userId);
        Task<JournalEntry?> GetEntryByIdAsync(int entryId, int userId);
        Task<JournalEntry> CreateEntryAsync(int userId, JournalEntryCreateDto dto);
        Task<bool> UpdateEntryAsync(int entryId, int userId, JournalEntryUpdateDto dto);
        Task<bool> DeleteEntryAsync(int entryId, int userId);
        Task<bool> DeleteSongAsync(int entryId, int userId);
    }

    public class JournalEntryService : IJournalEntryService
    {
        private readonly JournalDbContext _context;

        public JournalEntryService(JournalDbContext context)
        {
            _context = context;
        }

        public async Task<List<JournalEntry>> GetEntriesByUserAsync(int userId)
        {
            return await _context.JournalEntries
                .Include(e => e.Song)
                .Where(e => e.UserId == userId)
                .OrderBy(e => e.CreatedAt)
                .ToListAsync();
        }

        public async Task<JournalEntry?> GetEntryByIdAsync(int entryId, int userId)
        {
            return await _context.JournalEntries
                .Include(e => e.Song)
                .FirstOrDefaultAsync(e => e.JournalEntryId == entryId && e.UserId == userId);
        }

        public async Task<JournalEntry> CreateEntryAsync(int userId, JournalEntryCreateDto dto)
        {
            var entry = new JournalEntry
            {
                Title = dto.Title,
                Content = dto.Content,
                CreatedAt = DateTime.UtcNow,
                LastUpdated = DateTime.UtcNow,
                UserId = userId
            };

            if (dto.Song != null)
            {
                entry.Song = new Song
                {
                    Artist = dto.Song.Artist,
                    Title = dto.Song.Title
                };
            }

            _context.JournalEntries.Add(entry);
            await _context.SaveChangesAsync();
            return entry;
        }

        public async Task<bool> UpdateEntryAsync(int entryId, int userId, JournalEntryUpdateDto dto)
        {
            var entry = await GetEntryByIdAsync(entryId, userId);
            if (entry == null) return false;

            entry.Title = dto.Title;
            entry.Content = dto.Content;
            entry.LastUpdated = DateTime.UtcNow;

            if (dto.Song != null)
            {
                if (entry.Song == null)
                {
                    entry.Song = new Song
                    {
                        Artist = dto.Song.Artist,
                        Title = dto.Song.Title
                    };
                }
                else
                {
                    entry.Song.Artist = dto.Song.Artist;
                    entry.Song.Title = dto.Song.Title;
                }
            }
            else if (entry.Song != null)
            {
                _context.Songs.Remove(entry.Song);
            }

            await _context.SaveChangesAsync();
            return true;
        }


        public async Task<bool> DeleteEntryAsync(int entryId, int userId)
        {
            var entry = await GetEntryByIdAsync(entryId, userId);
            if (entry == null) return false;

            if (entry.Song != null)
                _context.Songs.Remove(entry.Song);

            _context.JournalEntries.Remove(entry);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteSongAsync(int entryId, int userId)
        {
            var entry = await GetEntryByIdAsync(entryId, userId);
            if (entry?.Song == null) return false;

            _context.Songs.Remove(entry.Song);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
