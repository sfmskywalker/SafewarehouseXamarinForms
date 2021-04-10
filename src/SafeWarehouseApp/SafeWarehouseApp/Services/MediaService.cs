using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using SafeWarehouseApp.Models;
using SafeWarehouseApp.Persistence;
using Xamarin.Essentials;

namespace SafeWarehouseApp.Services
{
    public class MediaService : IMediaService
    {
        private readonly IStore<MediaItem> _mediaItemStore;
        public MediaService(IStore<MediaItem> mediaItemStore) => _mediaItemStore = mediaItemStore;

        public async Task SaveFileResultAsync(FileResult fileResult, string path)
        {
            await using var stream = await fileResult.OpenReadAsync();
            await using var newStream = File.OpenWrite(GetFullPath(path));
            await stream.CopyToAsync(newStream);
        }

        public async Task<MediaItem> SaveAsMediaItem(FileResult fileResult, string? tag = default)
        {
            var mediaItem = await CreateMediaItem(fileResult, tag);
            await _mediaItemStore.AddAsync(mediaItem);
            return mediaItem;
        }
        
        public async Task<MediaItem> CreateMediaItem(FileResult fileResult, string? tag = default)
        {
            var extension = Path.GetExtension(fileResult.FileName);

            var mediaItem = new MediaItem
            {
                Id = Guid.NewGuid().ToString("N"),
                FileName = $"{Guid.NewGuid():N}{extension}",
                Tag = tag
            };

            await SaveFileResultAsync(fileResult, mediaItem.FileName);
            return mediaItem;
        }

        public async Task<string?> GetMediaItemPathAsync(string mediaItemId)
        {
            var mediaItem = await _mediaItemStore.FindAsync(mediaItemId);
            return mediaItem == null ? null : GetFullPath(mediaItem.FileName);
        }

        public async Task DeleteManyByTagAsync(string tag)
        {
            var mediaItems = (await _mediaItemStore.FindManyAsync(x => x.Tag!.Contains(tag))).ToList();

            foreach (var mediaItem in mediaItems)
            {
                var fullPath = GetFullPath(mediaItem.FileName);

                try
                {
                    File.Delete(fullPath);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    throw;
                }
            }

            await _mediaItemStore.DeleteManyAsync(mediaItems);
        }

        private static string GetFullPath(string path) => Path.Combine(FileSystem.AppDataDirectory, path);
    }
}