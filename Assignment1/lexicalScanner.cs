using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assignment1
{
    public class lexicalScanner
    {
        //variables
        char ch;
        public static string token;
        private string fileName;

        public lexicalScanner(string fileName)
        {
            this.fileName = fileName;
        }

        public void openFile()
        {
            //So i dont need to enter filename every time xD
            fileName = "test.txt";
            
            try
            {
                
                using (StreamReader sr = new StreamReader(fileName))
                {
                    getNextChar(sr);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Error with reading file");
                Console.WriteLine(e.Message);
            }
        }//End openFile

    
        public void getNextChar(StreamReader sr)
        {
            ch = (char)sr.Read();
            Console.WriteLine(ch); //check for testing
            sr.Peek();


        }

        public void getNextToken()
        {

     
        }
        public void processToken()
        {

        }


    }
}
