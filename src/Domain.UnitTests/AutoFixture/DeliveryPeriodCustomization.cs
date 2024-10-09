using AutoFixture;
using SFA.DAS.Funding.ApprenticeshipEarnings.Types;

namespace SFA.DAS.Funding.ApprenticeshipPayments.Domain.UnitTests.AutoFixture
{
    public class DeliveryPeriodCustomization : ICustomization
    {
        public void Customize(IFixture fixture)
        {
            fixture.Customize<DeliveryPeriod>(c =>
                c.With(x => x.AcademicYear, 2223).With(x => x.CalenderYear, 2022).With(x => x.Period, 1)
                    .With(x => x.CalendarMonth, 10));
        }
    }
}
