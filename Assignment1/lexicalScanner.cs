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
        //public static string token;
        string lexeme;
        private string fileName;
        private StreamReader sr;

        public class Token
        {
            public string token;
            public string lexeme;
            public int value;
            public float valueR;
            public string literal;

        }



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

            return ch;
        }
        public char peekNextChar()
        {
            ch = (char)sr.Peek();

            return ch;
        }

        public Token getNextToken()
        {
            Token token = new Token();

            while (!sr.EndOfStream)
            {
                ch = getNextChar();
                if (Char.IsWhiteSpace(ch) != true)
                {
                    processToken(ch, token);
                }
                else
                {
                    return token;
                }
            }//end while eof

            token.token = "eoft";
            return token;
        }

        public void processToken(char ch, Token token)
        {
            lexeme = ch.ToString();
            //Console.Write(lexeme);
            ch = peekNextChar();
            //peek?

            if (Char.IsLetter(lexeme[0])) //IF LETTER
            {
                processWordToken(lexeme, token);

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
            

           // return lexeme;

            
        }

        public void processWordToken(string lexeme, Token token)
        {
        
            while(Char.IsWhiteSpace(ch) != true) // read the rest of the lexeme
            {
                if (sr.Peek() > -1)
                {
                    ch = getNextChar();
                    lexeme = lexeme + ch;
                }
                else
                    break;  
            }
            Console.WriteLine(lexeme); //GOT IT!
            if(reswords.ContainsKey(lexeme))
            {
                reswords.TryGetValue(lexeme, out token.lexeme);

            }
            else
            {
                token.lexeme = "idt";
            }


            //If lexeme is a reserved word

        }
        public void processNumToken (Token token)
        {
            bool hasFraction = false;
            // read rest of line
            // look for . 

            while (Char.IsWhiteSpace(ch) != true) // read the rest of the lexeme
            {


                if (sr.Peek() > -1)
                {
                    ch = getNextChar();
                    if (hasFraction == false)
                    {
                        token.value = token.value + ch;
                    }
                    else
                    {
                        token.valueR = token.valueR + ch;

                    }
                }
                else
                    break;
            }

            // if '.' store as float, if not store as int
            //check for min max for int and float



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
