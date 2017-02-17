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
    
            //if not two arguments
            if (args.Length != 1)
            {
                Console.WriteLine("Usage: \"program name\".exe, \"filename\"");
            }

            else
            {
                try
                {
                    fileName = args[0]; // args
                    StreamReader sr = new StreamReader(fileName);
                    //classes
                    //for return
                    lexicalScanner.Token token = new lexicalScanner.Token();

                    lexicalScanner lx = new lexicalScanner(fileName, sr);
                    rdp rdp = new rdp(token, lx, sr);

                    lx.createDictionary();

                    int j = 0;
                    token = lx.getNextToken();

                    token = rdp.parse(token);


                    if (token.token != lexicalScanner.SYMBOL.eoft)
                    {
                        Console.WriteLine("ERROR: EOF Expected, found: " + token.token);

                    }
                    else if (rdp.error != true)
                        Console.WriteLine("Program is Valid!");
                }
                catch(FileNotFoundException e)
                {
                    Console.WriteLine("Error: File not found");

                }

            }
        }   
    }
}
