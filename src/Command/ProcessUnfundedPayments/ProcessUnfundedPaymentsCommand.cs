using SFA.DAS.Funding.ApprenticeshipPayments.DurableEntities.Models;

namespace SFA.DAS.Funding.ApprenticeshipPayments.Command.ProcessUnfundedPayments
{
    public class ProcessUnfundedPaymentsCommand
    {
        public ProcessUnfundedPaymentsCommand(byte collectionPeriod, short collectionYear, short previousAcademicYear, DateTime hardCloseDate, ApprenticeshipEntityModel model)
        {
            CollectionPeriod = collectionPeriod;
            CollectionYear = collectionYear;
            PreviousAcademicYear = previousAcademicYear;
            HardCloseDate = hardCloseDate;
            Model = model;
        }

        public byte CollectionPeriod { get; }
        public short CollectionYear { get; set; }
        public short PreviousAcademicYear { get; set; }
        public DateTime HardCloseDate { get; set; }
        public ApprenticeshipEntityModel Model { get; }
    }
}
