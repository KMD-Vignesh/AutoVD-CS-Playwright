using System.Collections.Generic;

namespace AutoVD.Framework.Models
{
    public class TestDataFile
    {
        public List<User> Users { get; set; } = null!;
        public List<Product> Products { get; set; } = null!;
    }

    public class User
    {
        public string Name { get; set; } = null!;
        public string Username { get; set; } = null!;
        public string Password { get; set; } = null!;
        public bool IsValid { get; set; }
    }

    public class Product
    {
        public string Name { get; set; } = null!;
        public string Price { get; set; } = null!;
    }
}
