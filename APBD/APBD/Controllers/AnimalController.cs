using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using APBD.Models;
using APBD.Models.DTOs;

namespace APBD.Controllers;

[ApiController]
// [Route("api/animals")]
[Route("api/[controller]")]
public class AnimalsController : ControllerBase
{
    private readonly IConfiguration _configuration;
    public AnimalsController(IConfiguration configuration)
    {
        _configuration = configuration;
    }
    
    [HttpGet]
    public IActionResult GetAnimals([FromQuery] string orderBy = "name")
    {
        // Otwieramy połączenie
        using SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("Default"));
        connection.Open();
        
        // Definiujemy commanda
        using SqlCommand command = new SqlCommand();
        command.Connection = connection;
        
        string columnName = orderBy.ToLower() switch
        {
            "description" => "Description",
            "category" => "Category",
            "area" => "Area",
            _ => "Name"
        };
        
        command.CommandText = $"SELECT * FROM Animal ORDER BY {columnName}";
        
        // Wykonanie commanda
        var reader = command.ExecuteReader();
        
        List<Animal> animals = new List<Animal>();

        int idAnimalOrdinal = reader.GetOrdinal("IdAnimal");
        int nameOrdinal = reader.GetOrdinal("Name");

        while (reader.Read())
        {
            animals.Add(new Animal()
            {
                IdAnimal = reader.GetInt32(idAnimalOrdinal),
                Name = reader.GetString(nameOrdinal)
            });
        }
        
        return Ok(animals);
    }

    [HttpPost]
    public IActionResult AddAnimal(AddAnimal animal)
    {
        // Otwieramy połączenie
        using SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("Default"));
        connection.Open();
        
        // Definiujemy commanda
        using SqlCommand command = new SqlCommand();
        command.Connection = connection;
        command.CommandText = $"INSERT INTO Animal (IdAnimal, Name, Description, Category, Area) VALUES (@IdAnimal, @Name, @Description, @Category, @Area)";
        command.Parameters.AddWithValue("@IdAnimal", animal.IdAnimal);
        command.Parameters.AddWithValue("@Name", animal.Name);
        command.Parameters.AddWithValue("@Description", animal.Description);
        command.Parameters.AddWithValue("@Category", animal.Category);
        command.Parameters.AddWithValue("@Area", animal.Area);

        command.ExecuteNonQuery();
        
        return Created("", null);
    }
    
    [HttpPut("{idAnimal}")]
    public IActionResult UpdateAnimal(int idAnimal, UpdateAnimal animal)
    {
        // Otwieramy połączenie
        using SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("Default"));
        connection.Open();
    
        // Definiujemy commanda
        using SqlCommand command = new SqlCommand();
        command.Connection = connection;
        command.CommandText = $"UPDATE Animal SET Name = @Name, Description = @Description, Category = @Category, Area = @Area WHERE IdAnimal = @IdAnimal";
        command.Parameters.AddWithValue("@Name", animal.Name);
        command.Parameters.AddWithValue("@Description", animal.Description);
        command.Parameters.AddWithValue("@Category", animal.Category);
        command.Parameters.AddWithValue("@Area", animal.Area);
        command.Parameters.AddWithValue("@IdAnimal", idAnimal);

        command.ExecuteNonQuery();
    
        return Ok();
    }

    [HttpDelete("{idAnimal}")]
    public IActionResult DeleteAnimal(int idAnimal)
    {
        // Otwieramy połączenie
        using SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("Default"));
        connection.Open();
    
        // Definiujemy commanda
        using SqlCommand command = new SqlCommand();
        command.Connection = connection;
        command.CommandText = $"DELETE FROM Animal WHERE IdAnimal = @IdAnimal";
        command.Parameters.AddWithValue("@IdAnimal", idAnimal);

        command.ExecuteNonQuery();
    
        return Ok();
    }
}