using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using SafeWarehouseApp.Extensions;
using SafeWarehouseApp.Models;
using SafeWarehouseApp.Persistence;
using SkiaSharp;
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
        
        public async Task SaveBufferAsync(byte[] buffer, string path)
        {
            await using var newStream = File.OpenWrite(GetFullPath(path));
            await newStream.WriteAsync(buffer);
        }

        public async Task<MediaItem> SaveAsMediaItem(FileResult fileResult, string? tag = default)
        {
            var mediaItem = await CreateMediaItem(fileResult, tag);
            await _mediaItemStore.AddAsync(mediaItem);
            return mediaItem;
        }
        
        public async Task<MediaItem> CreateMediaItem(FileResult fileResult, string? tag = default)
        {
            var fileName = Path.GetFileName(fileResult.FileName);
            await using var stream = await fileResult.OpenReadAsync();
            var buffer = await stream.ToByteArrayAsync();
            var extension = Path.GetExtension(fileName);
            return await CreateMediaItem(buffer, extension, fileResult.ContentType);
        }
        
        public async Task<MediaItem> CreateMediaItem(byte[] buffer, string extension, string contentType, string? tag = default)
        {
            var mediaItem = new MediaItem
            {
                Id = Guid.NewGuid().ToString("N"),
                FileName = $"{Guid.NewGuid():N}{extension}",
                ContentType = contentType,
                Tag = tag
            };

            await SaveBufferAsync(buffer, mediaItem.FileName);
            return mediaItem;
        }

        public async Task<string?> GetMediaItemPathAsync(string mediaItemId)
        {
            var mediaItem = await _mediaItemStore.FindAsync(mediaItemId);
            return mediaItem == null ? null : GetFullPath(mediaItem.FileName);
        }

        public string GetMediaItemPath(MediaItem mediaItem) => GetFullPath(mediaItem.FileName);

        public Task<MediaItem?> GetMediaItemAsync(string mediaItemId) => _mediaItemStore.FindAsync(mediaItemId);

        public async Task DeleteManyByTagAsync(string tag)
        {
            var mediaItems = await _mediaItemStore.FindManyAsync(x => x.Tag!.Contains(tag));
            await DeleteManyAsync(mediaItems);
        }

        public async Task DeleteManyByIdAsync(IEnumerable<string> ids)
        {
            var idList = ids.ToList();
            var mediaItems = (await _mediaItemStore.FindManyAsync(x => idList.Contains(x.Id))).ToList();
            await DeleteManyAsync(mediaItems);
        }

        public async Task<string> GetImageDataUrlAsync(MediaItem mediaItem, CancellationToken cancellationToken = default)
        {
            var fullPath = GetFullPath(mediaItem.FileName);
            var bytes = await File.ReadAllBytesAsync(fullPath, cancellationToken);
            return bytes.GetDataUrl(mediaItem.ContentType);
        }
        
        public SKFileStream GetImageStream(MediaItem mediaItem)
        {
            var fullPath = GetFullPath(mediaItem.FileName);
            return new SKFileStream(fullPath);
        }

        public SKBitmap GetResizedImage(MediaItem mediaItem, int maxWidth)
        {
            var stream = GetImageStream(mediaItem);
            var bitmap = SKBitmap.Decode(stream);
            return bitmap.Resize(maxWidth);
        }

        private async Task DeleteManyAsync(IEnumerable<MediaItem> mediaItems)
        {
            var list = mediaItems.ToList();
            
            foreach (var mediaItem in list)
            {
                var fullPath = GetFullPath(mediaItem.FileName);
                TryDelete(fullPath);
            }

            await _mediaItemStore.DeleteManyAsync(list);
        }
        
        private void TryDelete(string path)
        {
            try
            {
                File.Delete(path);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
        
        private static string GetFullPath(string path) => Path.Combine(FileSystem.AppDataDirectory, path);
    }
}