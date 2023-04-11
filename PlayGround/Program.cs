using System;
using System.Diagnostics;
using System.Diagnostics.Metrics;
using System.Net;
using System.Net.Http.Json;
using System.Text;

namespace PlayGround;
class Program
{
    static async Task Main(string[] args)
    {
        var option = 0;
        while(option != 5)
        {
            option = PrintMenu();
            object result = null;
            if (option == 1)
            {
                result = DecimalToBinary();
            }
            else if (option == 2)
            {
                result = BinaryToDecimal();
            }
            else if (option == 3)
            {
                result = BinaryRightOneBit();
            }
            else if (option == 4)
            {
                result = await HighestRankedUniversity();
            }

            Console.WriteLine("RESULT: " + result);
        }
    }

    static string DecimalToBinary(string number = "")
    {
        if (string.IsNullOrEmpty(number))
        {
            Console.WriteLine("Enter a decimal number");
            number = Console.ReadLine();
        }

        long divided = 0;
        long.TryParse(number, out divided);
        if (divided == 0)
        {
            return "Invalid number";
        }

        var result = new StringBuilder();
        while (true)
        {
            if (divided == 1)
            {
                result.Append(1);
                break;
            }

            result.Append((divided % 2));
            divided = divided / 2;
        }


        return new string(result.ToString().Reverse().ToArray());
    }

    static double BinaryToDecimal()
    {
        Console.WriteLine("Enter a binary number");
        var binary = Console.ReadLine();
        double result = 0;
        for (var i = 0; i < binary.Length; i++)
        {
            double based = 2;
            var place = Convert.ToInt32(binary[i] - '0');
            double idx = binary.Length - (i + 1);
            var power = Math.Pow(based, idx);
            result += place * power;
        }

        return result;
    }

    static char BinaryRightOneBit()
    {
        Console.WriteLine("Enter a decimal number");
        var input = Console.ReadLine() ?? "";
        var result = DecimalToBinary(input);
        return result[result.Length - 1];
    }

    static async Task<string> HighestRankedUniversity()
    {
        var country = GetRandomCountry();
        Console.WriteLine("Country: " + country);
        Console.WriteLine("Loading...");

        var page = 1;
        var universities = new List<University>();
        var response = await GetUniversities(page);

        for (var i = 1; i <= response.total_pages; i++)
        {
            if (i > 1)
            {
                response = await GetUniversities(i);
            }

            universities.AddRange(response.data.Where(s => s.location.country.Equals(country)));
        }

        var university = universities.OrderByDescending(s => s.rank_display).ToList();
        return university[0].university;
    }

    #region Private
    private static int PrintMenu()
    {
        var displayMenu = "Select one option:\n"
            + "1. Decimal to binary\n"
            + "2. Binary to decimal\n"
            + "3. Bitwise most rightone bit\n"
            + "4. Best Ranked University\n"
            + "5. Salir\n";

        Console.WriteLine();
        Console.WriteLine(displayMenu);
        var input = Console.ReadLine();

        int result = 0;
        int.TryParse(input, out result);

        return result;
    }

    private static async Task<UniversityResponse> GetUniversities(int page = 1)
    {
        var url = $"https://jsonmock.hackerrank.com/api/universities?page={page}";
        var client = new HttpClient();
        var response = await client.GetAsync(url);
        var content = await response.Content.ReadFromJsonAsync<UniversityResponse>();
        return content;
    }

    private static string GetRandomCountry()
    {
        var countries = new string[] { "United States", "United Kingdom", "Switzerland", "Singapore"
            , "China(Mainland)", "Hong Kong SAR", "Japan", "Canada", "Australia", "South Korea"
            , "France", "Germany", "Netherlands", "Malaysia", "Taiwan", "Argentina", "Belgium"
            , "Russia", "Denmark", "New Zealand", "Sweden", "Ireland", "Norway", "Finland"
            , "Mexico", "Saudi Arabia", "Brazil", "Chile", "Italy", "Austria", "Spain"
            , "Kazakhstan", "India", "United Arab Emirates", "Israel" };
        return countries[new Random().Next(countries.Length)];
    }
    #endregion
}

class UniversityResponse
{
    public int page { get; set; }
    public int per_page { get; set; }
    public int total { get; set; }
    public int total_pages { get; set; }
    public List<University> data { get; set; }
}

class University
{
    public string university { get; set; }
    public int rank_display { get; set; }
    public Location location { get; set; }
}

class Location
{
    public string country { get; set; }
}