using AutoFixture;
using SFA.DAS.Funding.ApprenticeshipEarnings.Types;

namespace SFA.DAS.Funding.ApprenticeshipPayments.Domain.UnitTests.AutoFixture
{
    public class EarningsGeneratedEventCustomization : ICustomization
    {
        public void Customize(IFixture fixture)
        {
            fixture.Customize(new DeliveryPeriodCustomization());
            fixture.Customize<EarningsGeneratedEvent>(c =>
                c.With(x => x.Uln, fixture.Create<long>().ToString()).With(x => x.EmployerType, EmployerType.NonLevy)
                    .With(x => x.AgeAtStartOfApprenticeship, 22));
        }
    }
}
