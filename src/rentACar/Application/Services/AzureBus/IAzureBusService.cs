using Core.Persistence.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services.AzureBus;

    public interface IAzureBusService<T> where T : Entity
{
        Task ConsumerQuee<T>(string queName,Action<T> receivedAction);
        Task  sendMessageToQuee(T entity,string queName);
        Task CreateQueeIfNotExists(string queName);
        Task sendMessageToTopic(T entity, string topicName);
        Task CreateTopicIfNotExists(string topicName);
        Task CreateSubscriptionIfNotExists(string topicName,string subscriptionName);
        Task CreateSubscriptionIfNotExistsWithFilter(string topicName, string subscriptionName, string messageType , string ruleName);
}

