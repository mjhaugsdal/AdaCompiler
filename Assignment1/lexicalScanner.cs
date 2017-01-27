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
        public char peekNextChar()
        {
            ch = (char)sr.Peek();

            return ch;
        }

        public string getNextToken()
        {

            while (!sr.EndOfStream)
            {
                ch = getNextChar();
                if (Char.IsWhiteSpace(ch) != true)
                {
                    
                    processToken(ch);
                }
                else
                {
                    return token;

                }
            }

            return token;
        }

        public string processToken(char ch)
        {
            lexeme = ch.ToString();
            Console.Write(lexeme);
            ch = peekNextChar();
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
            else if(lexeme[0] == 58 || lexeme[0] == 60 || lexeme[0] == 62 )
            {

            }
            

            return lexeme;

            
        }

        public void processWordToken()
        {
        
            while(Char.IsWhiteSpace(ch) != true)
            {

            }
        }


    }
}
