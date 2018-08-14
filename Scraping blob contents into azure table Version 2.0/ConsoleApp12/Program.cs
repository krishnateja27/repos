using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Configuration;
using Newtonsoft.Json;
using System.Xml;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.Azure.Management.ResourceManager.Fluent;
using Microsoft.Azure.Management.ResourceManager.Fluent.Models;
using Microsoft.Azure.Management.Fluent;
using Microsoft.WindowsAzure.Storage.Table;
using Microsoft.Rest.Azure;
using Microsoft.Azure.Management.Storage.Fluent.Models;

//<summary>
//Class having the properties of the Record in WindowsAzure Storage Table.
//</summary>
public class TableRecord : TableEntity
{
    public TableRecord(string ScrapeName, string StorageAccountName)
    {
        RowKey = ScrapeName;
        PartitionKey = StorageAccountName;
    }
    public string ScrapeName { get; set; }
    public string KeyValue { get; set; }  
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
//<summary>
//Static class for storing all the Table Records into the same memory pool.
//</summary>
static class TableRecordsList
{
    public static List<TableRecord> TableRecords;
    static TableRecordsList()
    {
        TableRecords = new List<TableRecord>();
    }
    public static void Add(TableRecord value)
    {
        TableRecords.Add(value);
    }
    public static void Display()
    {
        foreach (var value in TableRecords)
        {
            Console.WriteLine(value);
        }
    }
    public static int Count()
    {
        return TableRecords.Count();
    }
}
//<summary>
//Class object loads all the Table Record properties of CloudBlockBlobs and CloudBlobDirectory within a CloudBlobContainer.
//</summary>
class LoadContentsInContainer
{
    //<summary>
    //Creates a Table Record object for storing Block Blob in a Directory.
    //</summary>
    public TableRecord CreateSubDirectoryBlockBlobTableRecord(IListBlobItem blobItem, int hidelength, 
         CloudBlobContainer container, string actualDirectoryName,CloudStorageAccount storageAccount)
    {
        if (blobItem != null && container != null && storageAccount != null && hidelength > 0 && string.IsNullOrWhiteSpace(actualDirectoryName) != true)
        {
            try
            {
                int length = ((CloudBlob)blobItem).Name.Length;
                var blobTableRecord =
                new TableRecord(((CloudBlob)blobItem).Name.Substring(hidelength, length - hidelength), storageAccount.Credentials.AccountName)
                {
                    ScrapeName = ((CloudBlob)blobItem).Name.Substring(hidelength, length - hidelength),
                    SizeOfFile = ((CloudBlob)blobItem).Properties.Length,
                    Type = blobItem.GetType().Name,
                    Directory = actualDirectoryName,
                    Uri = blobItem.Uri.ToString(),
                    ETagValue = ((CloudBlob)blobItem).Properties.ETag,
                    LeaseDuration = ((CloudBlob)blobItem).Properties.LeaseDuration.ToString(),
                    LeaseStatus = ((CloudBlob)blobItem).Properties.LeaseStatus.ToString(),
                    LeaseState = ((CloudBlob)blobItem).Properties.LeaseState.ToString(),
                    LastModified = ((CloudBlob)blobItem).Properties.LastModified.ToString(),
                    StorageAccountName = storageAccount.Credentials.AccountName,
                    KeyValue = storageAccount.Credentials.ExportBase64EncodedKey(),
                    ContainerName = container.Name
                };
                return blobTableRecord;
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine(ex.Source);
                Console.WriteLine(ex.StackTrace);
            }
        }
        return null;
    }
    //<summary>
    //Loads the Directory Contents (CloudBlockBlobs and CloudBlobDirectories) into Table Recursively.
    //</summary>
    public void LoadDirectoryContentsIntoTable(IEnumerable<IListBlobItem> listBlobs, CloudBlobContainer container,
        int hidelength, string actualDirectoryName, CloudStorageAccount storageAccount)
    {
        if (listBlobs.Count() > 0 && container != null && storageAccount != null 
            && hidelength > 0 && string.IsNullOrWhiteSpace(actualDirectoryName) != true)
        {
            try
            {
                foreach (var blobItem in listBlobs)
                {
                    if (blobItem.GetType().Name != "CloudBlockBlob")
                    {
                        int Subdirectorydisplaylength = (((CloudBlobDirectory)blobItem).Prefix).Length;
                        var directoryTableRecord = 
                        new TableRecord(((CloudBlobDirectory)blobItem).Prefix
                        .Substring(hidelength, Subdirectorydisplaylength - hidelength - 1),
                        storageAccount.Credentials.AccountName)
                        {
                            ScrapeName = ((CloudBlobDirectory)blobItem).Prefix.
                            Substring(hidelength, Subdirectorydisplaylength - hidelength - 1),
                            SizeOfFile = 0,
                            Type = blobItem.GetType().Name,
                            Directory = actualDirectoryName,
                            Uri = blobItem.Uri.ToString(),
                            StorageAccountName = storageAccount.Credentials.AccountName,
                            KeyValue = storageAccount.Credentials.ExportBase64EncodedKey(),
                            ContainerName = container.Name
                        };
                        TableRecordsList.Add(directoryTableRecord);
                        string directoryName = ((CloudBlobDirectory)blobItem).Prefix
                            .Substring(hidelength, Subdirectorydisplaylength - hidelength);
                        var directory = container.GetDirectoryReference(((CloudBlobDirectory)blobItem).Prefix);
                        LoadDirectoryContentsIntoTable(directory.ListBlobs(), container, Subdirectorydisplaylength, 
                        actualDirectoryName +
                        ((CloudBlobDirectory)blobItem).Prefix.Substring(hidelength, Subdirectorydisplaylength - hidelength - 1),
                        storageAccount);
                    }
                    else
                    {
                        var blobTableRecord = CreateSubDirectoryBlockBlobTableRecord
                            (blobItem, hidelength, container, actualDirectoryName, storageAccount);
                        TableRecordsList.Add(blobTableRecord);
                    }
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine(ex.Source);
                Console.WriteLine(ex.StackTrace);
            }
        }
    }
    //<summary>
    //Creates a Table Record object for storing Blob Directory.
    //</summary>
    public TableRecord CreateCloudBlobDirectoryTableRecord(IListBlobItem blobItem, CloudBlobContainer container, 
        CloudStorageAccount storageAccount)
    {
        if (blobItem != null && container != null && storageAccount != null)
        {
            try
            {
                int displayLength = (((CloudBlobDirectory)blobItem).Prefix).Length;
                var directoryTableRecord =
                new TableRecord(((CloudBlobDirectory)blobItem).Prefix.Substring(0, displayLength - 1),
                storageAccount.Credentials.AccountName)
                {
                    ScrapeName = ((CloudBlobDirectory)blobItem).Prefix.Substring(0, displayLength - 1),
                    SizeOfFile = 0,
                    Type = blobItem.GetType().Name,
                    Directory = container.Name,
                    Uri = blobItem.Uri.ToString(),
                    StorageAccountName = storageAccount.Credentials.AccountName,
                    KeyValue = storageAccount.Credentials.ExportBase64EncodedKey(),
                    ContainerName = container.Name
                };
                return directoryTableRecord;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine(ex.Source);
                Console.WriteLine(ex.StackTrace);
            }
        }
        return null;
    }
    //<summary>
    //Creates a Table Record object for storing Block Blob.
    //</summary>
    public TableRecord CreateCloudBlockBlobTableRecord(IListBlobItem blobItem, CloudBlobContainer container, 
        CloudStorageAccount storageAccount)
    {
        
        if (blobItem != null && container != null && storageAccount != null)
        {
            try
            {
                var blobTableRecord = new TableRecord((((CloudBlob)blobItem).Name), storageAccount.Credentials.AccountName)
                {
                    ScrapeName = (((CloudBlob)blobItem).Name),
                    SizeOfFile = ((CloudBlob)blobItem).Properties.Length,
                    Type = blobItem.GetType().Name,
                    Directory = container.Name,
                    Uri = blobItem.Uri.ToString(),
                    ETagValue = ((CloudBlob)blobItem).Properties.ETag,
                    LeaseDuration = ((CloudBlob)blobItem).Properties.LeaseDuration.ToString(),
                    LeaseStatus = ((CloudBlob)blobItem).Properties.LeaseStatus.ToString(),
                    LeaseState = ((CloudBlob)blobItem).Properties.LeaseState.ToString(),
                    LastModified = ((CloudBlob)blobItem).Properties.LastModified.ToString(),
                    StorageAccountName = storageAccount.Credentials.AccountName,
                    KeyValue = storageAccount.Credentials.ExportBase64EncodedKey(),
                    ContainerName = container.Name
                };
                return blobTableRecord;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine(ex.Source);
                Console.WriteLine(ex.StackTrace);
            }
        }
        return null;
    }
    //<summary>
    //Loads 
    //</summary>
    public void LoadBlobContentsInContainer(IEnumerable<IListBlobItem> listBlobs, CloudBlobContainer container, 
        CloudStorageAccount storageAccount)
    {
        if ( listBlobs.Count() > 0 && container != null && storageAccount != null)
        {
            try
            {
                foreach (var blobItem in listBlobs)
                {
                    if (blobItem.GetType().Name == "CloudBlockBlob")
                    {
                        var blobTableRecord = CreateCloudBlockBlobTableRecord(blobItem, container, storageAccount);
                        TableRecordsList.Add(blobTableRecord);
                    }
                    else
                    {
                        var directoryTableRecord = CreateCloudBlobDirectoryTableRecord(blobItem, container, storageAccount);
                        TableRecordsList.Add(directoryTableRecord);
                        int displayLength = (((CloudBlobDirectory)blobItem).Prefix).Length;
                        string directoryPrefix = ((CloudBlobDirectory)blobItem).Prefix.Substring(0, displayLength - 1);
                        string directoryName = ((CloudBlobDirectory)blobItem).Prefix;
                        var directory = container.GetDirectoryReference(directoryPrefix);
                        LoadDirectoryContentsIntoTable(directory.ListBlobs(), container, displayLength,
                            container.Name + "/" + directoryName, storageAccount);
                    }
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine(ex.Source);
                Console.WriteLine(ex.StackTrace);
            }
        }
    }
}
//<summary>
//Class controls the flow of Table Creation mechanism.
//</summary>
class TableCreationController
{
    //<summary>
    //Creates a Table Record object for storing Blob Container.
    //</summary>
    public TableRecord CreateCloudBlobContainerTableRecord(CloudBlobContainer container, CloudStorageAccount storageAccount)
    {
        if(container.Exists() && storageAccount !=null)
        {
           try
           {
                TableRecord containerTableRecord = new TableRecord(container.Name, storageAccount.Credentials.AccountName)
                {
                    ScrapeName = container.Name,
                    Type = container.GetType().Name,
                    Uri = container.Uri.ToString(),
                    ETagValue = container.Properties.ETag.ToString(),
                    LeaseDuration = container.Properties.LeaseDuration.ToString(),
                    LeaseStatus = container.Properties.LeaseStatus.ToString(),
                    LeaseState = container.Properties.LeaseState.ToString(),
                    LastModified = container.Properties.LastModified.ToString(),
                    KeyValue = storageAccount.Credentials.ExportBase64EncodedKey(),
                    StorageAccountName = storageAccount.Credentials.AccountName
                };
                return containerTableRecord;
            }
           catch(Exception ex)
           {
                Console.WriteLine(ex.Message);
                Console.WriteLine(ex.Source);
                Console.WriteLine(ex.StackTrace);
           }
        }
        return null;
    }
    //<summary>
    //Generates a ConnectionString from a given StorageAccountName and Key Value.
    //</summary>
    public string GenerateConnectionString(string storageAccountName, StorageAccountKey key)
    {
        if(storageAccountName != null && key.Value != null)
        {
            return "DefaultEndpointsProtocol=https;AccountName="+storageAccountName+";AccountKey="+key.Value+";EndpointSuffix=core.windows.net";
        }
        return null;
    }
    //<summary>
    //Helps in creating the AzureStorageTable in an asynchronous way.
    //</summary>
    public async Task CreateTableHelperAsync()
    {
        try
        {
            XmlDocument document = new XmlDocument();
            document.Load(@"C:\Users\00001120\source\repos\ConsoleApp5\ConsoleApp5\bin\Debug\Test.xml");
            var appSettings = ConfigurationManager.AppSettings;
            if (appSettings.Count == 0)
            {
                Console.WriteLine("AppSettings is empty");
                Console.WriteLine("Press any key to continue...");
                Console.ReadLine();
            }
            else
            {
                const string clientId = appSettings["ClientID"];
                const string tenantId = appSettings["TenantID"];
                const string clientSecretKey = appSettings["ClientSecretKey"];
                const string subscriptionId = appSettings["SubscriptionID"];

                var creds = SdkContext.AzureCredentialsFactory.FromServicePrincipal(clientId, clientSecretKey, tenantId, AzureEnvironment.AzureGlobalCloud);
                var azure = Azure.Authenticate(creds).WithSubscription(subscriptionId);
                var subscriptionClient = new SubscriptionClient(creds);

                IPage<SubscriptionInner> subscriptionTask = null;
                subscriptionTask = await subscriptionClient.Subscriptions.ListAsync();
                var SubscriptionList = subscriptionTask?.Select(y => y);

                foreach (var subscription in SubscriptionList)
                {
                    Console.WriteLine(subscription.DisplayName);
                    Console.WriteLine(subscription.Id);
                }

                var resourceManagementClient = new ResourceManagementClient(creds) { SubscriptionId = subscriptionId };
                var resourceGroupTask = await resourceManagementClient.ResourceGroups.ListAsync();
                var resourceGroupList = resourceGroupTask?.Select(y => y);

                foreach (var resourceGroup in resourceGroupList)
                {
                    foreach (var storageaccount in azure.StorageAccounts.ListByResourceGroup(resourceGroup.Name))
                    {
                        var key = storageaccount.GetKeys().First();
                        string StorageConnectionString = GenerateConnectionString(storageaccount.Name,key);
                        CloudStorageAccount storageAccount = CloudStorageAccount.Parse(StorageConnectionString);
                        if(CloudStorageAccount.TryParse(StorageConnectionString, out storageAccount))
                        {
                            Console.WriteLine("Connection is established");

                            CloudBlobClient cloudBlobClient = storageAccount.CreateCloudBlobClient();

                            Console.WriteLine("List of blobs in container.");

                            foreach (var container in cloudBlobClient.ListContainers())
                            {

                                TableRecord containerTableRecord = CreateCloudBlobContainerTableRecord(container, storageAccount);

                                /*StorageAccountName = storageAccount.Credentials.AccountName;

                                if (key == "StorageConnectionString3")
                                {
                                 
                                 * /
                                  /*
                                    const string clientId = "6b80f7db-7f71-4ef8-b412-9b2019a18d18";
                                    const string clientsecretkey = "PsilcW9hQPNYQi9raC4t7Mi/kCstwLdEcWcTU+0HLD4=";
                                    const string tenantId = "fa23f4b5-cee9-4c9e-a774-d31b0f10c151";
                                    const string subscriptionId = "d4df43b2-6edc-4263-b1b6-c600b660640f";
                                    */
                                /*containerTableRecord.SubcriptionId = subscriptionId;

                                var creds = SdkContext.AzureCredentialsFactory.FromServicePrincipal(clientId, clientsecretkey, tenantId, AzureEnvironment.AzureGlobalCloud);

                                var azure = Azure.Authenticate(creds).WithSubscription(subscriptionId);

                                var resourecelist = azure.ResourceGroups.List().ToList();

                                var subscriptionClient = new SubscriptionClient(creds);

                                Microsoft.Rest.Azure.IPage<SubscriptionInner> subscriptionTask = null;

                                subscriptionTask = await subscriptionClient.Subscriptions.ListAsync();

                                var SubscriptionList = subscriptionTask?.Select(y => y);

                                foreach (var subscription in SubscriptionList)
                                {
                                    Console.WriteLine(subscription.DisplayName);
                                    Console.WriteLine(subscription.Id);
                                }

                                var resourceManagementClient = new ResourceManagementClient(creds) { SubscriptionId = subscriptionId };

                                var resourceGroupTask = await resourceManagementClient.ResourceGroups.ListAsync();

                                var resourceGroupList = resourceGroupTask?.Select(y => y);

                                foreach (var resourceGroup in resourceGroupList)
                                {
                                    foreach (var storageaccount in azure.StorageAccounts.ListByResourceGroup(resourceGroup.Name))
                                    {
                                        if (storageaccount.Name == storageAccount.Credentials.AccountName)
                                            containerTableRecord.ResourceGroup = resourceGroup.Name;
                                    }
                                }

                                ResourceGroupName = containerTableRecord.ResourceGroup;
                                SubscriptionID = containerTableRecord.SubcriptionId;

                            }*/
                                if (container.GetType().Name != "CloudBlockBlob")
                                    containerTableRecord.SizeOfFile = 0;
                                TableRecordsList.Add(containerTableRecord);
                                IEnumerable<IListBlobItem> listBlobs = container.ListBlobs();
                                LoadContentsInContainer contnetsInContainer = new LoadContentsInContainer();
                                contnetsInContainer.LoadBlobContentsInContainer(listBlobs, container, storageAccount);
                            }
                        }
                    }
                }


                string ResourceGroupName = null;
                string SubscriptionID = null;
                string StorageAccountName = null;
                foreach (var key in appSettings.AllKeys)
                {
                    CloudStorageAccount storageAccount = CloudStorageAccount.Parse(ConfigurationManager.AppSettings[key]);
                    if (CloudStorageAccount.TryParse(ConfigurationManager.AppSettings[key], out storageAccount))
                    {
                       
                        foreach (var TableRecord in TableRecordsList.TableRecords)
                        {
                            if (TableRecord.ResourceGroup == null)
                                TableRecord.ResourceGroup = ResourceGroupName;
                            if (TableRecord.SubcriptionId == null)
                                TableRecord.SubcriptionId = SubscriptionID;
                            TableRecord.PartitionKey = ResourceGroupName;
                            TableRecord.RowKey = StorageAccountName + "!" + TableRecord.ScrapeName;
                        }
                    }
                    else
                    {
                        Console.WriteLine("Press any key to exit the sample application.");
                        Console.ReadLine();
                    }
                }

                string ConnectionString = "DefaultEndpointsProtocol=https;AccountName=ravitesthf;AccountKey=vlJTIHsj5mw3jLepkYkOrniLyoRax2vaGK9m9tCt9oYvhn9UQIYvXilg3AYftYHRhpyZqug7tVx5Hxn7YbWHKQ==;EndpointSuffix=core.windows.net";

                CloudStorageAccount storageAccountName = CloudStorageAccount.Parse(ConnectionString);
                CloudTableClient tableClient = storageAccountName.CreateCloudTableClient();
                CloudTable table = tableClient.GetTableReference("CustomerInformation");
                table.CreateIfNotExists();
                foreach (var TableRecord in TableRecordsList.TableRecords)
                {
                    TableOperation insertOperation = TableOperation.Insert(TableRecord);
                    table.Execute(insertOperation);
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            Console.WriteLine(ex.StackTrace);
            Console.WriteLine(ex.Source);
        }
    }
}
class AzureStorageTable
{
    //<summary>
    //Creates the AzureStorageTable in an asynchronous way.
    //</summary>
    public async Task CreateTable()
    {
        TableCreationController tableCreationController = new TableCreationController();
        await tableCreationController.CreateTableHelperAsync();

        foreach (var tableRecord in TableRecordsList.TableRecords)
        {
            string json = JsonConvert.SerializeObject(tableRecord, Newtonsoft.Json.Formatting.Indented);
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
            AzureStorageTable azureStorageTable = new AzureStorageTable();
            azureStorageTable.CreateTable().Wait();
        }
    }
}
