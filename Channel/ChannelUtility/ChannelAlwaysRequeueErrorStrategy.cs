using EasyNetQ.Consumer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ChannelUtility
{
    public class ChannelAlwaysRequeueErrorStrategy : IConsumerErrorStrategy
    {
        public void Dispose()
        {
        }

        public Task<AckStrategy> HandleConsumerCancelledAsync(ConsumerExecutionContext context, CancellationToken cancellationToken = default)
        {
            return Task.FromResult(AckStrategies.NackWithRequeue);
        }

        public Task<AckStrategy> HandleConsumerErrorAsync(ConsumerExecutionContext context, Exception exception, CancellationToken cancellationToken = default)
        {
            return Task.FromResult(AckStrategies.NackWithRequeue);
        }
    }
}
