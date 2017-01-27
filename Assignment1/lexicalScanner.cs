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

        public StreamReader openFile()
        {
            //So i dont need to enter filename every time xD
            fileName = "test.txt";


            StreamReader sr = new StreamReader(fileName);
            return sr;

        }//End openFile

    
        public char getNextChar(StreamReader sr)
        {

            ch = (char)sr.Read();
            Console.WriteLine(ch); //check for testing
            sr.Peek(); // peek at next char
            //if... something about the next char
            return ch;
        }

        public string getNextToken()
        {

            StreamReader sr = openFile();

            //while !EOF
            while (!sr.EndOfStream)
            {
                ch = getNextChar(sr);
                if (Char.IsWhiteSpace(ch) != false)
                {
                    token += ch;
                }
                else
                    return token;
            }

            return "Debug token?";

        }
        public void processToken()
        {

        }


    }
}
