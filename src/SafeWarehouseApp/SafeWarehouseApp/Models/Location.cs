using System.Collections.Generic;

namespace SafeWarehouseApp.Models
{
    public class Location : Entity
    {
        public int Number { get; set; }
        public string? Description { get; set; }
        public ICollection<Damage> Damages { get; set; } = new List<Damage>();
        public float Left { get; set; }
        public float Top { get; set; }
        public float Radius { get; set; }

        public void UpdateDamageNumbers()
        {
            var currentIndex = 0;
            foreach (var damage in Damages)
                damage.Number = ++currentIndex;
        }
    }
}