using Newtonsoft.Json;

namespace SFA.DAS.Funding.ApprenticeshipPayments.DurableEntities.Models
{
    [JsonObject(MemberSerialization.OptIn)]
    public class ApprenticeshipEntityModel
    {
        [JsonProperty] public Guid ApprenticeshipKey { get; set; }
    }
}
