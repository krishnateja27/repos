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
    class Program
    {
        static void Main(string[] args)
        {
            bool somename = null;
            if (somename)
                Console.WriteLine("true");
            else
                Console.WriteLine("false");
            
        }
    }
}