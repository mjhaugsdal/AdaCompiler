using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Globalization;


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
                    processSingleToken(token);
                }
            }
            
            //IF single and or double
            else if (lexeme[0] == 60 || lexeme[0] == 62 || lexeme[0] == 61 || lexeme[0] == 47 || lexeme[0] == 58 ||
                     lexeme[0] == 40 || lexeme[0] == 41 || lexeme[0] == 44 || lexeme[0] == 59 || lexeme[0] == 34 ||
                     lexeme[0] == 46   )
                
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

            else
            {
                token.token = "unkownt";
                token.lexeme = lexeme;
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
            else if(lexeme[0] == 40)
            {
                token.token = "lparent";
            }
            else if (lexeme[0] == 41)
            {
                token.token = "rparent";
            }
            else if (lexeme[0] == 44)
            {
                token.token = "commat";
            }
            else if (lexeme[0] == 58)
            {
                token.token = "colont";
            }
            else if (lexeme[0] == 59)
            {
                token.token = "semicolont";
            }
            else if (lexeme[0] == 46)
            {
                token.token = "periodt";
            }
            else if (lexeme[0] == 34)
            {
                processStringLiteral(token);
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

                else if (sr.Peek() == 32 || sr.Peek() == 10)
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
          
            if(token.lexeme.Length > 17)
            {
                token.token = "unkownt";
            }


        }

        private void processStringLiteral(Token token)
        {
            string literal = "\"";

            /*while(sr.Peek() != 34)
            {
                ch = getNextChar();
            }*/
            //Look for opening literal
            //found!
            //literal = ch.ToString();

            //ch = getNextChar(); // set ch to '"'
            getNextChar();


            while (sr.Peek() != 34 && sr.Peek() != 10)
            {

                literal = literal + ch;
                getNextChar();
                

            }

            if (sr.Peek() != 10)
            {
                literal = literal + ch;
                ch = getNextChar();
                literal = literal + ch;
                token.token = "literalt";
                token.literal = literal;
                token.lexeme = literal;
            }
            else
            {
                
                token.token = "unkownt";
              
                token.literal = literal;
                token.lexeme = literal;
            }

        }


        public void processNumToken(Token token)
        {
           // getNextChar();
            hasDot = false;
            string nums = lexeme[0].ToString();
            token.token = "numt";
            char p;
            if (char.IsWhiteSpace(ch)) // only one char
            {
                Int32.TryParse(nums, out token.value);
            }
            else
            { 

                //char p = peekNextChar(); // peek at next position
                while (sr.Peek() > -1)
                {
                    p = peekNextChar();
                    if (char.IsDigit(p) || p == 46)
                    {
                        getNextChar();
                        

                        nums = nums + ch;
                        if (char.IsWhiteSpace(p))
                        {
                            break;
                        }
                        if (ch == 46 || p == 46)
                        {
                            hasDot = true;
                        }
                        
                    }
                    else
                    {
                        //Check for 
                        //token.token = "unkownt";
                        break;
                    }


                }//end while
         


                if (hasDot == true)
                {
                    if (ch == 46) // if last char of number is .
                    {
                        token.token = "unkownt";
                        token.lexeme = nums;
                    }
                    else
                        token.valueR = float.Parse(nums);
                }

                else
                {

                    //Int32.TryParse(nums, out token.value);
                    token.value = Int32.Parse(nums);

                }
                
                         
    }




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

        public void printToken(Token token)
        {
            string output = "";
           /* Tuple<string, string, int, float, string> tuple=
                new Tuple<string, string, int, float, string>(token.token, token.lexeme,token.value, token.valueR, token.literal);
                */
            if (!String.IsNullOrEmpty(token.token))
            {

                if (token.token == "numt")
                {
                    if (lexicalScanner.hasDot == false)
                    {
                        output = string.Format("{0,-15}  {1,-15} {2,-15}", token.token, token.lexeme, token.value);
                        // Console.WriteLine(token.token + "\t\t" + token.lexeme + "\t\t" + token.value);
                    }

                    else
                        //Console.WriteLine(token.token + "\t\t" + token.lexeme + "\t\t" + token.valueR);
                        output = string.Format("{0,-15}  {1,-15} {2,-15}", token.token, token.lexeme,token.valueR);
                }
                else if (token.token == "literalt")
                {
                    //Console.WriteLine(token.token + "\t\t" + token.lexeme + "\t\t" + token.literal);
                    output = string.Format("{0,-15}  {1,-15} {2,-15}", token.token, token.lexeme, token.literal);

                }
                else
                {
                     output = string.Format("{0,-15}  {1,-15}", token.token , token.lexeme);
                        //Console.WriteLine( token.token + "\t\t" + token.lexeme);
                     //Console.WriteLine(output);

                }

                Console.WriteLine(output);
            }

        } // end print token

    }
}
