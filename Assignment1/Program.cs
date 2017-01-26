using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assignment1
{
    class Program
    {
        public static void Main(string[] args)
        {
            //variables
            string fileName;
            string token;
            //classes
            lexicalScanner lx = new lexicalScanner();

            //if not two arguments
            if (args.Length != 2)
            {
                Console.WriteLine("Usage: " + args[0] + " filename");
            }
            fileName = args[2];

            lx.openFile(fileName);

            /*
            //While not eof...
            while(token != "eoft")
            {
                token = lx.getNextToken();

                Console.WriteLine(token);
            
            }


            */
        }
    }
}
