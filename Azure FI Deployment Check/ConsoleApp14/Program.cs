using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;
using System.Web.Http;
using Microsoft.WindowsAzure.Storage;
using System.Xml;
using Microsoft.Azure.Management.Fluent;
using Microsoft.Azure.Management.ResourceManager.Fluent;
using Microsoft.Azure.Management.ResourceManager.Fluent.Models;
using Microsoft.Azure.Management.ResourceManager.Fluent.Authentication;
using Microsoft.Azure.Management.Storage;
using Microsoft.Azure.Management.Storage.Fluent.Models;
using Microsoft.WindowsAzure.Storage.Blob;


namespace System.Web.Http
{
    //
    // Summary:
    //     An exception that allows for a given System.Net.Http.HttpResponseMessage to be
    //     returned to the client.
    public class HttpResponseException : Exception
    {
        //
        // Summary:
        //     Initializes a new instance of the System.Web.Http.HttpResponseException class.
        //
        // Parameters:
        //   statusCode:
        //     The status code of the response.
        public HttpResponseException(HttpStatusCode statusCode)
        {
            Console.WriteLine(statusCode);
        }
        //
        // Summary:
        //     Initializes a new instance of the System.Web.Http.HttpResponseException class.
        //
        // Parameters:
        //   response:
        //     The HTTP response to return to the client.
        public HttpResponseException(HttpResponseMessage response)
        {
            Console.WriteLine(response);
        }
        //
        // Summary:
        //     Gets the HTTP response to return to the client.
        //
        // Returns:
        //     The System.Net.Http.HttpResponseMessage that represents the HTTP response.
        public HttpResponseMessage Response { get; }
    }
}

namespace UpdateDeploymentService
{
    class DeploymentStatus
    {
        private const string CommonName = "azurefi";
        private const string TargetStorageAccountName = "krishnateja";
        private const string ResourceGroupName = "afimsv";
        private const string TargetResourceGroupName = "krishna";

        private static string GetWebAppResourceId(string subscriptionId,string webAppName)
        {
            string webAppResourceId = "/subscriptions/" + subscriptionId + "/resourceGroups/" + ResourceGroupName + "/providers/Microsoft.Web/sites/" + webAppName;
            return webAppResourceId;
        }

        private static string GetTargetResourceGroupId(string subscriptionId, string targetResourceGroupName)
        {
            return ("/subscriptions/" + subscriptionId + "/resourceGroups/" + targetResourceGroupName);
        }
        private static AzureCredentials GetAzureCredentials(string clientId, string clientSecret, string tenantId)
        {
            return SdkContext.AzureCredentialsFactory
                            .FromServicePrincipal(clientId, clientSecret, tenantId, AzureEnvironment.AzureGlobalCloud);
        }
        public static IResourceManagementClient GetResourceManagementClientClient(string clientId, string clientSecret, string tenantId, string subscriptionId)
        {
            var azureCredentials = GetAzureCredentials(clientId, clientSecret, tenantId);
            return azureCredentials == null ? null : new ResourceManagementClient(azureCredentials)
            {
                SubscriptionId = subscriptionId
            };
        }
        public static async Task<IEnumerable<ResourceGroupInner>> GetResourceGroups(string tenantId, string clientId, string clientSecret, string subscriptionId)
        {
            if (string.IsNullOrWhiteSpace(clientId) ||
                string.IsNullOrWhiteSpace(clientSecret) ||
                string.IsNullOrWhiteSpace(tenantId) ||
                string.IsNullOrWhiteSpace(subscriptionId))
            {
                throw new HttpResponseException(HttpStatusCode.BadRequest);
            }

            var resourceManagementClient = GetResourceManagementClientClient(clientId,
                clientSecret,
                tenantId, subscriptionId);
            var resourceGroupTask = await resourceManagementClient.ResourceGroups.ListAsync();

            var resourceGroupList = resourceGroupTask?.Select(x => x);

            return resourceGroupList;
        }

