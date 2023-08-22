using System.Text.Json;

namespace SFA.DAS.Funding.ApprenticeshipPayments.Infrastructure;

public static class JsonStringExtensions
{
    public static string SerialiseForLogging(this object json)
    {
        return JsonSerializer.Serialize(json, new JsonSerializerOptions { WriteIndented = true });
    }
}