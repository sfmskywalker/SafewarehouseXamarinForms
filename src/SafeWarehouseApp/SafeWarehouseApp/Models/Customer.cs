using System.Collections.Generic;

namespace SafeWarehouseApp.Models
{
    public class Customer : Entity
    {
        public string CompanyName { get; set; } = default!;
        public string? ContactName { get; set; }
        public string Email { get; set; }= default!;
        public string? City { get; set; }
        public string? Address { get; set; }
        public ICollection<string> Suppliers { get; set; } = new List<string>();
    }
}