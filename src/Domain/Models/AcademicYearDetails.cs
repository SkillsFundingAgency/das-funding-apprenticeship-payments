namespace SFA.DAS.Funding.ApprenticeshipPayments.Domain.Models;

public class AcademicYears
{
    public AcademicYearDetails PreviousYear { get; set; }
    public AcademicYearDetails CurrentYear { get; set; }

    public AcademicYears(AcademicYearDetails previous, AcademicYearDetails current)
    {
        PreviousYear = previous;
        CurrentYear = current;
    }
}

public class AcademicYearDetails
{
    public short AcademicYear { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public DateTime HardCloseDate { get; set; }

    public AcademicYearDetails(short academicYear, DateTime startDate, DateTime endDate, DateTime hardCloseDate)
    {
        AcademicYear = academicYear;
        StartDate = startDate;
        EndDate = endDate;
        HardCloseDate = hardCloseDate;
    }
}