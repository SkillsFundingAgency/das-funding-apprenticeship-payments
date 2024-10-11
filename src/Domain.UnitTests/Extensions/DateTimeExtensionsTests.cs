using NUnit.Framework;
using SFA.DAS.Funding.ApprenticeshipPayments.Domain.Extensions;
using System;

namespace SFA.DAS.Funding.ApprenticeshipPayments.Domain.UnitTests.Extensions;

[TestFixture]
public class DateTimeExtensionsTests
{
    [TestCase("2023-08-01", 2324)]
    [TestCase("2023-07-31", 2223)]
    [TestCase("2022-09-01", 2223)]
    [TestCase("2022-01-01", 2122)]
    public void ToAcademicYear_ShouldReturnExpectedAcademicYear(string date, short expectedAcademicYear)
    {
        // Arrange
        var dateTime = DateTime.Parse(date);

        // Act
        var result = dateTime.ToAcademicYear();

        // Assert
        Assert.AreEqual(expectedAcademicYear, result);
    }
}
