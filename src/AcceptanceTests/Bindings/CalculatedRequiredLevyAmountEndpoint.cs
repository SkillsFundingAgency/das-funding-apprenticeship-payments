﻿using SFA.DAS.Funding.ApprenticeshipPayments.AcceptanceTests.Helpers;
using SFA.DAS.Funding.ApprenticeshipPayments.Infrastructure;
using SFA.DAS.Payments.RequiredPayments.Messages.Events;

namespace SFA.DAS.Funding.ApprenticeshipPayments.AcceptanceTests.Bindings;

[Binding]
[Scope(Tag = "CalculatedRequiredLevyAmountEndpoint")]
public class CalculatedRequiredLevyAmountEndpoint
{
    [BeforeScenario]
    public async Task StartEndpoint(TestContext context)
    {
        if (context.CalculatedRequiredLevyAmountEndpoint is not null) return;

        context.CalculatedRequiredLevyAmountEndpoint = await EndpointHelper
            .StartEndpoint("sfa.das.funding.payments.calculatedlevyamount", false, new[]
            {
                typeof(CalculatedRequiredLevyAmount)
            });
    }
}