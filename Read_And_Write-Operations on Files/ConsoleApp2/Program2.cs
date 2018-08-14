using System;
/*
namespace ConsoleApp2
{
    class Write_Operation_on_File
    {
        static void Main(string[] args)
        {
            string path = @"C:\Users\Administrator\Downloads\textsdocument.txt";
            string random_text = "This is Zen3 Info solutions";
            System.IO.File.WriteAllText(path,random_text);
           // Console.WriteLine(texts);
            Console.WriteLine("Press any key to exit");
            Console.ReadKey();
        }
    }
}
*/

/*
 sample_path = @"C:\Downloads\write_file";
            if(!System.IO.File.Exists())
            {
                string sample_text = "This is zen3" + Environment.NewLine;
                System.IO.File.WriteAllText(sample_path,sample_text);
            }
            string append_text = "This is an extra text" + Environment.NewLine;
            System.IO.File.AppendAllText(sample_path,append_text);
            string readText = File.ReadAllText(sample_path);
            Console.WriteLine(readText);
*/
namespace ConsoleApp2
{
    class Write_Operation_using_String_Reader
    {
        static void Main(string[] args)
        {
            try
            {
                //Business logic
                System.IO.StreamReader sr = new System.IO.StreamReader(@"C:\Users\Administrator\Downloads\textdocument.txt");
                //stream reader constructor for passing the file path and file name
                string line = sr.ReadLine();//reading first line of the text
                while (line != null)
                {
                    Console.WriteLine(line);
                    line = sr.ReadLine();// it is like reading line and updating the character pointer 
                                         // so after updating a line then we can see that it is actually pointing to
                                         // the next line
                }
            }
            catch(Exception e)
            {
                //catch block catches the Exception
                Console.WriteLine("Exception is:" + e.Message);
            }
            finally
            {
                //close all the resource blocks
                //close the file
                sr.Close();
                Console.ReadLine();
            }
        }
    }
}
