using SFA.DAS.Funding.ApprenticeshipPayments.Command.CalculateApprenticeshipPayments;
using SFA.DAS.Funding.ApprenticeshipPayments.Domain.Apprenticeship;
using SFA.DAS.Funding.ApprenticeshipPayments.DurableEntities.Models;

namespace SFA.DAS.Funding.ApprenticeshipPayments.Command.RecalculateApprenticeshipPayments
{
    public class RecalculateApprenticeshipPaymentsCommand
    {
        public RecalculateApprenticeshipPaymentsCommand(ApprenticeshipEntityModel apprenticeshipEntity, List<EarningsRecalculatedDeliveryPeriod> newEarnings)
        {
            ApprenticeshipEntity = apprenticeshipEntity;
            NewEarnings = newEarnings;
        }

        public ApprenticeshipEntityModel ApprenticeshipEntity { get; }
        public List<EarningsRecalculatedDeliveryPeriod> NewEarnings { get; set; }
    }

    //todo where should this live and should it be scoped to this command or a reusable concept of a delivery period that is not either tied to an output event or party of the entity model?
    public class EarningsRecalculatedDeliveryPeriod
    {
        public byte Period { get; set; }
        public byte CalendarMonth { get; set; }
        public short CalenderYear { get; set; }
        public short AcademicYear { get; set; }
        public Decimal LearningAmount { get; set; }
        public string FundingLineType { get; set; }
    }

    public interface IRecalculateApprenticeshipPaymentsCommandHandler
    {
        Task<Apprenticeship> Calculate(RecalculateApprenticeshipPaymentsCommand command);
    }

    public class RecalculateApprenticeshipPaymentsCommandHandler : IRecalculateApprenticeshipPaymentsCommandHandler
    {
        public Task<Apprenticeship> Calculate(RecalculateApprenticeshipPaymentsCommand command)
        {
            throw new NotImplementedException();
        }
    }
}
