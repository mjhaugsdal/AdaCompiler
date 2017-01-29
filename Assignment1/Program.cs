using System;
using System.IO;
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

            fileName = "test.txt";

            StreamReader sr = new StreamReader(fileName);

            //if not two arguments
            if (args.Length != 2)
            {
                Console.WriteLine("Usage: program name, filename");
            }

            //classes
            lexicalScanner lx = new lexicalScanner(fileName, sr);
            lx.createDictionary();
            while (lexicalScanner.token != "eoft")
            {
                //Console.WriteLine("s");
                lx.getNextToken();

               

            }

        }
    }
}
