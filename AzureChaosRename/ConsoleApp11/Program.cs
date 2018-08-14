using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;

public class Json_File : Microsoft.WindowsAzure.Storage.Table.TableEntity
{
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
namespace AzureFaultInjection
{

    class RestoreOperationFields
    {
        //add the fields renameOperation,retreive result,newFileName,scrapeName to the list of new class
        public bool RenameOperation { get; set; }
        public Microsoft.WindowsAzure.Storage.Table.TableResult RetreiveResult { get; set; }
        public string NewFileName { get; set; }
        public string ScrapeName { get; set; }
    }
    static class RestoreOperationFieldsList
    {
        public static List<RestoreOperationFields> restoreOperationFields;
        static RestoreOperationFieldsList()
        {
            restoreOperationFields = new List<RestoreOperationFields>();
        }
        public static void Add(RestoreOperationFields value)
        {
            restoreOperationFields.Add(value);
        }
        public static void Display()
        {
            foreach (var value in restoreOperationFields)
            {
                Console.WriteLine(value);
            }
        }
        public static int Count()
        {
            return restoreOperationFields.Count();
        }
    }
    class ChaosRename
    {
        public static async Task<bool> RestoreHelperAsync(Microsoft.WindowsAzure.Storage.Table.TableResult retreiveResult, string newFileName, string ScrapeName)
        {
            string Container = ((Json_File)retreiveResult.Result).ContainerName;
            string StorageAccountName = ((Json_File)retreiveResult.Result).StorageAccountName;
            string StorageAccessKey = ((Json_File)retreiveResult.Result).KeyValue;

            if (Container != null)
            {
                // DefaultEndpointsProtocol = https; AccountName = afimssav; AccountKey = ZiLCuZ6WvK8aZYLzXUNbTn + Tjl1hhPMaOeBhIdi00Urk8nTl3fwAsyCkXOXXcYzsjlBDDNG1Sahgte0q / 3SJgQ ==; EndpointSuffix = core.windows.net
                string StorageConnectionString = "DefaultEndpointsProtocol=https;AccountName=afimssav;AccountKey=ZiLCuZ6WvK8aZYLzXUNbTn+Tjl1hhPMaOeBhIdi00Urk8nTl3fwAsyCkXOXXcYzsjlBDDNG1Sahgte0q/3SJgQ==;EndpointSuffix=core.windows.net";
                CloudStorageAccount storageaccount = CloudStorageAccount.Parse(StorageConnectionString);
                CloudBlobClient blobClient = storageaccount.CreateCloudBlobClient();
                CloudBlobContainer containername = blobClient.GetContainerReference(Container);
                // var permissions = containername.GetPermissions();
                //permissions.PublicAccess = BlobContainerPublicAccessType.Off;
                switch (((Json_File)retreiveResult.Result).Type)
                {
                    case "CloudBlockBlob":
                        {
                            CloudBlockBlob blobCopy = containername.GetBlockBlobReference(ScrapeName);
                            if (!await blobCopy.ExistsAsync())
                            {
                                CloudBlockBlob blob = containername.GetBlockBlobReference(newFileName);

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
                            if(containername != null)
                            {
                                var permissions = containername.GetPermissions();
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
            //Console.WriteLine(((Json_File)retreiveResult.Result).ScrapeName);
            return false;
        }
        public static async Task<bool> RenameHelperAsync(Microsoft.WindowsAzure.Storage.Table.TableResult retreiveResult, string newFileName)
        {
            string scrapeName = ((Json_File)retreiveResult.Result).ScrapeName;
            string Container = ((Json_File)retreiveResult.Result).ContainerName;
            string StorageAccountName = ((Json_File)retreiveResult.Result).StorageAccountName;
            string StorageAccessKey = ((Json_File)retreiveResult.Result).KeyValue;

            if (Container != null)
            {
                // DefaultEndpointsProtocol = https; AccountName = afimssav; AccountKey = ZiLCuZ6WvK8aZYLzXUNbTn + Tjl1hhPMaOeBhIdi00Urk8nTl3fwAsyCkXOXXcYzsjlBDDNG1Sahgte0q / 3SJgQ ==; EndpointSuffix = core.windows.net
                string StorageConnectionString = "DefaultEndpointsProtocol=https;AccountName=afimssav;AccountKey=ZiLCuZ6WvK8aZYLzXUNbTn+Tjl1hhPMaOeBhIdi00Urk8nTl3fwAsyCkXOXXcYzsjlBDDNG1Sahgte0q/3SJgQ==;EndpointSuffix=core.windows.net";
                CloudStorageAccount storageaccount = CloudStorageAccount.Parse(StorageConnectionString);
                CloudBlobClient blobClient = storageaccount.CreateCloudBlobClient();
                CloudBlobContainer containername = blobClient.GetContainerReference(Container);
                // var permissions = containername.GetPermissions();
                //permissions.PublicAccess = BlobContainerPublicAccessType.Off;
                switch (((Json_File)retreiveResult.Result).Type)
                {
                    case "CloudBlockBlob":
                        {
                            string fileName = scrapeName;
                            CloudBlockBlob blobCopy = containername.GetBlockBlobReference(newFileName);
                            if (!await blobCopy.ExistsAsync())
                            {
                                CloudBlockBlob blob = containername.GetBlockBlobReference(fileName);

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
                            var permissions = containername.GetPermissions();
                            permissions.PublicAccess = BlobContainerPublicAccessType.Off;
                            break;
                        }
                    case "CloudBlobDirectory":
                        {
                            break;
                        }

                }
            }
            //Console.WriteLine(((Json_File)retreiveResult.Result).ScrapeName);
            return false;
        }

        public static Microsoft.WindowsAzure.Storage.Table.TableResult GetRandomTableResult()
        {
            //try
            //{
            Microsoft.WindowsAzure.Storage.Table.TableResult retreiveResult = null;
            XmlDocument document = new XmlDocument();
            document.Load(@"C:\Users\Administrator\source\repos\ConsoleApp5\ConsoleApp5\bin\Debug\Test.xml");
            var appSettings = System.Configuration.ConfigurationManager.AppSettings;
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
                    Microsoft.WindowsAzure.Storage.CloudStorageAccount storageAccount = Microsoft.WindowsAzure.Storage.CloudStorageAccount.Parse(System.Configuration.ConfigurationManager.AppSettings[key]);
                    Microsoft.WindowsAzure.Storage.Table.CloudTableClient tableClient = storageAccount.CreateCloudTableClient();
                    Microsoft.WindowsAzure.Storage.Table.CloudTable table = tableClient.GetTableReference("CustomerInformation");
                    //table.CreateIfNotExists();
                    /*foreach (Json_File file in json_Files)
                    {
                        Microsoft.WindowsAzure.Storage.Table.TableOperation insertOperation = Microsoft.WindowsAzure.Storage.Table.TableOperation.Insert(file);
                        table.Execute(insertOperation);
                    }
                    */
                    if (table.Exists())
                    {
                        Console.WriteLine("The table exists..");
                        //Microsoft.WindowsAzure.Storage.Table.EntityProperty.CreateEntityPropertyFromObject
                        Dictionary<int, string> Partition_Keys = new Dictionary<int, string>();
                        int num = 1;
                        Microsoft.WindowsAzure.Storage.Table.TableContinuationToken token = null;
                        var entities = new List<Json_File>();
                        do
                        {
                            var queryResult = table.ExecuteQuerySegmented(new Microsoft.WindowsAzure.Storage.Table.TableQuery<Json_File>(), token);
                            entities.AddRange(queryResult.Results);
                            token = queryResult.ContinuationToken;
                        } while (token != null);
                        foreach (var file in entities)
                        {
                            Partition_Keys.Add(num, file.PartitionKey);
                            num++;
                        }
                        int size = Partition_Keys.Count;
                        Random random = new Random();
                        int random_number = random.Next(1, size + 1);
                        string RandomPartitionKey = Partition_Keys[random_number];

                        Microsoft.WindowsAzure.Storage.Table.TableContinuationToken continuationToken = null;
                        var Entities = new List<Json_File>();
                        do
                        {
                            var rangeQueryResult = table
                            .ExecuteQuerySegmented(new Microsoft.WindowsAzure.Storage.Table.TableQuery<Json_File>()
                            .Where(Microsoft.WindowsAzure.Storage.Table.TableQuery
                            .GenerateFilterCondition("PartitionKey",
                            Microsoft.WindowsAzure.Storage.Table.QueryComparisons.Equal, RandomPartitionKey)),
                            continuationToken);
                            Entities.AddRange(rangeQueryResult.Results);
                            continuationToken = rangeQueryResult.ContinuationToken;
                        } while (continuationToken != null);

                        Dictionary<int, string> Row_Keys = new Dictionary<int, string>();
                        int number = 1;
                        foreach (var file in Entities)
                        {
                            Row_Keys.Add(number, file.RowKey);
                            number++;
                        }
                        int sub_size = Row_Keys.Count;

                        Random rand = new Random();
                        int rand_number = rand.Next(1, sub_size + 1);

                        string RandomRowKey = Row_Keys[rand_number];

                        Microsoft.WindowsAzure.Storage.Table.TableOperation retreiveOperation = Microsoft.WindowsAzure.Storage.Table.TableOperation.Retrieve<Json_File>(RandomPartitionKey, RandomRowKey);

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
            /*catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            Console.WriteLine(ex.StackTrace);
            Console.WriteLine(ex.InnerException);
            Console.WriteLine(ex.Source);
            Console.WriteLine("Press a key to exit...");
            Console.ReadLine();
        }
        */
        }
        public static async Task<bool> RenameAsync(Microsoft.WindowsAzure.Storage.Table.TableResult retreiveResult)
        {
            bool renameOperation = false;
            if (retreiveResult.Result != null)
            {

                Console.WriteLine("Retreive result is done successfully");
                string scrapeName = ((Json_File)retreiveResult.Result).ScrapeName;
                Console.WriteLine(scrapeName);
                string newFileName = "renamed" + scrapeName;

                if (((Json_File)retreiveResult.Result).Type != "CloudBlockBlob")
                    Console.WriteLine("The selected file is not blob file");



                renameOperation = await RenameHelperAsync(retreiveResult, newFileName);

            }
            return renameOperation;
        }
        public async void RandomRenamingAsync()
        {
            var result = GetRandomTableResult();
            if (result != null)
            {
                bool renameOperation = await RenameAsync(result);
                if (renameOperation)
                {
                    string scrapeName = ((Json_File)result.Result).ScrapeName;
                    string newFileName = "renamed" + scrapeName;
                    RestoreOperationFields restoreOperationField = new RestoreOperationFields();
                    restoreOperationField.ScrapeName = scrapeName;
                    restoreOperationField.RetreiveResult = result;
                    restoreOperationField.RenameOperation = renameOperation;
                    restoreOperationField.NewFileName = newFileName;
                    RestoreOperationFieldsList.Add(restoreOperationField);
                    //add the fields renameOperation,retreive result,newFileName,scrapeName to the list of new class

                }
            }
        }
        public async void RestoreAsync(RestoreOperationFields restoreOperationField)
        {
            string scrapeName = restoreOperationField.ScrapeName;
            Microsoft.WindowsAzure.Storage.Table.TableResult retreiveResult = restoreOperationField.RetreiveResult;
            bool renameOperation = restoreOperationField.RenameOperation;
            string newFileName = restoreOperationField.NewFileName;
            if (renameOperation)
            {
                Console.WriteLine("The rename operation is completed");
                //do use database
                string Container = ((Json_File)retreiveResult.Result).ContainerName;
                string StorageAccountName = ((Json_File)retreiveResult.Result).StorageAccountName;
                string StorageAccessKey = ((Json_File)retreiveResult.Result).KeyValue;

                if (Container != null)
                {
                    string StorageConnectionString = "DefaultEndpointsProtocol=https;AccountName=afimssav;AccountKey=ZiLCuZ6WvK8aZYLzXUNbTn+Tjl1hhPMaOeBhIdi00Urk8nTl3fwAsyCkXOXXcYzsjlBDDNG1Sahgte0q/3SJgQ==;EndpointSuffix=core.windows.net";
                    CloudStorageAccount storageaccount = CloudStorageAccount.Parse(StorageConnectionString);
                    CloudBlobClient blobClient = storageaccount.CreateCloudBlobClient();
                    CloudBlobContainer containername = blobClient.GetContainerReference(Container);
                    CloudBlockBlob blob = containername.GetBlockBlobReference(newFileName);
                    if (blob.Exists())
                        Console.WriteLine("The rename blob has been created");
                    TimeSpan timeSpan = TimeSpan.FromMinutes(1);
                    if (DateTime.UtcNow - blob.Properties.LastModified > timeSpan)
                    {
                        bool restoreOperation = await RestoreHelperAsync(retreiveResult, newFileName, scrapeName);
                        if (restoreOperation)
                            Console.WriteLine("The restore has been done");
                    }
                    //if present time - newfileblobtime is > 1hr then call for restore operation
                    //if(blob.Properties.LastModified - (DateTimeOffset?)((Json_File)retreiveResult.Result).LastModified > )
                }
            }
            else
                Console.WriteLine("The rename task is not done yet..");
        }
        static void Main(string[] args)
        {
            ChaosRename chaosRename = new ChaosRename();
            //1
            if (RestoreOperationFieldsList.Count() > 0)
            {
               foreach (var blobdata in RestoreOperationFieldsList.restoreOperationFields)
               {
                    chaosRename.RestoreAsync(blobdata);
               }
            }
            //2
            //create a  list of new class
            chaosRename.RandomRenamingAsync();
        }
    }
}

