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
/// Assignment: 
/// Due Date: 04.04.2017
/// Instructor: Hamer
/// 
/// Description: Main Driver for compiler assignment 1
/// 
/// </summary>


namespace Assignment1
{
    class Program
    {
        unsafe public static void Main(string[] args)
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

                
                    //Create file
                    string path = args[0];
                    string[] tacPath = path.Split('.');
                    path = tacPath[0] + ".tac";




                    //File.Create(path);
                    if (File.Exists(path))
                        File.Delete(path);
                 /*   else
                        File.Create(path);*/


                  /*  else
                        File.Create(aPath);*/


                    SymTab st = new SymTab();
                
                    fileName = args[0]; // args


                    StreamWriter sw = new StreamWriter(path, true); //Write to .tac
                    StreamReader sr = new StreamReader(fileName);   //read from .ada



                    //classes
                    //for return
                    lexicalScanner.Token token = new lexicalScanner.Token();

                    lexicalScanner lx = new lexicalScanner(fileName, sr);
                    rdp rdp = new rdp(token, lx, sr, st, sw);

                   
                    lx.createDictionary();
      
                    //While NOT eoft
                    while (token.token != lexicalScanner.SYMBOL.eoft )
                    {
                    

                        token = lx.getNextToken();
                    
                        token = rdp.parse(token);
                        
                        // st.writeTable(1);

                        if (rdp.error != true)
                        {
                            Console.WriteLine("Program is Valid!");
                            rdp.emit("start proc " + rdp.mainProc);
                            Console.WriteLine();
                            sw.Close(); // close writing .tac


                            // StreamWriter asmSw = new StreamWriter(aPath, true); //Write to .asm
                            StreamReader tacSr = new StreamReader(path);   //read from .tac

                            Assembler asm = new Assembler(path, rdp, tacSr, st);

                            asm.buildDataSeg();
                            asm.addCodeAndIncludes();

                            asm.genAssembly();
                        }


                        /* if(j > 20)
                         {
                             Console.WriteLine("Press any key to continue...");
                             Console.ReadKey();
                             j = 0;    
                         }
                         lx.printToken(token);

                         j++;*/
                    }// end while NOT eoft
                   

                   // Console.WriteLine("Tokens processed: " + lexicalScanner.i); 
                }
                catch(FileNotFoundException)
                {
                    Console.WriteLine("Error!!! File not found");


                }

            }
        }   
    }
}
