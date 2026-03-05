using Npgsql;
using Healthify.Data;
using Healthify.ViewModels;

public class HomeRepository
{
    private readonly DbConnectionFactory _dbFactory;

        public HomeRepository(DbConnectionFactory dbFactory)
        {
            _dbFactory = dbFactory;
        }

        public List<ProductListViewModel> GetAllProducts()
        {
            var products = new List<ProductListViewModel>();
            using (NpgsqlConnection connection = _dbFactory.CreateConnection())
            {
                connection.Open();
                string query = "SELECT DISTINCT ON (p.id) p.id, p.name, p.description, p.price, p.stock, pi.image_url FROM products p LEFT JOIN product_images pi ON p.id = pi.product_id AND pi.is_active = true ORDER BY p.id DESC limit 8";

                using (var cmd = new NpgsqlCommand(query, connection)){
                    using (var reader = cmd.ExecuteReader()){
                        while (reader.Read())
                        {
                            
                            products.Add(new ProductListViewModel
                            {
                                Id = Convert.ToInt32(reader["id"]),
                                Name = reader["name"].ToString(),
                                Description = reader["description"]?.ToString(),
                                Price = Convert.ToDecimal(reader["price"]),
                                Stock = Convert.ToInt32(reader["stock"]),
                                ImageUrl = reader["image_url"]?.ToString()
                            });
                        }
                    }
                }
            }
            return products;
        }

        public List<CategoryViewModel> GetAllCategories()
        {
           List<CategoryViewModel> categories = new List<CategoryViewModel>();

            using (NpgsqlConnection connection = _dbFactory.CreateConnection())
            {
                connection.Open();

                string query = "SELECT id, name, description, image FROM categories";

                using (var cmd = new NpgsqlCommand(query, connection))
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        categories.Add(new CategoryViewModel
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("id")),
                            Name = reader["name"].ToString(),
                            Description = reader["description"]?.ToString(),
                            ImageUrl = reader["image"]?.ToString()
                        });
                    }
                }
            }
            return categories; 
        }

        

}