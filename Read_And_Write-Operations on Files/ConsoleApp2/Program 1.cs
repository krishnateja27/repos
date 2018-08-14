using System;

namespace ConsoleApp2
{
    class Read_operation_on_File
    {
        static void Main(string[] args)
        {
            //Reading data from file at a stretch

            /*
            string text_address = @"C:\Users\Administrator\Downloads\textdocument.txt";
            string path = System.IO.File.ReadAllText(text_address);
            Console.WriteLine(path);
            */
            //Read data line by line and then print the data
            string []lines = System.IO.File.ReadAllLines(@"C:\Users\Administrator\Downloads\textdocument.txt");/* storing the data line by line in a array named lines*/
            Console.WriteLine("The contents of the text file are:");
            foreach (string line in lines)//analogous to for loop and varags
            {
                Console.WriteLine("\t" + line);
            }
            string texts = System.IO.File.WriteAllText(@"C:\Users\Administrator\Downloads\textsdocument.txt");
            Console.WriteLine("Press any key to exit");
            Console.ReadKey();
        }
    }
}
