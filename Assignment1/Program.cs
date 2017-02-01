using System;
using System.Threading;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Globalization;



namespace Assignment1
{
    class Program
    {
        public static void Main(string[] args)
        {
            CultureInfo.DefaultThreadCurrentCulture = new CultureInfo("us-US");
            CultureInfo.DefaultThreadCurrentUICulture = new CultureInfo("us-US");

            //variables
            string fileName;
            //string token;

            fileName = "test2.adb";

            StreamReader sr = new StreamReader(fileName);

            //if not two arguments
            if (args.Length != 2)
            {
                Console.WriteLine("Usage: program name, filename");
            }

           // else
           // {

          
                 //classes
                 lexicalScanner lx = new lexicalScanner(fileName, sr);

                 lexicalScanner.Token token = new lexicalScanner.Token();

                 lx.createDictionary();
                 string output = String.Format("{0,-15}  {1,-15} {2,-15}", "Token", "Lexeme", "Attribute");
                 //Console.WriteLine("Token\t\tlexeme\t\tAttribute");
                 Console.WriteLine(output);


            int i = 0;

                while (token.token != "eoft")
                {
                    

                    token = lx.getNextToken();

                lx.printToken(token);
                i++;


            }
            Console.WriteLine("Tokens: " + i);
            //}
        }   
    }
}
