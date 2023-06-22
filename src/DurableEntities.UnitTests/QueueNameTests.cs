using System.Linq;
using System.Reflection;
using NUnit.Framework;
using SFA.DAS.Funding.ApprenticeshipPayments.Infrastructure;

namespace SFA.DAS.Funding.ApprenticeshipPayments.DurableEntities.UnitTests
{
    public class QueueNameTests
    {
        // Fix for ServiceBus error
        // The listener for function 'HandleXCommand' was unable to start.
        // Entity path 'SFA.DAS.EmployerIncentives.QueueName' exceeds the '50' character limit. (Parameter 'SubscriptionName')
        [Test]
        public void QueueName_does_not_exceed_50_characters()
        {

            var queues = typeof(QueueNames).GetFields(BindingFlags.Public | BindingFlags.Static |
                                                      BindingFlags.FlattenHierarchy)
                .Where(fi => fi is { IsLiteral: true, IsInitOnly: false }).Select(x => x.GetRawConstantValue()).ToList();

            Assert.IsTrue(queues.Any());
            queues.ForEach(q => Assert.IsTrue(((string)q).Length <= 50, $"'{q}' is {((string)q).Length} long and therefore exceeds 50 character limit"));
        }
    }
}