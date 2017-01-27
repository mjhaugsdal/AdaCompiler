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
        string lexeme;
        private string fileName;
        private StreamReader sr;

        public lexicalScanner(string fileName)
        {
            this.fileName = fileName;
        }

        public lexicalScanner(string fileName, StreamReader sr) : this(fileName)
        {
            this.sr = sr;
        }


        public char getNextChar()
        {

            ch = (char)sr.Read();
            
            //sr.Peek(); // peek at next char
            //if... something about the next char
            return ch;
        }

        public string getNextToken()
        {

            //StreamReader sr = openFile();

            while(Char.IsWhiteSpace(ch) == true)
            {
                ch = getNextChar();
                if(!sr.EndOfStream)
                {
                    token = processToken(ch);
                }
                else
                {
                    token = "eoft";
                }

            }


            return token;

        }

        public string processToken(char ch)
        {
            lexeme = ch.ToString();
            Console.WriteLine(lexeme);
            ch = getNextChar();
            //peek?

            if (Char.IsLetter(lexeme[0])) //IF LETTER
            {
                //processWordToken

            }
            else if (Char.IsDigit(lexeme[0])) //IF NUMBER
            {
                //processNumToken
            }
            else if(lexeme[0] == 123) //IF LEFT CURLY BRACE (COMMENT)
            {
                //processComment
            }
            else if(lexeme[0] == 58 || lexeme[0] == 60 || lexeme[0] == 62 || lexeme[0] == )
            {

            }
            

            return lexeme;


        }

    }
}
