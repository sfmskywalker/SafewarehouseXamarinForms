using System.Collections.Generic;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using SafeWarehouseApp.Models;

namespace SafeWarehouseApp.Persistence.Stores
{
    public class CustomerStore : EntityFrameworkStore<Customer>
    {
        public CustomerStore(IDbContextFactory<SafeWarehouseContext> factory, IMapper mapper) : base(factory, mapper)
        {
        }

        protected override void OnSaving(SafeWarehouseContext dbContext, Customer entity)
        {
            dbContext.Entry(entity).Property("SuppliersData").CurrentValue = string.Join(",", entity.Suppliers);
        }

        protected override void OnLoading(SafeWarehouseContext dbContext, Customer entity)
        {
            var suppliersValue = (string)dbContext.Entry(entity).Property("SuppliersData").CurrentValue;
            entity.Suppliers = !string.IsNullOrWhiteSpace(suppliersValue) ? suppliersValue.Split(',') : new List<string>();
        }
    }
}