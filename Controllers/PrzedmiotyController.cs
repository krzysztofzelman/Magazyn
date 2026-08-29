using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.Sqlite;

[ApiController]
[Route("api/[controller]")]
public class PrzedmiotyController : ControllerBase
{
    private readonly IConfiguration _config;

    public PrzedmiotyController(IConfiguration config)
    {
        _config = config;
    }

    [HttpGet]
    public List<Item> GetPrzedmioty() // GET: api/przedmioty
    {
        List<Item> lista = new List<Item>();
        string connectionString = _config.GetConnectionString("MagazynDb");

        using (SqliteConnection connection = new SqliteConnection(connectionString))
        {
            connection.Open();
            SqliteCommand command = connection.CreateCommand();
            command.CommandText = "SELECT Id, Nazwa, Ilosc, Cena FROM Przedmioty";
            SqliteDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                lista.Add(new Item
                {
                    Id = Convert.ToInt32(reader["Id"]),
                    Name = Convert.ToString(reader["Nazwa"]) ?? "",
                    Quantity = Convert.ToInt32(reader["Ilosc"]),
                    Price = Convert.ToDecimal(reader["Cena"])
                });
            }
        }
        return lista;
    }
    [HttpPost]
    public void DodajPrzedmiot(Item nowy) // POST: api/przedmioty
    {
        string connectionString = _config.GetConnectionString("MagazynDb");
        using (SqliteConnection connection = new SqliteConnection(connectionString))
        {
            connection.Open();
            SqliteCommand command = connection.CreateCommand();
            command.CommandText = "INSERT INTO Przedmioty (Nazwa, Ilosc, Cena) VALUES (@Nazwa, @Ilosc, @Cena)";
            command.Parameters.AddWithValue("@Nazwa", nowy.Name);
            command.Parameters.AddWithValue("@Ilosc", nowy.Quantity);
            command.Parameters.AddWithValue("@Cena", nowy.Price);
            command.ExecuteNonQuery();
        }
    }

    [HttpPut("{id}")]
    public void EdytujPrzedmiot(int id, Item nowy) // PUT: api/przedmioty/5
    {
        string connectionString = _config.GetConnectionString("MagazynDb");

        using (SqliteConnection connection = new SqliteConnection(connectionString))
        {
            connection.Open();
            SqliteCommand command = connection.CreateCommand();
            command.CommandText = "UPDATE Przedmioty SET Nazwa =@Nazwa, Ilosc = @Ilosc, Cena = @Cena WHERE Id = @Id";
            command.Parameters.AddWithValue("@Id", id);
            command.Parameters.AddWithValue("@Nazwa", nowy.Name);
            command.Parameters.AddWithValue("@Ilosc", nowy.Quantity);
            command.Parameters.AddWithValue("@Cena", nowy.Price);
            command.ExecuteNonQuery();
        }
    }

    [HttpDelete("{id}")]
    public void UsunPrzedmiot(int id)
    {
        string connectionString = _config.GetConnectionString("MagazynDb");
        using (SqliteConnection connection = new SqliteConnection(connectionString))
        {
            connection.Open();
            SqliteCommand command = connection.CreateCommand();
            command.CommandText = "DELETE FROM Przedmioty WHERE Id = @Id";
            command.Parameters.AddWithValue("@Id", id);
            command.ExecuteNonQuery();
        }
    }
}
