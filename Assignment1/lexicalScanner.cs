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

        Dictionary<string, string> reswords = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

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
            //Console.WriteLine("Wooptidoo!");
            token = "eoft";
            return token;
        }

        public string processToken(char ch)
        {
            lexeme = ch.ToString();
            //Console.Write(lexeme);
            ch = peekNextChar();
            //peek?

            if (Char.IsLetter(lexeme[0])) //IF LETTER
            {
                processWordToken(lexeme);

            }
            else if (Char.IsDigit(lexeme[0])) //IF NUMBER
            {
                //processNumToken
            }
            else if(lexeme[0] == 123) //IF LEFT CURLY BRACE (COMMENT)
            {
                //processComment
            }
            //IF ':', '<', '>', 
            else if(lexeme[0] == 58 || lexeme[0] == 60 || lexeme[0] == 62 )
            {
                //check next token for =
                if(ch == 61)
                {

                }
            }
            

            return lexeme;

            
        }

        public void processWordToken(string lexeme)
        {
        
            while(Char.IsWhiteSpace(ch) != true) // read the rest of the lexeme
            {
                ch = getNextChar();
                lexeme = lexeme + ch;
            }
            //Console.WriteLine(lexeme); //GOT IT!
            if(reswords.ContainsKey(lexeme))
            {

            }

            //If lexeme is a reserved word




        }

        public void createDictionary()
        {
            reswords.Add("begin", "begint");
            reswords.Add("module", "modult");
            reswords.Add("constant", "constt");
            reswords.Add("procedure", "proct");
            reswords.Add("is", "ist");
            reswords.Add("if", "ift");
            reswords.Add("then", "thent");
            reswords.Add("else", "elset");
            reswords.Add("elseif", "elseift");
            reswords.Add("while", "whilet");
            reswords.Add("loop", "loopt");
            reswords.Add("float", "floatt");
            reswords.Add("integer", "integert");
            reswords.Add("char", "chart");
            reswords.Add("get", "gett");
            reswords.Add("put", "putt");
            reswords.Add("end", "endt");

        }
    }
}
