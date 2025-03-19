using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using System.Collections.Generic;
using System.Data;
using API.Models;

[Route("api/BourbonMaster")]
[ApiController]
public class BourbonController : ControllerBase
{
    private readonly IDbConnection databaseConnection;

    public BourbonController(IDbConnection db)
    {
        databaseConnection = db;
        if (databaseConnection.State == ConnectionState.Closed)
        {
            databaseConnection.Open();
        }
    }

    // Retrieve active bourbons
    [HttpGet]
    public ActionResult<IEnumerable<Bourbon>> GetActiveBourbons()
    {
        var bourbons = new List<Bourbon>();

        using var command = databaseConnection.CreateCommand();
        command.CommandText = "SELECT BourbonID, Name, DistilleryID, MashBillID, Proof, FlavorNotes, Age, PhotoUrl, Deleted FROM Bourbon WHERE Deleted = 0";

        using var reader = command.ExecuteReader();
        while (reader.Read())
        {
            bourbons.Add(new Bourbon
            {
                BourbonID = reader.GetInt32(0),
                Name = reader.GetString(1),
                DistilleryID = reader.GetInt32(2),
                MashBillID = reader.IsDBNull(3) ? (int?)null : reader.GetInt32(3),
                Proof = reader.GetDecimal(4),
                FlavorNotes = reader.GetString(5),
                Age = reader.IsDBNull(6) ? (int?)null : reader.GetInt32(6),
                PhotoUrl = reader.IsDBNull(7) ? null : reader.GetString(7), // ✅ Includes PhotoUrl
                Deleted = reader.GetBoolean(8)
            });
        }
        return Ok(bourbons);
    }

    // Add a new Bourbon
    [HttpPost]
    public IActionResult AddBourbon([FromBody] Bourbon newBourbon)
    {
        using var command = databaseConnection.CreateCommand();
        command.CommandText = @"INSERT INTO Bourbon 
            (Name, DistilleryID, MashBillID, Proof, FlavorNotes, Age, PhotoUrl, Deleted) 
            VALUES (@name, @distilleryID, @mashBillID, @proof, @flavorNotes, @age, @photoUrl, 0)";

        command.Parameters.Add(new MySqlParameter("@name", newBourbon.Name));
        command.Parameters.Add(new MySqlParameter("@distilleryID", newBourbon.DistilleryID));
        command.Parameters.Add(new MySqlParameter("@mashBillID", (object)newBourbon.MashBillID ?? DBNull.Value));
        command.Parameters.Add(new MySqlParameter("@proof", newBourbon.Proof));
        command.Parameters.Add(new MySqlParameter("@flavorNotes", newBourbon.FlavorNotes));
        command.Parameters.Add(new MySqlParameter("@age", (object)newBourbon.Age ?? DBNull.Value));
        command.Parameters.Add(new MySqlParameter("@photoUrl", (object)newBourbon.PhotoUrl ?? DBNull.Value)); // ✅ Inserts PhotoUrl

        command.ExecuteNonQuery();
        return Ok();
    }

    // Soft delete a Bourbon
    [HttpDelete("{id}")]
    public IActionResult SoftDeleteBourbon(int id)
    {
        using var command = databaseConnection.CreateCommand();
        command.CommandText = "UPDATE Bourbon SET Deleted = 1 WHERE BourbonID = @id";
        command.Parameters.Add(new MySqlParameter("@id", id));

        command.ExecuteNonQuery();
        return NoContent();
    }

    // Retrieve deleted bourbons
    [HttpGet("deleted")]
    public ActionResult<IEnumerable<Bourbon>> GetDeletedBourbons()
    {
        var bourbons = new List<Bourbon>();

        using var command = databaseConnection.CreateCommand();
        command.CommandText = "SELECT BourbonID, Name, DistilleryID, MashBillID, Proof, FlavorNotes, Age, PhotoUrl, Deleted FROM Bourbon WHERE Deleted = 1";

        using var reader = command.ExecuteReader();
        while (reader.Read())
        {
            bourbons.Add(new Bourbon
            {
                BourbonID = reader.GetInt32(0),
                Name = reader.GetString(1),
                DistilleryID = reader.GetInt32(2),
                MashBillID = reader.IsDBNull(3) ? (int?)null : reader.GetInt32(3),
                Proof = reader.GetDecimal(4),
                FlavorNotes = reader.GetString(5),
                Age = reader.IsDBNull(6) ? (int?)null : reader.GetInt32(6),
                PhotoUrl = reader.IsDBNull(7) ? null : reader.GetString(7), // ✅ Includes PhotoUrl
                Deleted = reader.GetBoolean(8)
            });
        }
        return Ok(bourbons);
    }

    // Restore a deleted bourbon
    [HttpPut("restore/{id}")]
    public IActionResult RestoreBourbon(int id)
    {
        using var command = databaseConnection.CreateCommand();
        command.CommandText = "UPDATE Bourbon SET Deleted = 0 WHERE BourbonID = @id";
        command.Parameters.Add(new MySqlParameter("@id", id));

        command.ExecuteNonQuery();
        return NoContent();
    }

    [HttpGet("all")]
    public ActionResult<IEnumerable<Bourbon>> GetAllBourbons()
    {
        var bourbons = new List<Bourbon>();

        using var command = databaseConnection.CreateCommand();
        command.CommandText = "SELECT BourbonID, Name, DistilleryID, MashBillID, Proof, FlavorNotes, Age, PhotoUrl, Deleted FROM Bourbon WHERE Deleted = 0 ORDER BY BourbonID ASC";

        using var reader = command.ExecuteReader();
        while (reader.Read())
        {
            bourbons.Add(new Bourbon
            {
                BourbonID = reader.GetInt32(0),
                Name = reader.GetString(1),
                DistilleryID = reader.GetInt32(2),
                MashBillID = reader.IsDBNull(3) ? (int?)null : reader.GetInt32(3),
                Proof = reader.GetDecimal(4),
                FlavorNotes = reader.GetString(5),
                Age = reader.IsDBNull(6) ? (int?)null : reader.GetInt32(6),
                PhotoUrl = reader.IsDBNull(7) ? null : reader.GetString(7), // ✅ Includes PhotoUrl
                Deleted = reader.GetBoolean(8)
            });
        }
        return Ok(bourbons);
    }
}
