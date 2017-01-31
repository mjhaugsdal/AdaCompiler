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

            fileName = "test6.adb";

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
                 Console.WriteLine("Token\t\tlexeme\t\tLiteral");

            

                while (token.token != "eoft")
                {

                    token = lx.getNextToken();
                    
                    
                    if(!String.IsNullOrEmpty(token.token))
                    {
                    //Console.WriteLine(token.token + "\t\t" + token.lexeme + "\t\t\t" + token.value + "\t\t" + token.literal);
                    Console.WriteLine(token.token + "\t\t" + token.lexeme + "\t\t" + token.literal);
                    }
                    //Console.WriteLine(token.token +"\t\t"+ token.lexeme + "\t\t\t" +token.value + "\t\t"+token.literal);
                    
                }
            //}
        }   
    }
}
