using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using Newtonsoft.Json;
using System.Xml;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.Azure.Management.Authorization;
using Microsoft.Azure.Management.Compute;
using Microsoft.Azure.Management.ResourceManager;
using Microsoft.Azure.Management.ResourceManager.Fluent;
using Microsoft.Azure.Management.ResourceManager.Fluent.Models;
using Microsoft.WindowsAzure;
using Microsoft.Azure.CosmosDB.Table;
using Microsoft.Azure.Management.Fluent;

public class Json_File : Microsoft.WindowsAzure.Storage.Table.TableEntity
{
    public Json_File(string ScrapeName, string StorageAccountName)
    {
        this.RowKey = ScrapeName;
        this.PartitionKey = StorageAccountName;
    }
    public string ScrapeName { get; set; }
    public string KeyValue { get; set; }  //Property of the fields or variables
    public string StorageAccountName { get; set; }
    public string ResourceGroup { get; set; }
    public string Directory { get; set; }
    public long SizeOfFile { get; set; }
    public string Type { get; set; }
    public string Uri { get; set; }
    public string LastModified { get; set; }
    public string LeaseStatus { get; set; }
    public string LeaseState { get; set; }
    public string LeaseDuration { get; set; }
    public string ETagValue { get; set; }
    public string SubcriptionId { get; set; }
    public string ContainerName { get; set; }
}
class LoadNamesInContainer
{
    //call the connection by passing the object list
    //Obj2 obj2 = new Obj2();
    //obj2.KeyName = //somefunction value
    public void LoadDirectoryContents(IEnumerable<IListBlobItem> listBlobs, Microsoft.WindowsAzure.Storage.Blob.CloudBlobContainer Container, int hidelength, List<Json_File> json_Files,String ActualDirectoryName,Microsoft.WindowsAzure.Storage.CloudStorageAccount storageAccount)
    {
        //hidelength = 0;
        foreach (IListBlobItem item in listBlobs)
        {
            if(item.GetType().Name!= "CloudBlockBlob")
            {
                var Subdirectorydisplaylength = (((Microsoft.WindowsAzure.Storage.Blob.CloudBlobDirectory)item).Prefix).Length;
                Json_File json_File = new Json_File(((Microsoft.WindowsAzure.Storage.Blob.CloudBlobDirectory)item).Prefix.Substring(hidelength, Subdirectorydisplaylength - hidelength - 1), storageAccount.Credentials.AccountName);
                json_Files.Add(json_File);
                json_File.ScrapeName = ((Microsoft.WindowsAzure.Storage.Blob.CloudBlobDirectory)item).Prefix.Substring(hidelength, Subdirectorydisplaylength - hidelength - 1);
                json_File.SizeOfFile = 0;
                json_File.Type = item.GetType().Name;
                json_File.Directory = ActualDirectoryName;
                json_File.Uri = item.Uri.ToString();
                json_File.StorageAccountName = storageAccount.Credentials.AccountName;
                json_File.KeyValue = storageAccount.Credentials.ExportBase64EncodedKey();
                json_File.ContainerName = Container.Name;
                String directoryName = ((Microsoft.WindowsAzure.Storage.Blob.CloudBlobDirectory)item).Prefix.Substring(hidelength, Subdirectorydisplaylength - hidelength);
                CloudBlobDirectory directory = Container.GetDirectoryReference(((Microsoft.WindowsAzure.Storage.Blob.CloudBlobDirectory)item).Prefix);
                LoadDirectoryContents(directory.ListBlobs(), Container, Subdirectorydisplaylength,json_Files,ActualDirectoryName + ((Microsoft.WindowsAzure.Storage.Blob.CloudBlobDirectory)item).Prefix.Substring(hidelength, Subdirectorydisplaylength - hidelength - 1),storageAccount);
            }
            else 
            {
                var length = ((Microsoft.WindowsAzure.Storage.Blob.CloudBlob)item).Name.Length;
                Json_File json_File = new Json_File(((Microsoft.WindowsAzure.Storage.Blob.CloudBlob)item).Name.Substring(hidelength, length - hidelength), storageAccount.Credentials.AccountName);
                json_Files.Add(json_File);
                json_File.ScrapeName = ((Microsoft.WindowsAzure.Storage.Blob.CloudBlob)item).Name.Substring(hidelength, length - hidelength);
                json_File.SizeOfFile = ((Microsoft.WindowsAzure.Storage.Blob.CloudBlob)item).Properties.Length;
                json_File.Type = item.GetType().Name;
                json_File.Directory = ActualDirectoryName;
                json_File.Uri = item.Uri.ToString();
                json_File.ETagValue = ((Microsoft.WindowsAzure.Storage.Blob.CloudBlob)item).Properties.ETag;
                json_File.LeaseDuration = ((Microsoft.WindowsAzure.Storage.Blob.CloudBlob)item).Properties.LeaseDuration.ToString();
                json_File.LeaseStatus = ((Microsoft.WindowsAzure.Storage.Blob.CloudBlob)item).Properties.LeaseStatus.ToString();
                json_File.LeaseState = ((Microsoft.WindowsAzure.Storage.Blob.CloudBlob)item).Properties.LeaseState.ToString();
                json_File.LastModified = ((Microsoft.WindowsAzure.Storage.Blob.CloudBlob)item).Properties.LastModified.ToString();
                json_File.StorageAccountName = storageAccount.Credentials.AccountName;
                json_File.KeyValue = storageAccount.Credentials.ExportBase64EncodedKey();
                json_File.ContainerName = Container.Name;
                //return;
            }
        }
    }

