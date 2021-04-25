using System;
using System.Collections.Generic;

namespace SafeWarehouseApp.Models
{
    public class Report : Entity
    {
        public string CustomerId { get; set; } = default!;
        public DateTime Date { get; set; }
        public DateTime? NextExaminationBefore { get; set; }
        public string? Remarks { get; set; }
        public string SchematicMediaId { get; set; } = default!;
        public string? PaintedSchematicMediaId { get; set; }
        public IList<Location> Locations { get; set; } = new List<Location>();

        public void UpdateLocationNumbers()
        {
            var currentIndex = 0;
            foreach (var location in Locations)
                location.Number = ++currentIndex;
        }
    }
}