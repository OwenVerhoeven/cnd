namespace cnd.Controllers
{
    using cnd.DTOs;
    using cnd.Services;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using System.Security.Claims;

    [ApiController]
    [Route("entries")]
    [Authorize]
    public class JournalEntriesController : ControllerBase
    {
        private readonly IJournalEntryService _service;

        public JournalEntriesController(IJournalEntryService service)
        {
            _service = service;
        }

        private int GetUserId() => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        [HttpPost]
        public async Task<IActionResult> CreateEntry([FromBody] JournalEntryCreateDto dto)
        {
            var entry = await _service.CreateEntryAsync(GetUserId(), dto);

            var result = new JournalEntryDto
            {
                JournalEntryId = entry.JournalEntryId,
                Title = entry.Title,
                Content = entry.Content,
                CreatedAt = entry.CreatedAt,
                LastUpdated = entry.LastUpdated,
                Song = entry.Song != null ? new SongDto
                {
                    Artist = entry.Song.Artist,
                    Title = entry.Song.Title
                } : null
            };

            return Ok(result);
        }

        [HttpGet]
        public async Task<IActionResult> GetEntries([FromQuery] DateTime? start, [FromQuery] DateTime? end)
        {
            var userId = GetUserId();
            var entries = await _service.GetEntriesByUserAsync(userId);

            if (start.HasValue)
                entries = entries.Where(e => e.CreatedAt >= start.Value).ToList();

            if (end.HasValue)
                entries = entries.Where(e => e.CreatedAt <= end.Value).ToList();

            var result = entries
                .OrderByDescending(e => e.CreatedAt)
                .Select(e => new JournalEntryDto
                {
                    JournalEntryId = e.JournalEntryId,
                    Title = e.Title,
                    Content = e.Content,
                    CreatedAt = e.CreatedAt,
                    LastUpdated = e.LastUpdated,
                    Song = e.Song != null ? new SongDto
                    {
                        Artist = e.Song.Artist,
                        Title = e.Song.Title
                    } : null
                });

            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetEntryById(int id)
        {
            var entry = await _service.GetEntryByIdAsync(id, GetUserId());

            if (entry == null) return NotFound();

            var result = new JournalEntryDto
            {
                JournalEntryId = entry.JournalEntryId,
                Title = entry.Title,
                Content = entry.Content,
                CreatedAt = entry.CreatedAt,
                LastUpdated = entry.LastUpdated,
                Song = entry.Song != null ? new SongDto
                {
                    Artist = entry.Song.Artist,
                    Title = entry.Song.Title
                } : null
            };

            return Ok(result);
        }


        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateEntry(int id, [FromBody] JournalEntryUpdateDto dto)
        {
            var success = await _service.UpdateEntryAsync(id, GetUserId(), dto);
            return success ? Ok("Entry bijgewerkt") : NotFound();
        }



        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEntry(int id)
        {
            var success = await _service.DeleteEntryAsync(id, GetUserId());
            return success ? Ok("Entry + song verwijderd") : NotFound();
        }

        [HttpDelete("{id}/song")]
        public async Task<IActionResult> DeleteSongOnly(int id)
        {
            var success = await _service.DeleteSongAsync(id, GetUserId());
            return success ? Ok("Song verwijderd uit entry") : NotFound();
        }
    }
}
