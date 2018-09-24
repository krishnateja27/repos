using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.Management.ResourceManager.Fluent;
using Microsoft.Azure.Management.ResourceManager.Fluent.Authentication;
using Microsoft.Azure.Management.ResourceManager.Fluent.Models;
using Microsoft.Rest.Azure;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using Microsoft.Azure.Management.OperationalInsights.Models;
using Intersoft.Crosslight.ViewModels;
using Microsoft.Azure.Management.Resources;
/*   Microsoft.Azure.Management.Resources.
            ViewModelBase viewModelBase = new ViewModelBase();
            viewModelBase.
            StorageInsight storageInsight = new StorageInsight();
            InsightsClient client = new InsightsClient(credentials);
            DateTime endDateTime = DateTime.Now;
            DateTime startDateTime = endDateTime.AddDays(days);
            string filterString = FilterString.Generate<ListEventsForResourceProviderParameters>(eventData => (eventData.EventTimestamp >= startDateTime) && (eventData.EventTimestamp <= endDateTime) && (eventData.ResourceType == "Microsoft.Network/connections"));

            EventDataListResponse response = client.EventOperations.ListEvents(filterString, selectedProperties: null);
            List<EventData> logList = new List<EventData>(response.EventDataCollection.Value);

            while (!string.IsNullOrEmpty(response.EventDataCollection.NextLink))
            {
                response = client.EventOperations.ListEventsNext(response.EventDataCollection.NextLink);
                logList.AddRange(response.EventDataCollection.Value);
            }
                */
namespace Test
{
    class JsonFile
    {
        public string ResourceGroupName { get; set; }
        public string ResourceName { get; set; }
        //JsonFile() { }
    }
    class Program
    {
        private const int V = 100;
        
        static void Main(string[] args)
        {
            List<string> scheduleEntitiesResourceIds = new List<string>();
            scheduleEntitiesResourceIds.Add("/subscriptions/0b349e3e-9da1-454f-941b-1f992729a1ff/resourceGroups/testazure2/providers/Microsoft.Compute/virtualMachines/StandaloneVM1");
            scheduleEntitiesResourceIds.Add("/subscriptions/0b349e3e-9da1-454f-941b-1f992729a1ff/resourceGroups/testazure2/providers/Microsoft.Compute/virtualMachines/StandaloneVM2");
            scheduleEntitiesResourceIds.Add("/subscriptions/0b349e3e-9da1-454f-941b-1f992729a1ff/resourceGroups/testazure2/providers/Microsoft.Compute/virtualMachines/StandaloneVM1");
            scheduleEntitiesResourceIds.Add("/subscriptions/0b349e3e-9da1-454f-941b-1f992729a1ff/resourceGroups/testazure2/providers/Microsoft.Compute/virtualMachines/StandaloneVM2");
            scheduleEntitiesResourceIds.Add("/subscriptions/0b349e3e-9da1-454f-941b-1f992729a1ff/resourceGroups/testazure2/providers/Microsoft.Compute/virtualMachines/StandaloneVM2");

            //scheduleEntitiesResourceIds.Add("/subscriptions/0b349e3e-9da1-454f-941b-1f992729a1ff/resourceGroups/testazure2/providers/Microsoft.Compute/virtualMachines/StandaloneVM3");
            var jsonFile1 = new JsonFile
            {
                ResourceGroupName = "testazure2",
                ResourceName = "StandaloneVM1"
            };
            var jsonFile2 = new JsonFile
            {
                ResourceGroupName = "testazure2",
                ResourceName = "StandaloneVM2"
            };
            var jsonFile3 = new JsonFile
            {
                ResourceGroupName = "testazure2",
                ResourceName = "StandaloneVM3"
            };
            var resultsSet = new List<JsonFile>();
            resultsSet.Add(jsonFile1);
            resultsSet.Add(jsonFile2);
            resultsSet.Add(jsonFile3);

            var resultsSet1 = new List<JsonFile>();
            if (scheduleEntitiesResourceIds.Count() != 0)
            {
                foreach (var result in resultsSet)
                 {
                     foreach (var Id in scheduleEntitiesResourceIds)
                     {
                         if ((Id.Contains(result.ResourceGroupName)) && (Id.Contains(result.ResourceName)))
                         {
                             resultsSet1.Add(result);
                                break;
                         }
                     }
                 }
                 resultsSet = resultsSet.Except(resultsSet1).ToList();
                 
               // resultsSet1 = resultsSet.Where(x => (scheduleEntitiesResourceIds.Contains(x.ResourceGroupName) && scheduleEntitiesResourceIds.Contains(x.ResourceName))).ToList();
               // resultsSet = resultsSet.Except(resultsSet1).ToList();
            }
            //resultsSet = resultsSet.Where(x => (scheduleEntitiesResourceIds.Contains(x.ResourceGroupName) && scheduleEntitiesResourceIds.Contains(x.ResourceName))).ToList();
            else
                resultsSet = resultsSet.ToList();
            foreach (var result in resultsSet)
            {
                Console.WriteLine(result.ResourceGroupName);
                Console.WriteLine(result.ResourceName);
            }
            Console.ReadLine();
        }
    }
}