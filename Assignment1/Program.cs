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

            fileName = "test.txt";

            /*

            //if not two arguments
            if (args.Length != 2)
            {
                Console.WriteLine("Usage: program name, filename");
            }
            fileName = args[2];
            */

            //classes
            lexicalScanner lx = new lexicalScanner(fileName);

            while (lexicalScanner.token != "eoft")
            {
                lx.getNextToken();

            }
            
            /*
            //While n..........loooooooooooooooooooooookot eof...
            while(token != "eoft")
            {
                token = lx.getNextToken();

                Console.WriteLine(token);
            
            }
            */
        }
    }
}
