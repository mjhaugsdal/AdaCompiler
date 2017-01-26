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
        char ch;

        public void openFile(string fileName)
        {
            
            try
            {
                using (StreamReader sr = new StreamReader(fileName))
                {
                    getNextChar(sr);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }//End openFile

    
        public void getNextChar(StreamReader sr)
        {
            ch = (char)sr.Read();
            Console.WriteLine(ch); //check for testing
        }

        public void getNextToken()
        {

        }
        public void processToken()
        {

        }


    }
}
