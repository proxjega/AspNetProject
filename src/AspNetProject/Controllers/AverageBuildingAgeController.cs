using Microsoft.AspNetCore.Mvc;
using CsvHelper;
using System.Globalization;
using CsvHelper.Configuration;
using AspNetProject.Utilities;

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
        
        // parse csv
        List<dynamic> records = new();
        try
        {
            using (var reader = new StreamReader(filename))
            using (var csv = new CsvReader(reader, config))
            {
                records = csv.GetRecords<dynamic>().ToList();
            }
            
        }
        catch (FileNotFoundException ex)
        {
            Console.WriteLine("File not found: " + ex.Message);
            return StatusCode(500, new { error = "An error occurred. Please try again later." });
        }
        catch (IOException ex)
        {
            Console.WriteLine("IO error: " + ex.Message);
            return StatusCode(500, new { error = "An error occurred. Please try again later." });
        }

        // put records into dictionary
        Dictionary<string, (int counter, double yearSum)> averageAgesByStreet = new Dictionary<string, (int counter, double yearSum)>();
        foreach (var record in records)
        {
            if (record.build_year != null && record.build_year != "0" && record.build_year != "")
            {   
                string street = AddressHelper.GetStreetName(record.adresas);
                if (street == "") continue;
                if (!averageAgesByStreet.ContainsKey(street))
                {
                    averageAgesByStreet.Add(street, (0, 0.0));
                }
                if (averageAgesByStreet.TryGetValue(street, out (int counter, double yearSum) current))
                {
                    averageAgesByStreet[street] = (current.counter + 1, current.yearSum + double.Parse(record.build_year));
                }
            }
        }

        // calculate average
        SortedDictionary<string, double> result = new();
        foreach (var street in averageAgesByStreet)
        {
            result[street.Key] = Math.Round(street.Value.yearSum / street.Value.counter, 2);
        }

        return Ok(result);
    }

    [HttpPost]
    public async Task<ActionResult> PostException()
    {
        throw new Exception("Exception");
        return Ok();
    }
}