    public void LoadBlobContents(IEnumerable<IListBlobItem> listBlobs, CloudBlobContainer Container,List<Json_File> json_Files,Microsoft.WindowsAzure.Storage.CloudStorageAccount storageAccount)
    {
        foreach (IListBlobItem item in listBlobs)
        {
            if (item.GetType().Name == "CloudBlockBlob")
            {
                Json_File json_File = new Json_File((((Microsoft.WindowsAzure.Storage.Blob.CloudBlob)item).Name), storageAccount.Credentials.AccountName);
                json_Files.Add(json_File);
                json_File.ScrapeName = (((Microsoft.WindowsAzure.Storage.Blob.CloudBlob)item).Name);
                json_File.SizeOfFile = ((Microsoft.WindowsAzure.Storage.Blob.CloudBlob)item).Properties.Length;
                json_File.Type = item.GetType().Name;
                json_File.Directory = Container.Name;
                json_File.Uri = item.Uri.ToString();
                json_File.ETagValue = ((Microsoft.WindowsAzure.Storage.Blob.CloudBlob)item).Properties.ETag;
                json_File.LeaseDuration = ((Microsoft.WindowsAzure.Storage.Blob.CloudBlob)item).Properties.LeaseDuration.ToString();
                json_File.LeaseStatus = ((Microsoft.WindowsAzure.Storage.Blob.CloudBlob)item).Properties.LeaseStatus.ToString();
                json_File.LeaseState = ((Microsoft.WindowsAzure.Storage.Blob.CloudBlob)item).Properties.LeaseState.ToString();
                json_File.LastModified = ((Microsoft.WindowsAzure.Storage.Blob.CloudBlob)item).Properties.LastModified.ToString();
                json_File.StorageAccountName = storageAccount.Credentials.AccountName;
                json_File.KeyValue = storageAccount.Credentials.ExportBase64EncodedKey();
                json_File.ContainerName = Container.Name;
            }
            //Console.WriteLine(item.Uri);
            //if(item.GetType().Name == "CloudBlobDirectory")
            else
            {
                
                var displaylength = (((Microsoft.WindowsAzure.Storage.Blob.CloudBlobDirectory)item).Prefix).Length;
                Json_File json_File = new Json_File(((Microsoft.WindowsAzure.Storage.Blob.CloudBlobDirectory)item).Prefix.Substring(0, displaylength - 1), storageAccount.Credentials.AccountName);
                json_Files.Add(json_File);
                json_File.ScrapeName = ((Microsoft.WindowsAzure.Storage.Blob.CloudBlobDirectory)item).Prefix.Substring(0, displaylength - 1);
                json_File.SizeOfFile = 0;
                json_File.Type = item.GetType().Name;
                json_File.Directory = Container.Name;
                json_File.Uri = item.Uri.ToString();
                json_File.StorageAccountName = storageAccount.Credentials.AccountName;
                json_File.KeyValue = storageAccount.Credentials.ExportBase64EncodedKey();
                json_File.ContainerName = Container.Name;
                String directoryName = ((Microsoft.WindowsAzure.Storage.Blob.CloudBlobDirectory)item).Prefix.Substring(0, displaylength - 1);
                String ActualDirectoryName = ((Microsoft.WindowsAzure.Storage.Blob.CloudBlobDirectory)item).Prefix;
                CloudBlobDirectory directory = Container.GetDirectoryReference(directoryName);
                //directory.Prefix;
                LoadDirectoryContents(directory.ListBlobs(), Container, displaylength,json_Files,Container.Name+"/"+ActualDirectoryName,storageAccount);
            }
        }
    }
}
    
