using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Microsoft.WindowsAzure.Storage;
using System.Configuration;

class FetchData
{
    public string keyName { get; set; }  //Property of the fields or variables
    public string StorageAccountName { get; set; }
    public string ResourceGroup { get; set; }
    public string Directory { get; set; }
    public string SizeofFile { get; set; }
}
namespace ConsoleApp8
{
    class Program
    {
        static void Main(string[] args)
        {
            FetchData fetchDataObj = new FetchData
            { keyName = "a",
            StorageAccountName = "lko%j12kr",
            ResourceGroup = "abc123",
            Directory = "@C:downloads\test.txt",
            SizeofFile = "25kB",
            };
            string json = JsonConvert.SerializeObject(fetchDataObj,Formatting.Indented);//Formating is indented so that we don't get all
                                                                                        //key value pairs in  same line
            Console.WriteLine(json);
          
            //Retreiving a JSON object of type FetchData
            FetchData fetchData = JsonConvert.DeserializeObject<FetchData>(json);
            Console.WriteLine(fetchData.StorageAccountName);

            Console.WriteLine("Press any key to continue...");
            Console.ReadLine();
        }
    }
}
