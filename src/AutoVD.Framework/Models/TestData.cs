using System.Collections.Generic;

namespace AutoVD.Framework.Models
{
    public class TestDataFile
    {
        public List<User> Users { get; set; }
        public List<Product> Products { get; set; }
    }

    public class User
    {
        public string Name { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public bool IsValid { get; set; }
    }

    public class Product
    {
        public string Name { get; set; }
        public string Price { get; set; }
    }
}
