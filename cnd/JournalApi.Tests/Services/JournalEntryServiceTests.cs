using cnd;
using cnd.DTOs;
using cnd.Models;
using cnd.Services;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace JournalApi.Tests.Services
{
    public class JournalEntryServiceTests
    {
        private JournalDbContext GetDbContext()
        {
            var options = new DbContextOptionsBuilder<JournalDbContext>()
                .UseInMemoryDatabase(databaseName: $"TestDb_{Guid.NewGuid()}")
                .Options;
            return new JournalDbContext(options);
        }

        private JournalEntryCreateDto SampleEntry(string title = "Dagboek", string content = "Inhoud", string? artist = null, string? songTitle = null)
        {
            return new JournalEntryCreateDto
            {
                Title = title,
                Content = content,
                Song = artist != null && songTitle != null ? new SongDto { Artist = artist, Title = songTitle } : null
            };
        }

        private JournalEntryUpdateDto SampleUpdate(string title = "Dagboek Update", string content = "Inhoud Update", string? artist = null, string? songTitle = null)
        {
            return new JournalEntryUpdateDto
            {
                Title = title,
                Content = content,
                Song = artist != null && songTitle != null ? new SongDto { Artist = artist, Title = songTitle } : null
            };
        }

        [Fact]
        public async Task CreateEntryAsync_CreatesEntryWithSong()
        {
            var db = GetDbContext();
            var service = new JournalEntryService(db);

            var dto = SampleEntry("Titel", "Tekst", "Eminem", "Lose Yourself");
            var entry = await service.CreateEntryAsync(1, dto);

            Assert.NotNull(entry);
            Assert.Equal("Titel", entry.Title);
            Assert.NotNull(entry.Song);
            Assert.Equal("Eminem", entry.Song.Artist);
        }

        [Fact]
        public async Task CreateEntryAsync_CreatesEntryWithoutSong()
        {
            var db = GetDbContext();
            var service = new JournalEntryService(db);

            var dto = SampleEntry("Zonder song", "Tekst");
            var entry = await service.CreateEntryAsync(1, dto);

            Assert.NotNull(entry);
            Assert.Null(entry.Song);
        }

        [Fact]
        public async Task UpdateEntryAsync_UpdatesEntryAndLastUpdated()
        {
            var db = GetDbContext();
            var service = new JournalEntryService(db);

            var dto = SampleEntry("Oud", "Oude tekst");
            var entry = await service.CreateEntryAsync(42, dto);
            var oldLastUpdated = entry.LastUpdated;

            await Task.Delay(100); // om zeker te zijn dat er tijdsverschil is

            var updateDto = SampleUpdate("Nieuw", "Nieuwe tekst", "Nas", "N.Y. State of Mind");
            var success = await service.UpdateEntryAsync(entry.JournalEntryId, entry.UserId, updateDto);

            var updated = await service.GetEntryByIdAsync(entry.JournalEntryId, entry.UserId);

            Assert.True(success);
            Assert.Equal("Nieuw", updated!.Title);
            Assert.True(updated.LastUpdated > oldLastUpdated);
            Assert.Equal("Nas", updated.Song!.Artist);
        }

        [Fact]
        public async Task UpdateEntryAsync_RemovesSong_WhenNullPassed()
        {
            var db = GetDbContext();
            var service = new JournalEntryService(db);

            var entry = await service.CreateEntryAsync(9, SampleEntry("Met song", "tekst", "Eminem", "Mockingbird"));

            var updateDto = SampleUpdate("Zonder song", "Nieuwe tekst", null, null);
            await service.UpdateEntryAsync(entry.JournalEntryId, entry.UserId, updateDto);

            var updated = await service.GetEntryByIdAsync(entry.JournalEntryId, entry.UserId);

            Assert.Equal("Zonder song", updated!.Title);
            Assert.Null(updated.Song);
        }

        [Fact]
        public async Task DeleteEntryAsync_RemovesEntryAndSong()
        {
            var db = GetDbContext();
            var service = new JournalEntryService(db);

            var entry = await service.CreateEntryAsync(7, SampleEntry("Del", "del", "2Pac", "Changes"));
            var deleted = await service.DeleteEntryAsync(entry.JournalEntryId, entry.UserId);

            var found = await service.GetEntryByIdAsync(entry.JournalEntryId, entry.UserId);

            Assert.True(deleted);
            Assert.Null(found);
            Assert.Empty(db.Songs.ToList());
        }

        [Fact]
        public async Task DeleteSongAsync_RemovesOnlySong()
        {
            var db = GetDbContext();
            var service = new JournalEntryService(db);

            var entry = await service.CreateEntryAsync(3, SampleEntry("Met song", "bla", "Drake", "God's Plan"));

            var result = await service.DeleteSongAsync(entry.JournalEntryId, entry.UserId);

            var reloaded = await service.GetEntryByIdAsync(entry.JournalEntryId, entry.UserId);

            Assert.True(result);
            Assert.Null(reloaded!.Song);
        }

        [Fact]
        public async Task GetEntriesByUserAsync_ReturnsOnlyUserEntriesSortedByCreationDate()
        {
            var db = GetDbContext();
            var service = new JournalEntryService(db);

            await service.CreateEntryAsync(10, SampleEntry("E1", "x"));
            await Task.Delay(50);
            await service.CreateEntryAsync(10, SampleEntry("E2", "y"));
            await Task.Delay(50);
            await service.CreateEntryAsync(10, SampleEntry("E3", "z"));

            await service.CreateEntryAsync(99, SampleEntry("Anders", "Not for user 10"));

            var entries = await service.GetEntriesByUserAsync(10);

            Assert.Equal(3, entries.Count);
            Assert.True(entries[0].CreatedAt < entries[1].CreatedAt);
            Assert.True(entries[1].CreatedAt < entries[2].CreatedAt);
        }
    }
}
