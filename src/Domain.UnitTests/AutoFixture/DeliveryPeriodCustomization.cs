using AutoFixture;
using SFA.DAS.Funding.ApprenticeshipEarnings.Types;

namespace SFA.DAS.Funding.ApprenticeshipPayments.Domain.UnitTests.AutoFixture
{
    public class DeliveryPeriodCustomization : ICustomization
    {
        public void Customize(IFixture fixture)
        {
            fixture.Customize<DeliveryPeriod>(c =>
                c.With(x => x.AcademicYear, 2324).With(x => x.CalenderYear, 2024).With(x => x.Period, 12)
                    .With(x => x.CalendarMonth, 7));
        }
    }
}
