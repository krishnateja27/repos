using System;
using System.Configuration;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System.IO;
//using Microsoft.WindowsAzure.Storage.File;
//using Microsoft.WindowsAzure;

class PrintNamesInContainer
{
    public void PrintDirectoryContents(IEnumerable<IListBlobItem> listBlobs, CloudBlobContainer x,int hidelength)
    {
        //hidelength = 0;
        foreach (IListBlobItem item in listBlobs)
        {
            if (item.GetType().Name == "CloudBlockBlob")
            {
                var length = ((Microsoft.WindowsAzure.Storage.Blob.CloudBlob)item).Name.Length;
                Console.WriteLine(((Microsoft.WindowsAzure.Storage.Blob.CloudBlob)item).Name.Substring(hidelength,length-hidelength));
                return;
            }
            else
            {
                var Subdirectorydisplaylength = (((Microsoft.WindowsAzure.Storage.Blob.CloudBlobDirectory)item).Prefix).Length;
                Console.WriteLine(((Microsoft.WindowsAzure.Storage.Blob.CloudBlobDirectory)item).Prefix.Substring(hidelength, Subdirectorydisplaylength - hidelength - 1));
                String directoryName = ((Microsoft.WindowsAzure.Storage.Blob.CloudBlobDirectory)item).Prefix.Substring(hidelength,Subdirectorydisplaylength - hidelength);
                CloudBlobDirectory directory = x.GetDirectoryReference(((Microsoft.WindowsAzure.Storage.Blob.CloudBlobDirectory)item).Prefix);
                //directory.Prefix;
                PrintDirectoryContents(directory.ListBlobs(),x,Subdirectorydisplaylength);
            }
            //Console.WriteLine(item.GetType().Name); 
            //Console.WriteLine(item.ToString());
            //Console.WriteLine(((Microsoft.WindowsAzure.Storage.Blob.CloudBlobDirectory)item).Prefix);
            //Console.WriteLine(item.Uri);
        }
        //} while (continuationToken != null); // Loop while the continuation token is not null.
    }



    public void PrintBlobContents(IEnumerable<IListBlobItem> listBlobs, CloudBlobContainer x)
    {
        //Console.WriteLine(listBlobs.Count());
        //BlobContinuationToken continuationToken = null;
        //do
        //{
        // Get the value of the continuation token returned by the listing call.
 
        foreach (IListBlobItem item in listBlobs)
        {
            if (item.GetType().Name == "CloudBlockBlob")
            {
                Console.WriteLine(((Microsoft.WindowsAzure.Storage.Blob.CloudBlob)item).Name);
            }
            //Console.WriteLine(item.Uri);
            //if(item.GetType().Name == "CloudBlobDirectory")
            else
            {
                var displaylength = (((Microsoft.WindowsAzure.Storage.Blob.CloudBlobDirectory)item).Prefix).Length;
                Console.WriteLine(((Microsoft.WindowsAzure.Storage.Blob.CloudBlobDirectory)item).Prefix.Substring(0, displaylength - 1));

                String directoryName = ((Microsoft.WindowsAzure.Storage.Blob.CloudBlobDirectory)item).Prefix.Substring(0, displaylength - 1);
                CloudBlobDirectory directory = x.GetDirectoryReference(directoryName);
                //directory.Prefix;
                PrintDirectoryContents(directory.ListBlobs(),x,displaylength);
             }
            //Console.WriteLine(item.GetType().Name); 
            //Console.WriteLine(item.ToString());
            //Console.WriteLine(((Microsoft.WindowsAzure.Storage.Blob.CloudBlobDirectory)item).Prefix);
            //Console.WriteLine(item.Uri);
        }
        //} while (continuationToken != null); // Loop while the continuation token is not null.
    }
}
namespace ConsoleApp7
{
    class Program
    {
        static void Main(string[] args)
        {
            XmlDocument document = new XmlDocument();
            document.Load(@"C:\Users\Administrator\source\repos\ConsoleApp5\ConsoleApp5\bin\Debug\Test.xml");

                var appSettings = System.Configuration.ConfigurationManager.AppSettings;
                if (appSettings.Count == 0)
                {
                    Console.WriteLine("AppSettings is empty");
                }
                else
                {
                foreach (var key in appSettings.AllKeys)
                {
                    CloudStorageAccount storageAccount = CloudStorageAccount.Parse(ConfigurationManager.AppSettings[key]);
                    // Check whether the connection string can be parsed.
                    if (CloudStorageAccount.TryParse(ConfigurationManager.AppSettings[key], out storageAccount))
                    {
                        Console.WriteLine("Connection is established");
                        // If the connection string is valid, proceed with operations against Blob storage here.
                        //...

                        CloudBlobClient cloudBlobClient = storageAccount.CreateCloudBlobClient();

                        // List the blobs in the container.
                        Console.WriteLine("List blobs in container.");
                       // Console.WriteLine(cloudBlobClient.ListContainers().Count());
                        foreach (var x in cloudBlobClient.ListContainers())
                        {
                            Console.WriteLine(x.Name);
                            IEnumerable<IListBlobItem> listBlobs  = x.ListBlobs();
                            //Console.WriteLine(x.ListBlobs().Count());
                            PrintNamesInContainer namesInContainer = new PrintNamesInContainer();
                            namesInContainer.PrintBlobContents(listBlobs,x);
                        }
                    }
                    else
                    {
                        // Otherwise, let the user know that they need to define the environment variable.
                        Console.WriteLine(
                            "A connection string has not been defined in the system environment variables. " +
                            "Add a environment variable named 'storageconnectionstring' with your storage " +
                            "connection string as a value.");
                        Console.WriteLine("Press any key to exit the sample application.");
                        Console.ReadLine();
                    }
                    Console.WriteLine("Press any key to continue");
                    Console.ReadLine();
                }
                }
                
        }
             
    }
}
