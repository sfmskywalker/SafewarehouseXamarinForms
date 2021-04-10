using System.Collections.Generic;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using SafeWarehouseApp.Models;

namespace SafeWarehouseApp.Persistence.Stores
{
    public class ReportStore : EntityFrameworkStore<Report>
    {
        public ReportStore(IDbContextFactory<SafeWarehouseContext> factory, IMapper mapper) : base(factory, mapper)
        {
        }

        protected override void OnSaving(SafeWarehouseContext dbContext, Report entity)
        {
            var json = JsonConvert.SerializeObject(entity.Locations);
            dbContext.Entry(entity).Property("LocationsData").CurrentValue = json;
        }

        protected override void OnLoading(SafeWarehouseContext dbContext, Report entity)
        {
            var json = (string)dbContext.Entry(entity).Property("LocationsData").CurrentValue;
            entity.Locations = !string.IsNullOrWhiteSpace(json) ? JsonConvert.DeserializeObject<List<Location>>(json) : new List<Location>();
        }
    }
}