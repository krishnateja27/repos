using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Test
{
    class Program
    {
        static void Main(string[] args)
        {
            string date = "12/10/2017 12:52:13+0000";
            var toDateTimeOffset = DateTimeOffset.ParseExact(date.Split('+')[0], "dd/MM/yyyy HH:mm:ss",
                    System.Globalization.CultureInfo.InvariantCulture);
            Console.WriteLine(toDateTimeOffset.ToLocalTime().ToString());
            Console.WriteLine($"This is a new operator");
            Console.WriteLine("Press a key to continue");
            Console.ReadLine();
        }
    }
}
