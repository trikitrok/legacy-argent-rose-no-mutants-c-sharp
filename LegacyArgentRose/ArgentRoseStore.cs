using System.Data;
using Microsoft.Data.SqlClient;

namespace ArgentRose;

public class ArgentRoseStore
{
    private const string DatabaseName = "inventory";
    private const string User = "user";
    private const string Pass = "123456";

    private List<Product> _products;

    public ArgentRoseStore()
    {
        _products = new List<Product>();
    }

    public void Update()
    {
        GetInventory();
        UpdateInventory();
        SaveInventory();
    }

    private void UpdateInventory()
    {
        _products.ForEach(UpdateProduct);
    }

    private void UpdateProduct(Product product)
    {
        product.DecreaseSellIn();
        UpdateQuality(product);
    }

    private void UpdateQuality(Product product)
    {
        if (product.IsLanzaroteWine())
        {
            UpdateLanzaroteWineQuality(product);
        }
        else if (product.IsTheatrePasses())
        {
            UpdateTheatrePassesQuality(product);
        }
        else
        {
            UpdateRegularProductQuality(product);
        }
    }

    private void UpdateRegularProductQuality(Product product)
    {
        product.DecreaseQualityBy(2);
    }

    private void UpdateTheatrePassesQuality(Product product)
    {
        if (product.IsExpired())
        {
            product.DropQualityToMinimum();
        }
        else if (product.SellInIsLessThan(5))
        {
            product.IncreaseQualityBy(3);
        }
        else
        {
            product.IncreaseQualityBy(1);
        }
    }

    private void UpdateLanzaroteWineQuality(Product product)
    {
        product.IncreaseQualityBy(2);
    }

    private void GetInventory()
    {
        _products = GetInventoryFromDb();
    }

    protected virtual List<Product> GetInventoryFromDb()
    {
        var products = new List<Product>();
        
        var connectionString = GetConnectionString();

        using (var connection = new SqlConnection(connectionString))
        {
            try
            {
                connection.Open();

                using (var command = new SqlCommand("SELECT sellIn, quality, description FROM Products", connection))
                {
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var sellIn = reader.GetInt32(reader.GetOrdinal("sellIn"));
                            var quality = reader.GetInt32(reader.GetOrdinal("quality"));
                            var description = reader.GetString(reader.GetOrdinal("description"));

                            var product = new Product(description, sellIn, quality);
                            products.Add(product);
                        }
                    }
                }
            }
            catch (SqlException e)
            {
                Console.WriteLine(e.Message);
            }
        }

        return products;
    }

    private void SaveInventory()
    {
        SaveInventory(_products);
    }

    protected virtual void SaveInventory(List<Product> inventory)
    {
        var connectionString = GetConnectionString();

        using (var connection = new SqlConnection(connectionString))
        {
            connection.Open();

            using (var transaction = connection.BeginTransaction())
            {
                try
                {
                    foreach (var product in inventory)
                    {
                        using (var command =
                               new SqlCommand(
                                   "INSERT INTO Products (sellIn, quality, description) VALUES (@sellIn, @quality, @description)",
                                   connection, transaction))
                        {
                            command.Parameters.Add(
                                new SqlParameter("@sellIn", SqlDbType.Int) { Value = product.SellIn });
                            command.Parameters.Add(new SqlParameter("@quality", SqlDbType.Int)
                                { Value = product.Quality });
                            command.Parameters.Add(new SqlParameter("@description", SqlDbType.NVarChar, 255)
                                { Value = product.Description });

                            command.ExecuteNonQuery();
                        }
                    }

                    transaction.Commit();
                    Console.WriteLine("Products saved successfully.");
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    Console.WriteLine("An error occurred while saving inventory: " + ex.Message);
                }
            }
        }
    }

    private static string GetConnectionString()
    {
        return $"Server=localhost;Database={DatabaseName};User Id={User};Password={Pass};";
    }
}