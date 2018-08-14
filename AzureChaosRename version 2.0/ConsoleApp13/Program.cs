using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.WindowsAzure.Storage.Table;
using System.Configuration;

//<summary>
//Class having the properties of the Record in WindowsAzure Storage Table.
//</summary>
public class TableRecord : TableEntity
{
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
namespace AzureFaultInjection
{
    //<summary>
    //Class object gives acces to the fields required to rollback the same blob name.
    //</summary>
    class RestoreOperationFields
    {
        public bool RenameOperation { get; set; }
        public TableResult RetreiveResult { get; set; }
        public string NewFileName { get; set; }
        public string ScrapeName { get; set; }
    }
    //<summary>
    //Static class object creates a list of the above RestoreOperationField Objects used for rollback operation.
    //</summary>
    static class RestoreOperationFieldsList
    {
        public static List<RestoreOperationFields> RestoreOperationFields;
        static RestoreOperationFieldsList()
        {
            RestoreOperationFields = new List<RestoreOperationFields>();
        }
        public static void Add(RestoreOperationFields value)
        {
            RestoreOperationFields.Add(value);
        }
        public static void Display()
        {
            foreach (var value in RestoreOperationFields)
            {
                Console.WriteLine(value);
            }
        }
        public static int Count()
        {
            return RestoreOperationFields.Count();
        }
    }
    //<summary>
    //Class object creates the chaos 'rename'.
    //</summary>
    class ChaosRename
    {
        //<summary>
        //Function helps in restoring back (rollback) the original filename. 
        //</summary>
        public static async Task<bool> RestoreHelperAsync(TableResult retreiveResult, string newFileName, string ScrapeName)
        {
            try
            {
                string containerName = ((TableRecord)retreiveResult.Result).ContainerName;
                string storageAccountName = ((TableRecord)retreiveResult.Result).StorageAccountName;
                string storageAccessKey = ((TableRecord)retreiveResult.Result).KeyValue;

                if (containerName != null)
                {
                    string StorageConnectionString = "DefaultEndpointsProtocol=https;AccountName=afimssav;AccountKey=ZiLCuZ6WvK8aZYLzXUNbTn+Tjl1hhPMaOeBhIdi00Urk8nTl3fwAsyCkXOXXcYzsjlBDDNG1Sahgte0q/3SJgQ==;EndpointSuffix=core.windows.net";
                    CloudStorageAccount storageAccount = CloudStorageAccount.Parse(StorageConnectionString);
                    CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();
                    CloudBlobContainer container = blobClient.GetContainerReference(containerName);
                    switch (((TableRecord)retreiveResult.Result).Type)
                    {
                        case "CloudBlockBlob":
                            {
                                CloudBlockBlob blobCopy = container.GetBlockBlobReference(ScrapeName);
                                if (!await blobCopy.ExistsAsync())
                                {
                                    CloudBlockBlob blob = container.GetBlockBlobReference(newFileName);

                                    if (await blob.ExistsAsync())
                                    {
                                        await blobCopy.StartCopyAsync(blob);
                                        await blob.DeleteIfExistsAsync();
                                        return true;
                                    }
                                    else return false;
                                }
                                break;
                            }
                        case "CloudBlobContainer":
                            {
                                if (containerName != null)
                                {
                                    var permissions = container.GetPermissions();
                                    permissions.PublicAccess = BlobContainerPublicAccessType.Container;
                                    return true;
                                }
                                break;
                            }
                        case "CloudBlobDirectory":
                            {
                                break;
                            }
                    }
                }
                return false;
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine(ex.Source);
                Console.WriteLine(ex.StackTrace);
                return false;
            }
        }
        //<summary>
        //Function helps in renaming task. 
        //</summary>
        public static async Task<bool> RenameHelperAsync(TableResult retreiveResult, string newFileName)
        {
            try
            {
                string scrapeName = ((TableRecord)retreiveResult.Result).ScrapeName;
                string container = ((TableRecord)retreiveResult.Result).ContainerName;
                string storageAccountName = ((TableRecord)retreiveResult.Result).StorageAccountName;
                string storageAccessKey = ((TableRecord)retreiveResult.Result).KeyValue;

                if (container != null)
                {
                    string StorageConnectionString = "DefaultEndpointsProtocol=https;AccountName=afimssav;AccountKey=ZiLCuZ6WvK8aZYLzXUNbTn+Tjl1hhPMaOeBhIdi00Urk8nTl3fwAsyCkXOXXcYzsjlBDDNG1Sahgte0q/3SJgQ==;EndpointSuffix=core.windows.net";
                    CloudStorageAccount storageAccount = CloudStorageAccount.Parse(StorageConnectionString);
                    CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();
                    CloudBlobContainer containerName = blobClient.GetContainerReference(container);
                    switch (((TableRecord)retreiveResult.Result).Type)
                    {
                        case "CloudBlockBlob":
                            {
                                string fileName = scrapeName;
                                CloudBlockBlob blobCopy = containerName.GetBlockBlobReference(newFileName);
                                if (!await blobCopy.ExistsAsync())
                                {
                                    CloudBlockBlob blob = containerName.GetBlockBlobReference(fileName);

                                    if (await blob.ExistsAsync())
                                    {
                                        await blobCopy.StartCopyAsync(blob);
                                        await blob.DeleteIfExistsAsync();
                                        return true;
                                    }
                                    else return false;
                                }
                                break;
                            }
                        case "CloudBlobContainer":
                            {
                                {
                                    var permissions = containerName.GetPermissions();
                                    permissions.PublicAccess = BlobContainerPublicAccessType.Off;
                                    return true;
                                }
                            }
                        case "CloudBlobDirectory":
                            {
                                break;
                            }

                    }
                }
                return false;
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine(ex.Source);
                Console.WriteLine(ex.StackTrace);
                return false;
            }
        }
        //<summary>
        //Select a random PartitionKey
        //</summary>
        public string GetRandomPartitionKey(CloudTable table)
        {
            try
            {
                Dictionary<int, string> PartitionKeys = new Dictionary<int, string>();
                int num = 1;
                TableContinuationToken token = null;
                var entityRecords = new List<TableRecord>();
                do
                {
                    var queryResult = table.ExecuteQuerySegmented(new TableQuery<TableRecord>(), token);
                    entityRecords.AddRange(queryResult.Results);
                    token = queryResult.ContinuationToken;
                } while (token != null);
                foreach (var entity in entityRecords)
                {
                    PartitionKeys.Add(num, entity.PartitionKey);
                    num++;
                }
                int size = PartitionKeys.Count;
                Random random = new Random();
                const int MinValue = 1;
                int randomNumber = random.Next(MinValue, size + 1);
                string RandomPartitionKey = PartitionKeys[randomNumber];
                return RandomPartitionKey;
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine(ex.Source);
                Console.WriteLine(ex.StackTrace);
                return null;
            }
        }
        //<summary>
        //Select a random RowKey from a given PartitionKey
        //</summary>
        public string GetRandomRowKey(CloudTable table, string PartitionKey)
        {
            try
            {
                TableContinuationToken continuationToken = null;
                var EntityRecords = new List<TableRecord>();
                do
                {
                    var rangeQueryResult = table
                    .ExecuteQuerySegmented(new TableQuery<TableRecord>()
                    .Where(TableQuery
                    .GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, PartitionKey)), continuationToken);
                    EntityRecords.AddRange(rangeQueryResult.Results);
                    continuationToken = rangeQueryResult.ContinuationToken;
                } while (continuationToken != null);

                Dictionary<int, string> RowKeys = new Dictionary<int, string>();
                int number = 1;
                foreach (var Entity in EntityRecords)
                {
                    RowKeys.Add(number, Entity.RowKey);
                    number++;
                }
                int subSize = RowKeys.Count;
                Random rand = new Random();
                const int MinValue = 1;
                int rand_number = rand.Next(MinValue, subSize + 1);
                string RandomRowKey = RowKeys[rand_number];
                return RandomRowKey;
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine(ex.Source);
                Console.WriteLine(ex.StackTrace);
                return null;
            }
        }
        //<summary>
        //Gets a random table record in the form of a TableResult.
        //</summary>
        public TableResult GetRandomTableResult()
        {
            try
            {
                XmlDocument document = new XmlDocument();
                document.Load(@"C:\Users\Administrator\source\repos\ConsoleApp5\ConsoleApp5\bin\Debug\Test.xml");
                var appSettings = ConfigurationManager.AppSettings;
                TableResult retreiveResult = null;
                if (appSettings.Count == 0)
                {
                    Console.WriteLine("The appsettings is empty\nEnter a valid ConnectionString in AppConfig file");
                    Console.WriteLine("Press a key to exit..");
                    Console.ReadLine();
                }
                else
                {
                    foreach (var key in appSettings.AllKeys)
                    {
                        CloudStorageAccount storageAccount = CloudStorageAccount.Parse(ConfigurationManager.AppSettings[key]);
                        CloudTableClient tableClient = storageAccount.CreateCloudTableClient();
                        CloudTable table = tableClient.GetTableReference("CustomerInformation");
                        if (table.Exists())
                        {
                            Console.WriteLine("The table exists..");
                            string RandomPartitionKey = GetRandomPartitionKey(table);
                            TableOperation retreiveOperation = TableOperation.Retrieve<TableRecord>(RandomPartitionKey, GetRandomRowKey(table, RandomPartitionKey));
                            retreiveResult = table.Execute(retreiveOperation);
                        }
                        else
                        {
                            Console.WriteLine("Table doesn't exists\nPress a key to exit....");
                            Console.ReadLine();
                        }
                    }
                }
                return retreiveResult;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine(ex.Source);
                Console.WriteLine(ex.StackTrace);
                return null;
            }
        }
        //<summary>
        //Function does the task of renaming in an asynchronous way.
        //</summary>
        public static async Task<bool> RenameAsync(TableResult retreiveResult)
        {
            try
            {
                bool renameOperation = false;
                if (retreiveResult.Result != null)
                {
                    Console.WriteLine("Retreive result is done successfully");
                    string scrapeName = ((TableRecord)retreiveResult.Result).ScrapeName;
                    Console.WriteLine(scrapeName);
                    string newFileName = "renamed" + scrapeName;

                    if (((TableRecord)retreiveResult.Result).Type != "CloudBlockBlob")
                        Console.WriteLine("The selected file is not blob file");

                    renameOperation = await RenameHelperAsync(retreiveResult, newFileName);

                }
                return renameOperation;
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine(ex.Source);
                Console.WriteLine(ex.StackTrace);
                return false;
            }
        }
        //<summary>
        //Does the task of randomly selecting a table record and generates the 'rename' chaos in an asynchronous way.
        //</summary>
        public async void RandomRenamingAsync()
        {
            try
            {
                var result = GetRandomTableResult();
                if (result != null)
                {
                    bool renameOperation = await RenameAsync(result);
                    if (renameOperation)
                    {
                        string scrapeName = ((TableRecord)result.Result).ScrapeName;
                        string newFileName = "renamed" + scrapeName;
                        RestoreOperationFields restoreOperationField = new RestoreOperationFields
                        {
                            ScrapeName = scrapeName,
                            RetreiveResult = result,
                            RenameOperation = renameOperation,
                            NewFileName = newFileName
                        };
                        RestoreOperationFieldsList.Add(restoreOperationField);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine(ex.Source);
                Console.WriteLine(ex.StackTrace);
            }
        }
        //<summary>
        //Rollback to the original filename caused by the RenameChaos.
        //</summary>
        public async void RestoreAsync(RestoreOperationFields restoreOperationField,double renameConfigurationTime)
        {
            try
            {
                TableResult retreiveResult = restoreOperationField.RetreiveResult;
                if (restoreOperationField.RenameOperation)
                {
                    Console.WriteLine("The rename operation is completed");
                    //do use database
                    string StorageAccountName = ((TableRecord)retreiveResult.Result).StorageAccountName;
                    string StorageAccessKey = ((TableRecord)retreiveResult.Result).KeyValue;

                    if (((TableRecord)retreiveResult.Result).ContainerName != null)
                    {
                        string StorageConnectionString = "DefaultEndpointsProtocol=https;AccountName=afimssav;AccountKey=ZiLCuZ6WvK8aZYLzXUNbTn+Tjl1hhPMaOeBhIdi00Urk8nTl3fwAsyCkXOXXcYzsjlBDDNG1Sahgte0q/3SJgQ==;EndpointSuffix=core.windows.net";
                        CloudStorageAccount storageaccount = CloudStorageAccount.Parse(StorageConnectionString);
                        CloudBlobClient blobClient = storageaccount.CreateCloudBlobClient();
                        CloudBlobContainer containername = blobClient.GetContainerReference(((TableRecord)retreiveResult.Result).ContainerName);
                        string newFileName = restoreOperationField.NewFileName;
                        CloudBlockBlob blob = containername.GetBlockBlobReference(newFileName);
                        if (blob.Exists())
                        {
                            Console.WriteLine("The rename blob has been created");
                        }

                        TimeSpan timeSpan = TimeSpan.FromMinutes(renameConfigurationTime);
                        if (DateTime.UtcNow - blob.Properties.LastModified > timeSpan)
                        {
                            bool restoreOperation = await RestoreHelperAsync(retreiveResult, newFileName, restoreOperationField.ScrapeName);
                            if (restoreOperation)
                                Console.WriteLine("The restore has been done");
                        }
                    }
                }
                else
                    Console.WriteLine("The rename task is not done yet..");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine(ex.Source);
                Console.WriteLine(ex.StackTrace);
            }
        }
        static void Main(string[] args)
        {
            try
            {
                ChaosRename chaosRename = new ChaosRename();
                //In Minutes
                double renameConfigurationTime = 1;
                //<summary>
                //Checks whether there are any files which are renamed.
                //If so, then check whether the file rename configuration time has expired.
                //If so, rollback to the original name.
                //</summary>
                if (RestoreOperationFieldsList.Count() > 0)
                {
                    foreach (var blobdata in RestoreOperationFieldsList.RestoreOperationFields)
                    {
                        chaosRename.RestoreAsync(blobdata, renameConfigurationTime);
                    }
                }
                //<summary>
                //Select a random file and do renaming operation.
                //</summary>
                chaosRename.RandomRenamingAsync();
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