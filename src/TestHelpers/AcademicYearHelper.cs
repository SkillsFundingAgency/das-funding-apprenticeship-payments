using System.Text.Json;
using System.Text.Json.Serialization;

namespace SFA.DAS.Funding.ApprenticeshipPayments.TestHelpers;

public static class AcademicYearHelper
{
    private static readonly Random Random = new Random();

    public static short GetRandomValidAcademicYear()
    {
        var startYearCode = Random.Next(10, 98);
        return short.Parse($"{startYearCode}{startYearCode+1}");
    }

    /// <summary>
    /// Returns a mocked academic year based on the provided date.
    /// Values are roughly accurate enough to be used in tests.
    /// </summary>
    public static T GetMockedAcademicYear<T>(DateTime searchDate)
    {

        int currentYear = searchDate.Year;
        int currentMonth = searchDate.Month;

        int yearFrom;
        int yearTo;

        if (currentMonth > 7)
        {
            yearFrom = currentYear;
            yearTo = currentYear + 1;
        }
        else
        {
            yearFrom = currentYear - 1;
            yearTo = currentYear;
        }

        var precastObject = new {
            AcademicYear = short.Parse(GetAcademicYearString(yearFrom, yearTo)),
            StartDate = new DateTime(yearFrom, 8, 1),
            EndDate = new DateTime(yearTo, 7, 31),
            HardCloseDate = new DateTime(yearTo, 10, 15, 23, 59, 59)
        };

        var options = new JsonSerializerOptions
        {
            Converters = { new FlexibleStringConverter() }
        };
        var json = JsonSerializer.Serialize(precastObject);

        return JsonSerializer.Deserialize<T>(json, options)!;

    }

    public static string GetAcademicYearString(int yearFrom, int yearTo)
    {
        int from = yearFrom - 2000; // removing 2000 turns 2023 into 23. This should work until the year 2100 at which point a refactor is needed :)
        int to = yearTo - 2000;

        return $"{from}{to}";
    }

    public class FlexibleStringConverter : JsonConverter<string>
    {
        public override string Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
#pragma warning disable CS8603 // Possible null reference return.
            return reader.TokenType switch
            {
                JsonTokenType.String => reader.GetString(),
                JsonTokenType.Number => reader.GetDouble().ToString(),
                _ => throw new JsonException()
            };
#pragma warning restore CS8603 // Possible null reference return.
        }

        public override void Write(Utf8JsonWriter writer, string value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value);
        }
    }
}