        //<summary>
        //Generates a unique hash for a given string input.
        //MD5CryptoServiceProvider class object is used to generate a unique sequence of bytes.
        //These bytes are added to hash 'string' in the following fashion.
        //Each byte of data is converted to string in hexadecimal format (rounded off to 2 digits each).
        //</summary>
        private static string GetUniqueHash(string input)
        {
            StringBuilder hash = new StringBuilder();
            MD5CryptoServiceProvider md5Provider = new MD5CryptoServiceProvider();
            byte[] bytes = md5Provider.ComputeHash(new UTF8Encoding().GetBytes(input));

            foreach (var t in bytes)
            {
                hash.Append(t.ToString("x2"));
            }
            return hash.ToString().Substring(0, 24);
        }
        public static string GenerateConnectionString(string storageAccountName, StorageAccountKey key)
        {
            if (storageAccountName != null && key.Value != null)
            {
                return "DefaultEndpointsProtocol=https;AccountName=" + storageAccountName + ";AccountKey=" + key.Value + ";EndpointSuffix=core.windows.net";
            }
            return null;
        }
        public static async Task<List<CloudBlobContainer>> ListContainersAsync(CloudBlobClient cloudBlobClient)
        {
            BlobContinuationToken continuationToken = null;
            List<CloudBlobContainer> results = new List<CloudBlobContainer>();
            do
            {
                var response = await cloudBlobClient.ListContainersSegmentedAsync(continuationToken);
                continuationToken = response.ContinuationToken;
                results.AddRange(response.Results);
            }
            while (continuationToken != null);
            return results;
        }
        public static async Task<List<IListBlobItem>> ListBlobsAsync(CloudBlobContainer cloudBlobContainer)
        {
            BlobContinuationToken continuationToken = null;
            List<IListBlobItem> results = new List<IListBlobItem>();
            do
            {
                var response = await cloudBlobContainer.ListBlobsSegmentedAsync(continuationToken);
                continuationToken = response.ContinuationToken;
                results.AddRange(response.Results);
            }
            while (continuationToken != null);
            return results;
        }
        public static async Task<List<IListBlobItem>> ListBlobsInDirectoryAsync(CloudBlobDirectory cloudBlobDirectory)
        {
            BlobContinuationToken continuationToken = null;
            List<IListBlobItem> results = new List<IListBlobItem>();
            do
            {
                var response = await cloudBlobDirectory.ListBlobsSegmentedAsync(continuationToken);
                continuationToken = response.ContinuationToken;
                results.AddRange(response.Results);
            }
            while (continuationToken != null);
            return results;
        }
        public static async Task<bool> CreateCloudBlobContainer(CloudBlobContainer cloudBlobContainer,CloudStorageAccount cloudStorageAccount)
        {
            var appSettings = ConfigurationManager.AppSettings;
            string clientId = appSettings["ClientID"];
            string clientSecret = appSettings["ClientSecretKey"];
            string tenantId = appSettings["TenantID"];
            string subscriptionId = appSettings["SubscriptionID"];
            var azureCredentials = GetAzureCredentials(clientId, clientSecret, tenantId);
            var azure = Azure.Authenticate(azureCredentials).WithSubscription(subscriptionId);
            CloudBlobClient cloudBlobClient = cloudStorageAccount.CreateCloudBlobClient();
            CloudBlobContainer container = cloudBlobClient.GetContainerReference(cloudBlobContainer.Name + "copy");
            var containerCreationOperation = await container.CreateIfNotExistsAsync();
            if (container.Exists())
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public static CloudStorageAccount GenerateCloudStorageAccount(string cloudStorageAccountName)
        {
            var appSettings = ConfigurationManager.AppSettings;
            string clientId = appSettings["ClientID"];
            string clientSecret = appSettings["ClientSecretKey"];
            string tenantId = appSettings["TenantID"];
            string subscriptionId = appSettings["SubscriptionID"];
            var azureCredentials = GetAzureCredentials(clientId, clientSecret, tenantId);
            var azure = Azure.Authenticate(azureCredentials).WithSubscription(subscriptionId);
            foreach(var storageAccount in azure.StorageAccounts.List())
            {
                if(storageAccount.Name == cloudStorageAccountName)
                {
                    var key = storageAccount.GetKeys().First();
                    string StorageConnectionString = GenerateConnectionString(storageAccount.Name, key);
                    CloudStorageAccount cloudStorageAccount = CloudStorageAccount.Parse(StorageConnectionString);
                    return cloudStorageAccount;
                }
            }
            return null; 
        }
        public static async void copyBlobDirectoryContents(CloudBlobDirectory cloudBlobDirectory, CloudBlobContainer cloudBlobContainer,CloudBlobContainer container)
        {
            //Create cloudblobdirectoryCopy
            //Copy the cloud blob contents
            //Create the Copy the Inner cloudBlobdirectory
            //Recurse the process
            var blobItemList = await ListBlobsInDirectoryAsync(cloudBlobDirectory);
            foreach (var blobItem in blobItemList)
            {
                if (blobItem.GetType().Name == "CloudBlobDirectory")
                {
                    string fileName = ((CloudBlobDirectory)blobItem).Prefix;
                    CloudBlobDirectory blobDirectory = container.GetDirectoryReference(fileName);
                    //We can't create folder independently because folder is a virtual entity in blob storage. You don't need to create a folder before using it.
                    copyBlobDirectoryContents(blobDirectory, cloudBlobContainer, container);
                }
                if (blobItem.GetType().Name == "CloudBlockBlob")
                {
                    string fileName = ((CloudBlob)blobItem).Name + "copy";
                    CloudBlockBlob blobCopy = container.GetBlockBlobReference(fileName);
                    if (!await blobCopy.ExistsAsync())
                    {
                        CloudBlockBlob blob = cloudBlobContainer.GetBlockBlobReference(((CloudBlob)blobItem).Name);
                        if (await blob.ExistsAsync())
                        {
                            await blobCopy.StartCopyAsync(blob);
                        }
                    }
                }
            }
        }
        private static string GetShareAccessUri(CloudBlob sourceBlob)
        {
            int validMins = 300;
            var policy = new SharedAccessBlobPolicy
            {
                Permissions = SharedAccessBlobPermissions.Read,
                SharedAccessStartTime = null,
                SharedAccessExpiryTime = DateTimeOffset.Now.AddMinutes(validMins)
            };

            var sas = sourceBlob.GetSharedAccessSignature(policy);
            return sourceBlob.Uri.AbsoluteUri + sas;
        }
        public static async void MoveContentsInContainer(CloudBlobContainer cloudBlobContainer)
        {
            try
            {
                CloudStorageAccount targetStorageAccount = GenerateCloudStorageAccount(TargetStorageAccountName);
                bool containerCreationOperation = await CreateCloudBlobContainer(cloudBlobContainer, targetStorageAccount);

                if (containerCreationOperation)
                {
                    CloudBlobClient cloudBlobClient = targetStorageAccount.CreateCloudBlobClient();
                    CloudBlobContainer container = cloudBlobClient.GetContainerReference(cloudBlobContainer.Name + "copy");
                    var blobItemList = await ListBlobsAsync(cloudBlobContainer);
                    foreach (var blobItem in blobItemList)
                    {
                        if (blobItem.GetType().Name == "CloudBlockBlob")
                        {
                            string fileName = ((CloudBlob)blobItem).Name + "copy";
                            CloudBlockBlob blobCopy = container.GetBlockBlobReference(fileName);
                            if (!await blobCopy.ExistsAsync())
                            {
                                CloudBlockBlob blob = cloudBlobContainer.GetBlockBlobReference(((CloudBlob)blobItem).Name);
                                if (await blob.ExistsAsync())
                                {
                                    var sharedAccessUri = GetShareAccessUri(blob);
                                    await blobCopy.StartCopyAsync(new Uri(sharedAccessUri));
                                    //await blobCopy.StartCopyAsync(blob.Uri);//Giving a exception The remote server returned an error: (404) Not Found.
                                }
                            }
                        }
                        if (blobItem.GetType().Name == "CloudBlobDirectory")
                        {
                            string fileName = ((CloudBlobDirectory)blobItem).Prefix;
                            CloudBlobDirectory blobDirectoryCopy = container.GetDirectoryReference(fileName);
                            copyBlobDirectoryContents(blobDirectoryCopy, cloudBlobContainer, container);
                        }
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
        public async Task randomfunasync()
        {
           try
           {
                var appSettings = ConfigurationManager.AppSettings;
                string clientId = appSettings["ClientID"];
                string clientSecret = appSettings["ClientSecretKey"];
                string tenantId = appSettings["TenantID"];
                string subscriptionId = appSettings["SubscriptionID"];

                var resourceGroupList = await GetResourceGroups(tenantId, clientId, clientSecret, subscriptionId);

                var azureCredentials = GetAzureCredentials(clientId, clientSecret, tenantId);
                var azure = Azure.Authenticate(azureCredentials).WithSubscription(subscriptionId);

                string functionAppName = CommonName + GetUniqueHash(clientId + tenantId + subscriptionId);

                List<string> resources = new List<string>();

                string targetResourceGroup = GetTargetResourceGroupId(subscriptionId, TargetResourceGroupName);

                var resourceManagementClient = GetResourceManagementClientClient(clientId, clientSecret, tenantId, subscriptionId);
                
                var webAppAndFunctionAppServices = azure.WebApps.List();

               var tee =await  azure.WebApps.GetByResourceGroupAsync("krishna","");
                /*
                var webApp = await azure.WebApps.GetByResourceGroupAsync(ResourceGroupName,appSettings["webSiteName"]);

                if (webApp.DefaultHostName.Contains(appSettings["webSiteName"]))
                {
                    Console.WriteLine(webApp.DefaultHostName);
                    Console.WriteLine("Retreived the website name.Hiiiii");
                }

                var functionApp = await azure.WebApps.GetByResourceGroupAsync(ResourceGroupName,functionAppName);
                
                if(functionApp.DefaultHostName.Contains(functionAppName))
                {
                    Console.WriteLine(functionApp.DefaultHostName);
                    Console.WriteLine("This is retreived");
                }
                */
                //await azure.WebApps.DeleteByResourceGroupAsync("krishna", "rajakeis");
                //await azure.StorageAccounts.DeleteByResourceGroupAsync("krishna", "rajakeis930a");
                //await azure.AppServices.FunctionApps.DeleteByResourceGroupAsync("ravite", "ravite");
                
                //await azure.AppServices.AppServicePlans.DeleteByResourceGroup("ravite",);
                /*
                var storageAcc = await azure.StorageAccounts.GetByResourceGroupAsync("krishna", "krishnateja");
                var keyt = storageAcc.GetKeys().First();
                string StorageConnectionString = GenerateConnectionString(storageAcc.Name, keyt);
                CloudStorageAccount cloudStorageAccount = CloudStorageAccount.Parse(StorageConnectionString);
                if (CloudStorageAccount.TryParse(StorageConnectionString, out cloudStorageAccount))
                {
                    Console.WriteLine("Connection is established");
                    CloudBlobClient cloudBlobClient = cloudStorageAccount.CreateCloudBlobClient();
                    var blobContainer = cloudBlobClient.GetContainerReference("afimsv-stageartifactscopy");
                    await blobContainer.DeleteIfExistsAsync();
                }
                */
                foreach (var service in webAppAndFunctionAppServices)
                {
                       
                    if (service.DefaultHostName.Contains(appSettings["webSiteName"]))
                    {
                        Console.WriteLine(service.DefaultHostName);
                        Console.WriteLine("Retreived the website");
                       /*
                        resources.Add(service.Id);                        

                        ResourcesMoveInfoInner resourcesMoveInfo = new ResourcesMoveInfoInner(resources,targetResourceGroup);
                        await resourceManagementClient.Resources.MoveResourcesAsync(service.ResourceGroupName, resourcesMoveInfo);
                        */
                    }
                    if (service.DefaultHostName.Contains(functionAppName))
                    {
                        Console.WriteLine(service.DefaultHostName);
                        Console.WriteLine("Retreived the functionApp");

                        /* 
                         resources.Add(service.Id);

                         ResourcesMoveInfoInner resourcesMoveInfo = new ResourcesMoveInfoInner(resources, targetResourceGroup);
                         await resourceManagementClient.Resources.MoveResourcesAsync(service.ResourceGroupName, resourcesMoveInfo);
                         */
                    }
                }
                /*var storageAccountsList = azure.StorageAccounts.List();
                foreach(var storageAccount in storageAccountsList)
                {
                    if (storageAccount.Name == appSettings["storageAccountName"])
                    {
                        var key = storageAccount.GetKeys().First();
                        string StorageConnectionString = GenerateConnectionString(storageAccount.Name, key);
                        CloudStorageAccount cloudStorageAccount = CloudStorageAccount.Parse(StorageConnectionString);
                        if (CloudStorageAccount.TryParse(StorageConnectionString, out cloudStorageAccount))
                        {
                            Console.WriteLine("Connection is established");
                            CloudBlobClient cloudBlobClient = cloudStorageAccount.CreateCloudBlobClient();
                            var cloudBlobContainerList = await ListContainersAsync(cloudBlobClient);
                            foreach(var cloudBlobContainer in cloudBlobContainerList)
                            {
                                if(cloudBlobContainer.Name.Contains("stageartifacts") && cloudBlobContainer.Name.Contains(storageAccount.ResourceGroupName))
                                {
                                    Console.WriteLine(cloudBlobContainer.Name);
                                    Console.WriteLine("Retreived the stageartifacts folder");
                                    MoveContentsInContainer(cloudBlobContainer);
                                }
                            }
                        }
                        else
                        {
                            Console.WriteLine("Connection is not established");
                        }
                    }
                }
                */
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

            DeploymentStatus deploymentStatus = new DeploymentStatus();
            deploymentStatus.randomfunasync().Wait();
            Console.WriteLine("Press a key to exit..");
            Console.ReadLine();
        }
    }
}
