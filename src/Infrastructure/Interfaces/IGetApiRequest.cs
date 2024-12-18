using System.Text.Json.Serialization;

namespace SFA.DAS.Funding.ApprenticeshipPayments.Infrastructure.Interfaces;

public interface IGetApiRequest : IApiRequest
{
    /// <summary>
    /// This is the relative URL used in the GET request. Note that this should not have a leading slash.
    /// </summary>
    [JsonIgnore]
    string GetUrl { get; }
}