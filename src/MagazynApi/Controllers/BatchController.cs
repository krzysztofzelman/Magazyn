using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.Sqlite;

[ApiController]
[Route("api/[controller]")]
public class BatchController : ControllerBase
{
    private readonly IConfiguration _configuration;

    public BatchController(IConfiguration config)
    {
        _configuration = config;
    }

    [HttpGet]
    public List<Batch> GetBatches()
    {
        List<Batch> lista = new List<Batch>();
        string connectionString = _configuration.GetConnectionString("MagazynDb");

        using (SqliteConnection connection = new SqliteConnection(connectionString))
        {
            connection.Open();
            SqliteCommand command = connection.CreateCommand();
            command.CommandText = "SELECT Partie.Id, Partie.PrzedmiotId, Przedmioty.Nazwa, Partie.Ilosc," +
                " Partie.Cena, Partie.Data, Partie.Status, Partie.BatchNumber FROM Partie JOIN Przedmioty" +
                " ON Partie.PrzedmiotId = Przedmioty.Id";
            SqliteDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                lista.Add(new Batch
                {
                    Id = Convert.ToInt32(reader["Id"]),
                    ItemId = Convert.ToInt32(reader["PrzedmiotId"]),
                    Name = Convert.ToString(reader["Nazwa"]) ?? "",
                    Quantity = Convert.ToInt32(reader["Ilosc"]),
                    Price = Convert.ToDecimal(reader["Cena"]),
                    Date = Convert.ToString(reader["Data"]),
                    Status = Convert.ToString(reader["Status"]),
                    BatchNumber = Convert.ToString(reader["BatchNumber"]) ?? ""
                });
            }
        }
        return lista;
    }
    [HttpPost]
    public void AddBatch(Batch batch)
    {
        string connectionString = _configuration.GetConnectionString("MagazynDb");
        using (SqliteConnection connection = new SqliteConnection(connectionString))
        {
            connection.Open();
            SqliteCommand command = connection.CreateCommand();
            command.CommandText = "INSERT INTO Partie (PrzedmiotId, Ilosc, Cena, Data, Status, BatchNumber)" +
                " VALUES (@PrzedmiotId, @Ilosc, @Cena, @Data, @Status, @BatchNumber)";
            command.Parameters.AddWithValue("@PrzedmiotId", batch.ItemId);
            command.Parameters.AddWithValue("@Ilosc", batch.Quantity);
            command.Parameters.AddWithValue("@Cena", batch.Price);
            command.Parameters.AddWithValue("@Data", batch.Date);
            command.Parameters.AddWithValue("@Status", batch.Status);
            command.Parameters.AddWithValue("@BatchNumber", batch.BatchNumber);
            command.ExecuteNonQuery();
        }
    }
    [HttpPut("{id}")]
    public void UpdateBatch(int id, Batch batch)
    {
        string connectionString = _configuration.GetConnectionString("MagazynDb");
        using (SqliteConnection connection = new SqliteConnection(connectionString))
        {
            connection.Open();
            SqliteCommand command = connection.CreateCommand();
            command.CommandText = "UPDATE Partie SET PrzedmiotId=@PrzedmiotId, Ilosc=@Ilosc, Cena=@Cena, Data=@Data, Status=@Status, BatchNumber=@BatchNumber WHERE Id=@Id";
            command.Parameters.AddWithValue("@Id", id);
            command.Parameters.AddWithValue("@PrzedmiotId", batch.ItemId);
            command.Parameters.AddWithValue("@Ilosc", batch.Quantity);
            command.Parameters.AddWithValue("@Cena", batch.Price);
            command.Parameters.AddWithValue("@Data", batch.Date);
            command.Parameters.AddWithValue("@Status", batch.Status);
            command.Parameters.AddWithValue("@BatchNumber", batch.BatchNumber);
            command.ExecuteNonQuery();
        }
    }
    [HttpDelete("{id}")]
    public void DeleteBatch(int id)
    {
        string connectionString = _configuration.GetConnectionString("MagazynDb");
        using (SqliteConnection connection = new SqliteConnection(connectionString))
        {
            connection.Open();
            SqliteCommand command = connection.CreateCommand();
            command.CommandText = "DELETE FROM Partie WHERE Id=@Id";
            command.Parameters.AddWithValue("@Id", id);
            command.ExecuteNonQuery();
        }
    }
    [HttpPost("wydanie")]
    public IActionResult Wydanie(DispatchRequest request)
    {
        string connectionString = _configuration.GetConnectionString("MagazynDb");

        // 1. parking: przeczytaj partie artykułu (najstarsza pierwsza)
        List<int> batchIds = new List<int>();
        List<int> batchSizes = new List<int>();
        using (SqliteConnection connection = new SqliteConnection(connectionString))
        {
            connection.Open();
            SqliteCommand command = connection.CreateCommand();
            command.CommandText = "SELECT Id, Ilosc FROM Partie WHERE PrzedmiotId = @itemId AND Ilosc > 0 ORDER BY Id ASC";
            command.Parameters.AddWithValue("@itemId", request.ItemId);
            SqliteDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                batchIds.Add(Convert.ToInt32(reader["Id"]));
                batchSizes.Add(Convert.ToInt32(reader["Ilosc"]));
            }
        }

        // 2. sprawdź, czy w ogóle jest dość towaru
        int total = 0;
        foreach (int size in batchSizes) total += size;
        if (total < request.Quantity)
            return BadRequest("Za mało towaru w magazynie");

        // 3. wydaj z najstarszych partii (FIFO)
        int remaining = request.Quantity;
        using (SqliteConnection connection = new SqliteConnection(connectionString))
        {
            connection.Open();
            for (int i = 0; i < batchIds.Count && remaining > 0; i++)
            {
                if (batchSizes[i] <= remaining)
                {
                    SqliteCommand command = connection.CreateCommand();
                    command.CommandText = "UPDATE Partie SET Ilosc = 0, Status = 'Wydane' WHERE Id = @batchId";
                    command.Parameters.AddWithValue("@batchId", batchIds[i]);
                    command.ExecuteNonQuery();
                    remaining -= batchSizes[i];
                }
                else
                {

                    SqliteCommand command = connection.CreateCommand();
                    command.CommandText = "UPDATE Partie SET Ilosc = Ilosc - @take WHERE Id = @batchId";
                    command.Parameters.AddWithValue("@take", remaining);
                    command.Parameters.AddWithValue("@batchId", batchIds[i]);
                    command.ExecuteNonQuery();
                    remaining = 0;
                }
            }
        }

        // 4. zmniejsz stan artykułu
        using (SqliteConnection connection = new SqliteConnection(connectionString))
        {
            connection.Open();
            SqliteCommand command = connection.CreateCommand();
            command.CommandText = "UPDATE Przedmioty SET Ilosc = Ilosc - @quantity WHERE Id = @itemId";
            command.Parameters.AddWithValue("@quantity", request.Quantity);
            command.Parameters.AddWithValue("@itemId", request.ItemId);
            command.ExecuteNonQuery();
        }

        return Ok("Wydano towar");
    }
}