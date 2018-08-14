using System;
using System.Configuration;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.File;
using Microsoft.WindowsAzure;
class share
{
    public void printNamesInShares(CloudFileShare share)
    {
        if(share.Exists())
        {

            //if we have share then create ref
            var x= share.GetRootDirectoryReference();
            CloudFileDirectory sampleDir = x.GetDirectoryReference("*");
            Console.WriteLine(sampleDir.Name);
        }
    }
}
namespace ConsoleApp6
{
    class Program:share
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
                    CloudFileClient fileClient = storageAccount.CreateCloudFileClient();
                    foreach(var fileshare in fileClient.ListShares())
                    {
                        Console.WriteLine(fileshare.Name);
                        if(fileshare.Exists())
                        {
                            share shareobject = new share();
                            shareobject.printNamesInShares(fileshare);
                        }
                    }
                }
            }
            Console.WriteLine("Press any key to continue ");
            Console.ReadLine();
        }
    }
}
