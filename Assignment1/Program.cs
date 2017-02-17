using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
//using System.Threading.Tasks;
using System.Globalization;

/// <summary>
/// Name: Markus Johan Haugsdal
/// Class: CSC 446 Compiler Construction
/// Assignment: 2
/// Due Date: 13.02.2017
/// Instructor: Hamer
/// 
/// Description: Main Driver for compiler assignment 2
/// 
/// </summary>


namespace Assignment1
{
    class Program
    {
        public static void Main(string[] args)
        {
            //Because my computer is european, and we use , and not . for numbers!
            CultureInfo.DefaultThreadCurrentCulture = new CultureInfo("us-US");
            CultureInfo.DefaultThreadCurrentUICulture = new CultureInfo("us-US");

            //variables
            string fileName;

            //for debugging
           // fileName = "test2.adb";

           
            //if not two arguments
            if (args.Length != 1)
            {
                Console.WriteLine("Usage: \"program name\".exe, \"filename\"");
            }

            else
            {
                fileName = args[0]; // args
                //fileName = "parse8.ada";
                StreamReader sr = new StreamReader(fileName);
                //classes
                //for return
                lexicalScanner.Token token = new lexicalScanner.Token();

                lexicalScanner lx = new lexicalScanner(fileName, sr);
                rdp rdp = new rdp(token, lx,sr);

                lx.createDictionary();
                /*string output = String.Format("{0,-15}  {1,-15} ", "Token"
                     , "Lexeme");
              
                Console.WriteLine(output);
                */
                int j = 0;
                token = lx.getNextToken();
                //While NOT eoft
               // while (token.token != lexicalScanner.SYMBOL.eoft )
             //   {
                    

                    
                    
                    token = rdp.parse(token);

                    //Console.WriteLine("Reached eof");
                    if(rdp.error != true)
                        Console.WriteLine("Program is Valid!");
                    
                if(token.token != lexicalScanner.SYMBOL.eoft)
                {
                    Console.WriteLine("ERROR: EOF Expected, found: " + token.token);

                }

                   /* if(j > 20)
                    {
                        Console.WriteLine("Press any key to continue...");
                        Console.ReadKey();
                        j = 0;    
                    }
                    lx.printToken(token);
                   
                    j++;*/
               // }// end while NOT eoft
               // Console.WriteLine("Tokens processed: " + lexicalScanner.i); 
            }
        }   
    }
}
