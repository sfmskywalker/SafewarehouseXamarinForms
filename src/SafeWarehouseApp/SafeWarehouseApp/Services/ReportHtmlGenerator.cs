using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using HandlebarsDotNet;
using SafeWarehouseApp.Extensions;
using SafeWarehouseApp.Models;
using SafeWarehouseApp.Persistence;
using SkiaSharp;

namespace SafeWarehouseApp.Services
{
    public class ReportHtmlGenerator : IReportHtmlGenerator
    {
        private readonly IStore<Customer> _customerStore;
        private readonly IStore<DamageType> _damageTypeStore;
        private readonly IStore<Material> _materialStore;
        private readonly IMediaService _mediaService;

        public ReportHtmlGenerator(IStore<Customer> customerStore, IStore<DamageType> damageTypeStore, IStore<Material> materialStore, IMediaService mediaService)
        {
            _customerStore = customerStore;
            _damageTypeStore = damageTypeStore;
            _materialStore = materialStore;
            _mediaService = mediaService;
        }

        public async Task<string> GenerateHtmlAsync(Report report, CancellationToken cancellationToken = default)
        {
            var template = await AssetLoader.ReadAssetStream("report.handlebars").ReadStringAsync(cancellationToken);
            var damageTypes = (await _damageTypeStore.ListAsync(cancellationToken: cancellationToken)).ToDictionary(x => x.Id);
            var customers = (await _customerStore.ListAsync(cancellationToken: cancellationToken)).ToDictionary(x => x.Id);
            var customer = customers.TryGet(report.CustomerId) ?? new Customer();
            var locations = report.Locations;
            var schematic = (await _mediaService.GetMediaItemAsync(report.PaintedSchematicMediaId!))!;
            var materials = (await _materialStore.ListAsync(cancellationToken: cancellationToken)).ToDictionary(x => x.Id);

            var requiredMaterials = locations
                .SelectMany(location => location.Damages.SelectMany(damage => damage.RequiredMaterials))
                .GroupBy(x => x.MaterialId)
                .Select(x => new RequiredMaterial
                {
                    MaterialId = x.Key,
                    Quantity = x.Select(y => y.Quantity).Sum()
                }).ToList();

            var model = new
            {
                CompanyName = customer.CompanyName,
                ReportDate = DateTime.Now.ToString("dd-MM-yyyy"),
                ContactName = customer.ContactName,
                City = customer.City,
                Address = customer.Address,
                NextExaminationBefore = report.NextExaminationBefore?.ToString("dd-MM-yyyy"),
                Remarks = report.Remarks,
                SchematicData = await _mediaService.GetImageDataUrlAsync(schematic, cancellationToken),
                Locations = await Task.WhenAll(locations.Select(async (location, locationIndex) =>
                {
                    return new
                    {
                        Number = locationIndex + 1,
                        Description = location.Description,
                        Damages = await Task.WhenAll(location.Damages.Select(async (damage, damageIndex) =>
                        {
                            var damagePictures = await Task.WhenAll(damage.Pictures.Select(async (damagePicture, damagePictureIndex) => new
                            {
                                Number = damagePictureIndex + 1,
                                Description = damagePicture.Description,
                                PictureData = await GetDataUrlAsync(damagePicture.PictureId, 300)
                            }).ToList());

                            var mainDamagePicture = damagePictures.FirstOrDefault();
                            
                            return new
                            {
                                Number = damageIndex + 1,
                                DamageType = damageTypes.TryGet(damage.DamageTypeId)?.Name ?? "(onbekend)",
                                RequiredMaterials = damage.RequiredMaterials.Where(x => !string.IsNullOrEmpty(x.MaterialId)).Select(x => materials.TryGet(x.MaterialId)).Where(x => x != null).Select(x => x!.Name).ToList(),
                                MainDamagePicture = mainDamagePicture,
                                DamagePictures = damagePictures
                            };
                        }).ToList())
                    };
                }).ToList()),
                RequiredMaterials = requiredMaterials.Where(x => !string.IsNullOrEmpty(x.MaterialId)).Select(requiredMaterial => new
                {
                    Material = materials.TryGet(requiredMaterial.MaterialId)?.Name ?? "(onbekend)",
                    Quantity = requiredMaterial.Quantity
                }).ToList()
            };

            var compiledTemplate = Handlebars.Compile(template);
            return compiledTemplate(model);
        }
        
        private async Task<string?> GetDataUrlAsync(string? mediaItemId, int maxWidth)
        {
            var mediaItem = mediaItemId is not null and not "" ? await _mediaService.GetMediaItemAsync(mediaItemId) : default;

            if (mediaItem == null)
                return null;
            
            var scaledBitmap = _mediaService.GetResizedImage(mediaItem, maxWidth);
            var data = scaledBitmap.Encode(SKEncodedImageFormat.Jpeg, 100);
            return data.ToArray().GetDataUrl(mediaItem.ContentType);
        }
    }
}