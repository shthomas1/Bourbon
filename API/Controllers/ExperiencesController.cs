using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MySql.Data.MySqlClient;
using System.Data;
using API.Models;

namespace API.Controllers
{
    [Route("api/Experiences")]
    [ApiController]
    public class ExperiencesController : ControllerBase
    {
        private readonly MySqlConnection databaseConnection;

        public ExperiencesController(MySqlConnection dbConnection)
        {
            databaseConnection = dbConnection;
            if (databaseConnection.State == ConnectionState.Closed)
            {
                databaseConnection.Open();
            }
        }

        [HttpPost]
        public IActionResult AddExperience([FromBody] Experience experience)
        {
            using var command = databaseConnection.CreateCommand();
            command.CommandText = @"
                INSERT INTO Experiences (BourbonID, UserID, Review, Rating, DateAdded) 
                VALUES (@bourbonId, @userId, @review, @rating, NOW())";

            command.Parameters.Add(new MySqlParameter("@bourbonId", experience.BourbonID));
            command.Parameters.Add(new MySqlParameter("@userId", experience.UserID));
            command.Parameters.Add(new MySqlParameter("@review", experience.Review));
            command.Parameters.Add(new MySqlParameter("@rating", experience.Rating));

            command.ExecuteNonQuery();
            return Ok();
        }

        [HttpGet("{bourbonId}")]
        public ActionResult<IEnumerable<Experience>> GetExperiencesByBourbon(int bourbonId)
        {
            var experiences = new List<Experience>();

            using var command = databaseConnection.CreateCommand();
            command.CommandText = "SELECT * FROM Bourbons WHERE BourbonID = @bourbonId";
            command.Parameters.Add(new MySqlParameter("@bourbonId", bourbonId));

            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                experiences.Add(new Experience
                {
                    ExperienceID = reader.GetInt32(0),
                    BourbonID = reader.GetInt32(1),
                    UserID = reader.GetString(2),
                    Review = reader.GetString(3),
                    Rating = reader.GetInt32(4),
                    DateAdded = reader.GetDateTime(5)
                });
            }
            return Ok(experiences);
        }
    }

}