using SFA.DAS.ApprenticeshipPayments.Query.GetApprenticeshipsWithDuePayments;
using SFA.DAS.Funding.ApprenticeshipPayments.Command.ProcessUnfundedPayments;
using SFA.DAS.Funding.ApprenticeshipPayments.Types;

namespace SFA.DAS.Funding.ApprenticeshipPayments.Functions;

public class PaymentsFunctions
{
    private readonly IProcessUnfundedPaymentsCommandHandler _processUnfundedPaymentsCommandHandler;
    private readonly IGetApprenticeshipsWithDuePaymentsQueryHandler _getApprenticeshipsWithDuePaymentsQueryHandler;

    public PaymentsFunctions(IProcessUnfundedPaymentsCommandHandler processUnfundedPaymentsCommandHandler, IGetApprenticeshipsWithDuePaymentsQueryHandler getApprenticeshipsWithDuePaymentsQueryHandler)
    {
        _getApprenticeshipsWithDuePaymentsQueryHandler = getApprenticeshipsWithDuePaymentsQueryHandler;
        _processUnfundedPaymentsCommandHandler = processUnfundedPaymentsCommandHandler;
    }

    [FunctionName(nameof(ReleasePaymentsEventServiceBusTrigger))]
    public async Task ReleasePaymentsEventServiceBusTrigger(
        [NServiceBusTrigger(Endpoint = QueueNames.ReleasePayments)] ReleasePaymentsCommand releasePaymentsCommand,
        ILogger log)
    {
        var result = await _getApprenticeshipsWithDuePaymentsQueryHandler.Get(new GetApprenticeshipsWithDuePaymentsQuery(releasePaymentsCommand.CollectionPeriod, releasePaymentsCommand.CollectionYear));

        var releasePaymentsTasks = result.Apprenticeships.Select(x => _processUnfundedPaymentsCommandHandler.Process(new ProcessUnfundedPaymentsCommand(releasePaymentsCommand.CollectionPeriod, releasePaymentsCommand.CollectionYear, x.ApprenticeshipKey)));
        log.LogInformation($"Releasing payments for collection period {releasePaymentsCommand.CollectionPeriod} & year {releasePaymentsCommand.CollectionYear} for apprenticeships. (Count: {result.Apprenticeships.Count()})");
        await Task.WhenAll(releasePaymentsTasks);

        log.LogInformation($"Releasing payments for collection period {releasePaymentsCommand.CollectionPeriod} & year {releasePaymentsCommand.CollectionYear} complete.");
    }
}