class C
{
    public async Task AssignFunctionAsync(List<Json_File> json_Files)
    {
        //load the connection
        try
        {
            XmlDocument document = new XmlDocument();
            document.Load(@"C:\Users\Administrator\source\repos\ConsoleApp5\ConsoleApp5\bin\Debug\Test.xml");
            var appSettings = System.Configuration.ConfigurationManager.AppSettings;
            if (appSettings.Count == 0)
            {
                Console.WriteLine("AppSettings is empty");
                Console.WriteLine("Press any key to continue...");
                Console.ReadLine();
            }
            else
            {
                string ResourceGroupName = null;
                string SubscriptionID = null;
                string StorageAccountName = null;
                foreach (var key in appSettings.AllKeys)
                {
                    Microsoft.WindowsAzure.Storage.CloudStorageAccount storageAccount = Microsoft.WindowsAzure.Storage.CloudStorageAccount.Parse(ConfigurationManager.AppSettings[key]);
                    
                    //Microsoft.Azure.Management.ResourceManager.Fluent.Authentication.AzureCredentials azureCredentials = new Microsoft.Azure.Management.ResourceManager.Fluent.Authentication.AzureCredentials();
                    //Console.WriteLine(azureCredentials.DefaultSubscriptionId);
                    
                    //Microsoft.Azure.Management.ResourceManager.Fluent.SdkContext.AzureCredentialsFactory.

                    //Microsoft.Azure.Management.Compute.Models.Resource resource = new Microsoft.Azure.Management.Compute.Models.Resource();
                    
                    //Console.WriteLine(resource);
                    //"d4df43b2 - 6edc - 4263 - b1b6 - c600b660640f"

                    //var resourceId = Microsoft.Azure.Management.ResourceManager.Fluent.Core.ResourceId.FromString("acd");
                    //Microsoft.Azure.Management.ResourceManager.Fluent.Core.
                    //resourceId.ResourceGroupName;
                    //Microsoft.WindowsAzure.Management.Compute.Models.
                    //Microsoft.WindowsAzure.Management.Compute.ComputeManagementClient computeManagementClient = new Microsoft.WindowsAzure.Management.Compute.ComputeManagementClient();
                    //Console.WriteLine(computeManagementClient.Credentials.SubscriptionId);
                    //Microsoft.Azure.Management.Storage.StorageManagementClient storageManagementClient = new Microsoft.Azure.Management.Storage.StorageManagementClient(Microsoft.Azure.SubscriptionCloudCredentials subscriptionCloudCredentials);
                    //string SubscriptionId = storageManagementClient.Credentials.SubscriptionId;
                    //Console.WriteLine(SubscriptionId);
                    //Microsoft.WindowsAzure.SubscriptionCloudCredentials subscriptionCloudCredentials
                    //Console.WriteLine("Newwwwwwwwwwww");
                    if (Microsoft.WindowsAzure.Storage.CloudStorageAccount.TryParse(ConfigurationManager.AppSettings[key], out storageAccount))
                    {
                        Console.WriteLine("Connection is established");
                        CloudBlobClient cloudBlobClient = storageAccount.CreateCloudBlobClient();
                        
                        // List the blobs in the container.
                        Console.WriteLine("List blobs in container.");

                        //Console.WriteLine(cloudBlobClient.ListContainers().Count());

                        foreach (var Container in cloudBlobClient.ListContainers())
                        {
                            Json_File json_File = new Json_File(Container.Name, storageAccount.Credentials.AccountName);
                            StorageAccountName = storageAccount.Credentials.AccountName;
                            json_Files.Add(json_File);
                            json_File.ScrapeName = Container.Name;
                            json_File.Type = Container.GetType().Name;
                            json_File.Uri = Container.Uri.ToString();
                            json_File.ETagValue = Container.Properties.ETag.ToString();
                            json_File.LeaseDuration = Container.Properties.LeaseDuration.ToString();
                            json_File.LeaseStatus = Container.Properties.LeaseStatus.ToString();
                            json_File.LeaseState = Container.Properties.LeaseState.ToString();
                            json_File.LastModified = Container.Properties.LastModified.ToString();
                            json_File.KeyValue = storageAccount.Credentials.ExportBase64EncodedKey();
                            json_File.StorageAccountName = storageAccount.Credentials.AccountName;
                            if (key == "StorageConnectionString3")
                            {
                                const string clientId = "6b80f7db-7f71-4ef8-b412-9b2019a18d18";
                                const string clientsecretkey = "PsilcW9hQPNYQi9raC4t7Mi/kCstwLdEcWcTU+0HLD4=";
                                const string tenantId = "fa23f4b5-cee9-4c9e-a774-d31b0f10c151";
                                const string subscriptionId = "d4df43b2-6edc-4263-b1b6-c600b660640f";

                                json_File.SubcriptionId = subscriptionId;

                                var creds = SdkContext.AzureCredentialsFactory.FromServicePrincipal(clientId, clientsecretkey, tenantId, AzureEnvironment.AzureGlobalCloud);
                              
                                var azure = Azure.Authenticate(creds).WithSubscription(subscriptionId);

                                var resourecelist = azure.ResourceGroups.List().ToList();
                               
                                var subscriptionClient = new SubscriptionClient(creds);
                                
                                Microsoft.Rest.Azure.IPage<SubscriptionInner> subscriptionTask = null;

                                subscriptionTask = await subscriptionClient.Subscriptions.ListAsync();

                                var SubscriptionList = subscriptionTask?.Select(y => y);

                                foreach(var subscription in SubscriptionList)
                                {
                                    Console.WriteLine(subscription.DisplayName);
                                    Console.WriteLine(subscription.Id);
                                }

                                var m = new Microsoft.Azure.Management.Resources.ResourceManagementClient(creds) { SubscriptionId = subscriptionId };

                               // m.ResourceGroups.ListResourcesWithHttpMessagesAsync

                                var resourceManagementClient = new ResourceManagementClient(creds) { SubscriptionId = subscriptionId };
                                
                                var resourceGroupTask = await resourceManagementClient.ResourceGroups.ListAsync();
                                
                                var resourceGroupList = resourceGroupTask?.Select(y => y);

                                foreach(var resourceGroup in resourceGroupList)
                                {
                                    foreach(var storageaccount in azure.StorageAccounts.ListByResourceGroup(resourceGroup.Name))
                                    {
                                        if (storageaccount.Name == storageAccount.Credentials.AccountName)
                                            json_File.ResourceGroup = resourceGroup.Name;
                                    }
                                    //Console.WriteLine(resourceGroup.Name);
                                }

                                ResourceGroupName = json_File.ResourceGroup;
                                SubscriptionID = json_File.SubcriptionId;
                                /* var credentials = SdkContext.AzureCredentialsFactory.FromServicePrincipal(clientId, clientsecretkey, tenantId, AzureEnvironment.AzureGlobalCloud);

                                 Microsoft.WindowsAzure.Management.Compute.ComputeManagementClient computeManagementClient = new Microsoft.WindowsAzure.Management.Compute.ComputeManagementClient();

                                 var resourceClient = new ResourceManagementClient(credentials);
                                 resourceClient.SubscriptionId = subscriptionId;
                                 resourceClient.ResourceGroups.ListAsync();
                                 */
                                //var azure = Microsoft.Azure.Authenticate(credentials).WithSubscription(subscriptionId);
                                //var resourecelist = azure.ResourceGroups.List().ToList();
                            }
                            //x.Properties
                            if (Container.GetType().Name != "CloudBlockBlob")
                            json_File.SizeOfFile = 0;
                            IEnumerable<IListBlobItem> listBlobs = Container.ListBlobs();
                            //Console.WriteLine(x.ListBlobs().Count());
                            LoadNamesInContainer namesInContainer = new LoadNamesInContainer();
                            namesInContainer.LoadBlobContents(listBlobs, Container, json_Files,storageAccount);
                        }
                        /*
                         * 1)get all the names
                         * 2)store the names in the name
                        /*foreach (FetchData fetchData in fetchDatas)
                        {
                            fetchData.StorageAccountName = storageAccount.Credentials.AccountName;
                            fetchData.KeyName = storageAccount.Credentials.KeyName;
                            fetchData.ResourceGroup = "0";
                            Console.WriteLine("Press any key to continue...");
                            Console.ReadLine();
                        }*/

                        //foreach (var file in json_Files)
                        // {
                        //  file.StorageAccountName = storageAccount.Credentials.AccountName;
                        // file.KeyValue = storageAccount.Credentials.ExportBase64EncodedKey();
                        //Microsoft.Rest.ServiceClientCredentials serviceClientCredentials = new Microsoft.Rest.ServiceClientCredentials();
                        //file.KeyValue = System.Configuration.ConfigurationManager.AppSettings[key];
                        //Microsoft.Azure.Management.Resources.ResourceManagementClient resourceManagementClient = new Microsoft.Azure.Management.Resources.ResourceManagementClient()
                        //file.SubcriptionId = resourceManagementClient.Credentials.SubscriptionId;
                        //Microsoft.Azure.Management.ResourceManager.Fluent.Resource resource = new Microsoft.Azure.Management.ResourceManager.Fluent.Resource();
                        //if (resource.Name == null)
                        //   Console.WriteLine("This is waste");
                        //Console.WriteLine(resource.Name);
                        //}

                        foreach (var file in json_Files)
                        {
                            if (file.ResourceGroup == null)
                                file.ResourceGroup = ResourceGroupName;
                            if (file.SubcriptionId == null)
                                file.SubcriptionId = SubscriptionID;
                            file.PartitionKey = ResourceGroupName;
                            file.RowKey = StorageAccountName + "!" + file.ScrapeName;
                        }
                    }
                    else
                    {
                        Console.WriteLine("Press any key to exit the sample application.");
                        Console.ReadLine();
                    }
                }

                string ConnectionString = "DefaultEndpointsProtocol=https;AccountName=ravitesthf;AccountKey=vlJTIHsj5mw3jLepkYkOrniLyoRax2vaGK9m9tCt9oYvhn9UQIYvXilg3AYftYHRhpyZqug7tVx5Hxn7YbWHKQ==;EndpointSuffix=core.windows.net";

                Microsoft.WindowsAzure.Storage.CloudStorageAccount storageAccountName = Microsoft.WindowsAzure.Storage.CloudStorageAccount.Parse(ConnectionString);
                Microsoft.WindowsAzure.Storage.Table.CloudTableClient tableClient = storageAccountName.CreateCloudTableClient();
                //tableClient.
                //tableClient.PayloadFormat = TablePayloadFormat.JsonNoMetadata;
                Microsoft.WindowsAzure.Storage.Table.CloudTable table = tableClient.GetTableReference("CustomerInformation");
                table.CreateIfNotExists();
                foreach (Json_File file in json_Files)
                {
                    Microsoft.WindowsAzure.Storage.Table.TableOperation insertOperation = Microsoft.WindowsAzure.Storage.Table.TableOperation.Insert(file);
                    table.Execute(insertOperation);
                }
            }
        }
        catch(Exception e)
        {
            Console.WriteLine(e.Message);
            Console.WriteLine(e.StackTrace);
            Console.WriteLine(e.Source);
        }
    }
}
class D
{
    public async Task Helper()
    {
        List<Json_File> json_Files = new List<Json_File>();
        C c = new C();
       await c.AssignFunctionAsync(json_Files);

        foreach (Json_File jsonFile in json_Files)
        {
            string json = JsonConvert.SerializeObject(jsonFile, Newtonsoft.Json.Formatting.Indented);//Formating is indented so that we don't get all
                                                                                                     //key value pairs in  same line
            Console.WriteLine(json);
        }
        Console.WriteLine("Press a key to exit");
        Console.ReadLine();
    }
}
namespace ConsoleApp8
{
    class Program
    {
        static void Main(string[] args)
        {
            D d = new D();
            d.Helper().Wait();
        }
    }
}