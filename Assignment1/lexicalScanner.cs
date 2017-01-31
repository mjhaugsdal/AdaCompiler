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
        public static bool hasDot = false;
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

            ch =(char)sr.Read();
            return ch;
        }
        public char peekNextChar()
        {
            return (char)sr.Peek();
        }

        public Token getNextToken()
        {

            Token token = new Token();

            while (!sr.EndOfStream)
            {

                ch = peekNextChar();
                if (!Char.IsWhiteSpace(ch) && String.IsNullOrWhiteSpace(token.token))
                {
                    ch = getNextChar();
                    processToken(token);

                }
                if (ch == 10 || Char.IsWhiteSpace(ch)) 
                {
                    ch = getNextChar();
                }
                else
                {
                    return token;
                }
            }//end while eof

            token.token = "eoft";
            return token;
        }

        public void processToken(Token token)
        {
            lexeme = ch.ToString();

            ch = peekNextChar();
            //peek?

            if (Char.IsLetter(lexeme[0])) //IF LETTER
            {

                processWordToken( token);

            }
            else if (Char.IsDigit(lexeme[0])) //IF NUMBER
            {
                processNumToken(token);
            }
            else if (lexeme[0] == 45) //IF DOUBLE MINUS (COMMENT)
            {
                if (ch == 45)//processComment
                {
                    sr.ReadLine();

                }
                else
                {
                    token.token = "unkownt";
                }
            }
            
            //IF single and or double
            else if (lexeme[0] == 60 || lexeme[0] == 62 || lexeme[0] == 61 || lexeme[0] == 47 || lexeme[0] == 58 )
            {
                //check next token for =
                if (ch == 61)
                {
                    //process double token
                    processDoubleToken(token);

                }
                else 
                {
                    //Process Single token
                    processSingleToken(token);
                }
            }


            // return lexeme;


        }



        private void processDoubleToken(Token token)
        {

            lexeme = lexeme + ch.ToString();
            token.lexeme = lexeme;
            getNextChar();

            if (lexeme[0] == 47 || lexeme[0] == 60 || lexeme[0] == 62)
            {
                token.token = "relopt";
            }
            else if(lexeme[0] == 58)
            {
                token.token = "assignopt";
            }

        }

        private void processSingleToken(Token token)
        {
            
            //if =, >, <
       
            token.lexeme = lexeme;
            if (lexeme[0] == 61 || lexeme[0] == 60 || lexeme[0] == 62)
            {
                token.token = "relopt";
                
            }
            else if (lexeme[0] == 43 || lexeme[0] == 45)
            {
                token.token = "addopt";
                
            }
            else if(lexeme[0] == 42 || lexeme[0] == 47)
            {
                token.token = "multopt";
                
            }
            
        }

        public void processWordToken( Token token)
        {
            //Console.WriteLine();
            

            while(sr.Peek() > -1) // read the rest of the lexeme
            {
                char c = peekNextChar();
                //idt can be letters, underscore and/or digits
                if (!Char.IsLetterOrDigit(c) && c != 95)
                    break;

                if (sr.Peek() == 32 || sr.Peek() == 10)
                {
                    break;
                }
                else
                {
                    ch = getNextChar();
                    lexeme = lexeme + ch;
                }

            }//end while

            token.lexeme = lexeme;
//            Console.WriteLine("Lexeme: "+lexeme); //GOT IT!
            
            
            if(reswords.ContainsKey(lexeme))
            {
                //Console.WriteLine("Reserved" );
                //If lexeme is reserved word, used reserved token tag
                reswords.TryGetValue(lexeme, out token.token);
                //Console.WriteLine(token.token);
            }
            else
            {

                token.token = "idt";
            }
            //If lexeme is a reserved word
            if (token.token == "putt")
            {
                processStringLiteral(token);
            }
            if(token.lexeme.Length > 17)
            {
                token.token = "unkownt";
            }


        }

        private void processStringLiteral(Token token)
        {
            string literal ="";

            while(sr.Peek() != 34)
            {
                ch = getNextChar();
            }
            //Look for opening literal
            //found!
            //literal = ch.ToString();

            //ch = getNextChar(); // set ch to '"'
            getNextChar();
            while(sr.Peek() != 34)
            {
                literal = literal + ch;
                getNextChar();
                
            }
            literal = literal + ch;
            ch = getNextChar();
            literal = literal + ch;
            token.token = "literalt";
            token.literal = literal;
        }

        public void processNumToken (Token token)
        {
            
            string num = "";
            // read rest of line
            // look for . 
            char c = peekNextChar();
            if (Char.IsDigit(c))
            {
                while (sr.Peek() > -1 && Char.IsDigit(c)) // read the rest of the lexeme
                {
                    ch = getNextChar();
                    num = num + ch;
                }
            }
            else
            {
                int numb = Int32.Parse(lexeme);
                token.value = numb;
                //Console.WriteLine(numb);

            }

            token.token = "numt";

           // Console.WriteLine(num);

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
            reswords.Add("or", "ort");
            reswords.Add("rem", "remt");
            reswords.Add("mod", "modt");
            reswords.Add("and", "andt");


        }

     

    }
}
