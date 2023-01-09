using Core.Persistence.Repositories;
using Microsoft.Azure.ServiceBus;
using Microsoft.Azure.ServiceBus.Core;
using Microsoft.Azure.ServiceBus.Management;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Application.Services.AzureBus;

    public class AzureBusManager<TEntity>:IAzureBusService<TEntity> where TEntity:  Entity
{
        private readonly IConfiguration _configuration;
        private readonly ManagementClient _managementClient;


        public AzureBusManager(IConfiguration configuration,ManagementClient managementClient)
        {
            _configuration = configuration;
            _managementClient = managementClient;
        }

    public async Task CreateQueeIfNotExists(string queName)
    {
        if (! await _managementClient.QueueExistsAsync(queName))
            await _managementClient.CreateQueueAsync(queName);

    }

    public async Task CreateTopicIfNotExists(string topicName)
    {
        if (!await _managementClient.TopicExistsAsync(topicName))
            await _managementClient.CreateTopicAsync(topicName);
    }

    public async Task sendMessageToTopic(TEntity entity, string queName)
    {
        var connectionString = _configuration.GetConnectionString("AzureBus");
        var tClient = new TopicClient(connectionString, queName);
       await sendMessage(tClient, entity);
    }

    public async Task sendMessageToQuee(TEntity entity, string topicName)
    {
        var connectionString = _configuration.GetConnectionString("AzureBus");
        var qClient = new QueueClient(connectionString, topicName);
      await  sendMessage(qClient,entity);
      
    }
    private async Task sendMessage(ISenderClient senderClient,TEntity entity)
    {
      
        var msgBody = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(entity));
        var msg = new Message(msgBody);
        await senderClient.SendAsync(msg);
    }

    public async Task CreateSubscriptionIfNotExists(string topicName, string subscriptionName)
    {
        if (!await _managementClient.SubscriptionExistsAsync(topicName,subscriptionName))
            await _managementClient.CreateSubscriptionAsync(topicName,subscriptionName);
    }
    public async Task CreateSubscriptionIfNotExistsWithFilter(string topicName, string subscriptionName,string messageType=null,string ruleName=null)
    {
        if (!await _managementClient.SubscriptionExistsAsync(topicName, subscriptionName))
            return;
        if (messageType == null)
        {
            SubscriptionDescription sd = new(topicName,subscriptionName);
            CorrelationFilter filter = new();
            filter.Properties["MessageType"] = messageType;
            RuleDescription rd = new(ruleName ??messageType +"Rule" , filter);
            await _managementClient.CreateSubscriptionAsync(topicName, subscriptionName); 
        }
        else {
            await _managementClient.CreateSubscriptionAsync(topicName, subscriptionName);
        }
    }

    public async Task ConsumerQuee<TEntity>(string queName, Action<TEntity> receivedAction)
    {
        var connectionString = _configuration.GetConnectionString("AzureBus");
        IQueueClient qClient = new QueueClient(connectionString, queName);
       // SubscriptionClient _subscriptionClient;=new SubscriptionClient(connectionString);
           
        qClient.RegisterMessageHandler(async (message, ct) => {

            var model = JsonConvert.DeserializeObject<TEntity>(Encoding.UTF8.GetString(message.Body));
            receivedAction(model);
            await Task.CompletedTask;
        }, new MessageHandlerOptions(i => Task.CompletedTask));

         
        await Task.CompletedTask;
    }
}

