using Microsoft.AspNetCore.Mvc;
using AspNetProject.Models;
using CsvHelper;
using System.Globalization;
using CsvHelper.Configuration;

namespace AspNetProject.Controllers;

[ApiController]
[Route("[controller]")]
public class AverageBuildingAgeController : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<Double>> GetAverageBuildingAge() {
        const string filename = "Data/apartment_buildings_2019.csv";
        
        var config = new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            Delimiter = ";"
        };
        
        double average = 0;    
        int counter = 0;    
        
        try
        {
            using (var reader = new StreamReader(filename))
            using (var csv = new CsvReader(reader, config))
            {
                var records = csv.GetRecords<dynamic>().ToList();
                foreach (var record in records)
                {
                    if (record.build_year != null && record.build_year != "0" && record.build_year != "")
                    {
                        average += double.Parse(record.build_year);
                        counter++;
                    }
                }
            }
            
        }
        catch (FileNotFoundException ex)
        {
            Console.WriteLine("File not found: " + ex.Message);
        }
        catch (IOException ex)
        {
            Console.WriteLine("IO error: " + ex.Message);
        }
            
        return Math.Round(average / counter, 2);
    }
}
