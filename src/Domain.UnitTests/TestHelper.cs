using SFA.DAS.Funding.ApprenticeshipPayments.Domain.Models;
using SFA.DAS.Funding.ApprenticeshipPayments.TestHelpers;
using System;

namespace SFA.DAS.Funding.ApprenticeshipPayments.Domain.UnitTests;

internal static class TestHelper
{
    internal static AcademicYears CreateAcademicYears(DateTime now)
    {
        var currentYear = AcademicYearHelper.GetMockedAcademicYear<AcademicYearDetails>(now);
        var previousYear = AcademicYearHelper.GetMockedAcademicYear<AcademicYearDetails>(currentYear.StartDate.AddMonths(-6));
        return new AcademicYears(currentYear, previousYear);
    }
}
