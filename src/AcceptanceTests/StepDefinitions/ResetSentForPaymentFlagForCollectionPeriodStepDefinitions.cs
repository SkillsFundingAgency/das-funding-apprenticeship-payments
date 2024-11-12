using NServiceBus;
using SFA.DAS.Funding.ApprenticeshipPayments.AcceptanceTests.Helpers;
using SFA.DAS.Funding.ApprenticeshipPayments.Domain.SystemTime;
using SFA.DAS.Funding.ApprenticeshipPayments.Types;

namespace SFA.DAS.Funding.ApprenticeshipPayments.AcceptanceTests.StepDefinitions;

[Binding]
[Scope(Feature = "Payments Release")]
public class ResetSentForPaymentFlagForCollectionPeriodStepDefinitions
{
    private readonly TestContext _testContext;
    private readonly ISystemClockService _systemClockService;

    public ResetSentForPaymentFlagForCollectionPeriodStepDefinitions(TestContext testContext)
    {
        _testContext = testContext;
        _systemClockService = TestSystemClock.Instance();
    }

    [When("ResetSentForPaymentFlagForCollectionPeriod has been triggered")]
    public async Task ResetSentForPaymentFlagForCollectionPeriod()
    {
        await _testContext.ResetSentForPaymentFlagEndpoint.Publish(
            new ResetSentForPaymentFlagForCollectionPeriodDurableEntityCommand
            {
                CollectionPeriod = ((byte)_systemClockService.Now.Month).ToDeliveryPeriod(),
                CollectionYear = ((short)_systemClockService.Now.Year).ToAcademicYear((byte)DateTime.Now.Month)
            });
        Task.Delay(3500).Wait();
    }
}