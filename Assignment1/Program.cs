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
            //string token;

            fileName = "test.adb";

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
                 Console.WriteLine("Token\t\tlexeme\t\tAttribute");

            

                while (token.token != "eoft")
                {

                    token = lx.getNextToken();
                    
                    
                    if(!String.IsNullOrEmpty(token.token))
                    {
                        if(token.token == "numt")
                        {
                            if (lexicalScanner.hasDot == false)
                            {
                                Console.WriteLine(token.token + "\t\t" + token.lexeme + "\t\t" + token.value);
                            }
                            else
                                Console.WriteLine(token.token + "\t\t" + token.lexeme + "\t\t" + token.valueR);
                            }
                        else if(token.token == "literalt")
                        {
                            Console.WriteLine(token.token + "\t\t" + token.lexeme + "\t\t" + token.literal);

                        }
                        else
                            Console.WriteLine(token.token + "\t\t" + token.lexeme );

                   }

            }
            //}
        }   
    }
}
