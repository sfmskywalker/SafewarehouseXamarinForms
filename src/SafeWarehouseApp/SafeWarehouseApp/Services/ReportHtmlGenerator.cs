using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using HandlebarsDotNet;
using SafeWarehouseApp.Extensions;
using SafeWarehouseApp.Models;
using SafeWarehouseApp.Persistence;

namespace SafeWarehouseApp.Services
{
    public class ReportHtmlGenerator : IReportHtmlGenerator
    {
        private readonly IStore<Customer> _customerStore;
        private readonly IStore<DamageType> _damageTypeStore;
        private readonly IStore<Material> _materialStore;
        private readonly IMediaService _mediaService;
        private readonly IPdfGenerator _pdfGenerator;

        public ReportHtmlGenerator(IStore<Customer> customerStore, IStore<DamageType> damageTypeStore, IStore<Material> materialStore, IMediaService mediaService, IPdfGenerator pdfGenerator)
        {
            _customerStore = customerStore;
            _damageTypeStore = damageTypeStore;
            _materialStore = materialStore;
            _mediaService = mediaService;
            _pdfGenerator = pdfGenerator;
        }

        public async Task<string> GenerateHtmlAsync(Report report, CancellationToken cancellationToken = default)
        {
            var template = await AssetLoader.ReadAssetStream("report.handlebars").ReadStringAsync(cancellationToken);
            var damageTypes = (await _damageTypeStore.ListAsync(cancellationToken: cancellationToken)).ToDictionary(x => x.Id);
            var customers = (await _customerStore.ListAsync(cancellationToken: cancellationToken)).ToDictionary(x => x.Id);
            var customer = customers.TryGet(report.CustomerId) ?? new Customer();
            var locations = report.Locations;
            var schematic = (await _mediaService.GetMediaItemAsync(report.SchematicMediaId))!;
            var materials = (await _materialStore.ListAsync(cancellationToken: cancellationToken)).ToDictionary(x => x.Id);

            var requiredMaterials = locations
                .SelectMany(location => location.Damages.SelectMany(damage => damage.RequiredMaterials))
                .GroupBy(x => x.MaterialId)
                .Select(x => new RequiredMaterial
                {
                    MaterialId = x.Key,
                    Quantity = x.Select(y => y.Quantity).Sum()
                }).ToList();

            var scale = 0.9f;
            
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
                Locations = await Task.WhenAll(locations.Select(async location =>
                {
                    var scaledRadius = (int) location.Radius * scale;
                    var scaledWidth = scaledRadius * 2;
                    var scaledHeight = scaledRadius * 2;
                    var scaledLeft = (int) ((location.Left - location.Radius) * scale);
                    var scaledTop = (int) ((location.Top - location.Radius) * scale);
                    
                    return new
                    {
                        Number = location.Number,
                        Description = location.Description,
                        Left = scaledLeft,
                        Top = scaledTop,
                        Width = scaledWidth,
                        Height = scaledHeight,
                        Damages = await Task.WhenAll(location.Damages.Select(async damage => new
                        {
                            Number = damage.Number,
                            DamageType = damageTypes.TryGet(damage.DamageTypeId)?.Name ?? "(onbekend)",
                            RequiredMaterials = damage.RequiredMaterials.Select(x => materials.TryGet(x.MaterialId)).Where(x => x != null).Select(x => x!.Name).ToList(),
                            DamagePictures = await Task.WhenAll(damage.Pictures.Select(async damagePicture => new
                            {
                                Number = damagePicture.Number,
                                Description = damagePicture.Description,
                                PictureData = await GetDataUrlAsync(damagePicture.PictureId)
                            }).ToList())
                        }).ToList())
                    };
                }).ToList()),
                RequiredMaterials = requiredMaterials.Select(requiredMaterial => new
                {
                    Material = materials.TryGet(requiredMaterial.MaterialId)?.Name ?? "(onbekend)",
                    Quantity = requiredMaterial.Quantity
                }).ToList()
            };

            var compiledTemplate = Handlebars.Compile(template);
            return compiledTemplate(model);
        }
        
        private async Task<string?> GetDataUrlAsync(string? mediaItemId)
        {
            var file = mediaItemId is not null and not "" ? await _mediaService.GetMediaItemAsync(mediaItemId) : default;
            return file != null ? await _mediaService.GetImageDataUrlAsync(file) : default;
        }
    }
}