using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using System.Collections.Generic;
using System.Data;
using API.Models;

[Route("api/MashBills")]
[ApiController]
public class MashBillsController : ControllerBase
{
    private readonly MySqlConnection databaseConnection;

    public MashBillsController(MySqlConnection dbConnection)
    {
        databaseConnection = dbConnection;
        if (databaseConnection.State == ConnectionState.Closed)
        {
            databaseConnection.Open();
        }
    }

    // Retrieve active MashBills (not deleted)
    [HttpGet]
    public ActionResult<IEnumerable<MashBill>> GetActiveMashBills()
    {
        var mashBills = new List<MashBill>();

        using var command = databaseConnection.CreateCommand();
        command.CommandText = "SELECT * FROM MashBill WHERE Deleted = 0";

        using var reader = command.ExecuteReader();
        while (reader.Read())
        {
            mashBills.Add(new MashBill
            {
                MashBillID = reader.GetInt32(0),
                Name = reader.GetString(1),
                CornPercentage = reader.GetDecimal(2),
                RyePercentage = reader.GetDecimal(3),
                BarleyPercentage = reader.GetDecimal(4),
                WheatPercentage = reader.IsDBNull(5) ? (decimal?)null : reader.GetDecimal(5),
                Deleted = reader.GetBoolean(6)
            });
        }
        return Ok(mashBills);
    }

    [HttpGet("{bourbonId}")]
    public ActionResult<MashBill> GetMashBillByBourbon(int bourbonId)
    {
        using var command = databaseConnection.CreateCommand();
        command.CommandText = @"
            SELECT mb.* FROM MashBill mb
            JOIN BourbonMaster b ON mb.MashBillID = b.MashBillID
            WHERE b.BourbonID = @bourbonId";

        command.Parameters.Add(new MySqlParameter("@bourbonId", bourbonId));

        using var reader = command.ExecuteReader();
        if (reader.Read())
        {
            var mashBill = new MashBill
            {
                MashBillID = reader.GetInt32(0),
                Name = reader.GetString(1),
                CornPercentage = reader.GetDecimal(2),
                RyePercentage = reader.GetDecimal(3),
                BarleyPercentage = reader.GetDecimal(4),
                WheatPercentage = reader.IsDBNull(5) ? (decimal?)null : reader.GetDecimal(5),
                Deleted = reader.GetBoolean(6)
            };
            return Ok(mashBill);
        }

        return NotFound();
    }


    [HttpPost]
    public IActionResult AddMashBill([FromBody] MashBill newMashBill)
    {
        using var command = databaseConnection.CreateCommand();
        command.CommandText = @"INSERT INTO MashBill 
        (Name, CornPercentage, RyePercentage, BarleyPercentage, WheatPercentage, Deleted) 
        VALUES (@name, @corn, @rye, @barley, @wheat, 0)";

        command.Parameters.Add(new MySqlParameter("@name", newMashBill.Name));
        command.Parameters.Add(new MySqlParameter("@corn", newMashBill.CornPercentage));
        command.Parameters.Add(new MySqlParameter("@rye", newMashBill.RyePercentage));
        command.Parameters.Add(new MySqlParameter("@barley", newMashBill.BarleyPercentage));
        command.Parameters.Add(new MySqlParameter("@wheat", (object)newMashBill.WheatPercentage ?? DBNull.Value));

        command.ExecuteNonQuery();
        return Ok();
    }

    [HttpDelete("{id}")]
    public IActionResult DeleteMashBill(int id)
    {
        using var command = databaseConnection.CreateCommand();
        command.CommandText = "UPDATE MashBill SET Deleted = 1 WHERE MashBillID = @id";
        command.Parameters.Add(new MySqlParameter("@id", id));

        command.ExecuteNonQuery();
        return NoContent();
    }

    [HttpGet("deleted")]
    public ActionResult<IEnumerable<MashBill>> GetDeletedMashBills()
    {
        var mashBills = new List<MashBill>();

        using var command = databaseConnection.CreateCommand();
        command.CommandText = "SELECT * FROM MashBill WHERE Deleted = 1";

        using var reader = command.ExecuteReader();
        while (reader.Read())
        {
            mashBills.Add(new MashBill
            {
                MashBillID = reader.GetInt32(0),
                Name = reader.GetString(1),
                CornPercentage = reader.GetDecimal(2),
                RyePercentage = reader.GetDecimal(3),
                BarleyPercentage = reader.GetDecimal(4),
                WheatPercentage = reader.IsDBNull(5) ? (decimal?)null : reader.GetDecimal(5),
                Deleted = reader.GetBoolean(6)
            });
        }
        return Ok(mashBills);
    }

    [HttpPut("restore/{id}")]
    public IActionResult RestoreMashBill(int id)
    {
        using var command = databaseConnection.CreateCommand();
        command.CommandText = "UPDATE MashBill SET Deleted = 0 WHERE MashBillID = @id";
        command.Parameters.Add(new MySqlParameter("@id", id));

        command.ExecuteNonQuery();
        return NoContent();
    }
}
