using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using System.Collections.Generic;
using System.Data;
using API.Models;

[Route("api/Distilleries")]
[ApiController]
public class DistilleriesController : ControllerBase
{
    private readonly MySqlConnection databaseConnection;

    public DistilleriesController(MySqlConnection dbConnection)
    {
        databaseConnection = dbConnection;
        if (databaseConnection.State == ConnectionState.Closed)
        {
            databaseConnection.Open();
        }
    }

    // Retrieve active distilleries (not deleted)
    [HttpGet]
    public ActionResult<IEnumerable<Distillery>> GetActiveDistilleries()
    {
        var distilleries = new List<Distillery>();

        using var command = databaseConnection.CreateCommand();
        command.CommandText = "SELECT * FROM Distillery WHERE Deleted = 0";

        using var reader = command.ExecuteReader();
        while (reader.Read())
        {
            distilleries.Add(new Distillery
            {
                DistilleryID = reader.GetInt32(0),
                Name = reader.GetString(1),
                Location = reader.IsDBNull(2) ? null : reader.GetString(2),
                EstablishedDate = reader.IsDBNull(3) ? (DateTime?)null : reader.GetDateTime(3),
                Description = reader.IsDBNull(4) ? null : reader.GetString(4),
                Deleted = reader.GetBoolean(5)
            });
        }
        return Ok(distilleries);
    }

    [HttpPost]
    public IActionResult AddDistillery([FromBody] Distillery newDistillery)
    {
        using var command = databaseConnection.CreateCommand();
        command.CommandText = @"INSERT INTO Distillery 
        (Name, Location, EstablishedDate, Description, Deleted) 
        VALUES (@name, @location, @establishedDate, @description, 0)";

        command.Parameters.Add(new MySqlParameter("@name", newDistillery.Name));
        command.Parameters.Add(new MySqlParameter("@location", newDistillery.Location));
        command.Parameters.Add(new MySqlParameter("@establishedDate", (object)newDistillery.EstablishedDate ?? DBNull.Value));
        command.Parameters.Add(new MySqlParameter("@description", newDistillery.Description));

        command.ExecuteNonQuery();
        return Ok();
    }

    [HttpDelete("{id}")]
    public IActionResult DeleteDistillery(int id)
    {
        using var command = databaseConnection.CreateCommand();
        command.CommandText = "UPDATE Distillery SET Deleted = 1 WHERE DistilleryID = @id";
        command.Parameters.Add(new MySqlParameter("@id", id));

        command.ExecuteNonQuery();
        return NoContent();
    }

    [HttpGet("deleted")]
    public ActionResult<IEnumerable<Distillery>> GetDeletedDistilleries()
    {
        var distilleries = new List<Distillery>();

        using var command = databaseConnection.CreateCommand();
        command.CommandText = "SELECT * FROM Distillery WHERE Deleted = 1";

        using var reader = command.ExecuteReader();
        while (reader.Read())
        {
            distilleries.Add(new Distillery
            {
                DistilleryID = reader.GetInt32(0),
                Name = reader.GetString(1),
                Location = reader.IsDBNull(2) ? null : reader.GetString(2),
                EstablishedDate = reader.IsDBNull(3) ? (DateTime?)null : reader.GetDateTime(3),
                Description = reader.IsDBNull(4) ? null : reader.GetString(4),
                Deleted = reader.GetBoolean(5)
            });
        }
        return Ok(distilleries);
    }

    [HttpPut("restore/{id}")]
    public IActionResult RestoreDistillery(int id)
    {
        using var command = databaseConnection.CreateCommand();
        command.CommandText = "UPDATE Distillery SET Deleted = 0 WHERE DistilleryID = @id";
        command.Parameters.Add(new MySqlParameter("@id", id));

        command.ExecuteNonQuery();
        return NoContent();
    }
}
