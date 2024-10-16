﻿using System.Collections.Generic;
using SFA.DAS.Funding.ApprenticeshipPayments.Functions.Inputs;
using SFA.DAS.Funding.ApprenticeshipPayments.Query.GetDuePayments;

namespace SFA.DAS.Funding.ApprenticeshipPayments.Functions.Activities
{
    public class GetDuePayments
    {
        private readonly IGetDuePaymentsQueryHandler _queryHandler;

        public GetDuePayments(IGetDuePaymentsQueryHandler queryHandler)
        {
            _queryHandler = queryHandler;
        }

        [FunctionName(nameof(GetDuePayments))]
        public async Task<IEnumerable<Guid>> Get([ActivityTrigger] object input)
        {
            var duePaymentsInput = (GetDuePaymentsInput)input;
            var payments = (await _queryHandler.Get(new GetDuePaymentsQuery(duePaymentsInput.ApprenticeshipKey, duePaymentsInput.CollectionDetails.CollectionYear, duePaymentsInput.CollectionDetails.CollectionPeriod))).Payments;
            return payments.Select(x => x.Key);
        }
    }
}