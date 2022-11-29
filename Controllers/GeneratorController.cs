
using Microsoft.AspNetCore.Mvc;

namespace WebApplication1.Controllers;

[ApiController]
[Route("[controller]")]
public class GeneratorController : ControllerBase
{
    private  Random _rnd = new Random();
    
    private readonly ILogger<GeneratorController> _logger;

    public GeneratorController(ILogger<GeneratorController> logger)
    {
        _logger = logger;
    }
    
    [HttpGet(Name = "Generate")]
    public  IResult Generate(int size, int count, string letters)
    {
        if (IsDuplicates(letters) || size < 1 || count < 1)
        {
            return Results.BadRequest($"Incorrect input data! " +
                                      $"You may input letters with duplicates " +
                                      $"or(and) amount of words less than 1 " +
                                      $"or(and) words length is less than 1");
        }
        return Results.Ok(CreateListOfWords(size, count, letters));
    }

    private  List<string> CreateListOfWords(int size, int count, string letters)
    {
        var list = new List<string>();
        
        for (var i = 0; i < count; i++)
        {
            var word = new List<char>();
            for (var j = 0; j < size; j++)
            {
                var index = _rnd.Next(letters.Length);
                word.Add(letters[index]);
            }
            list.Add(string.Join("", word));
        }

        return list;
    }
    private bool IsDuplicates(string letters)
    {
        var list = new List<char>();
        foreach (var letter in letters)
        {
            if (list.Contains(letter))
            {
                return true;
            }

            list.Add(letter);
        }

        return false;
    }
}