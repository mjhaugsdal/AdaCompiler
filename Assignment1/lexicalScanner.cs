using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Globalization;

/// <summary>
/// Name: Markus Johan Haugsdal
/// Class: CSC 446 Compiler Construction
/// Assignment: 1
/// Due Date: 01.02.2017
/// Instructor: Hamer
/// 
/// Description: Lexical scanner for determining tokens
/// 
/// </summary>

namespace Assignment1
{
    public class lexicalScanner
    {
        
        //variables
        char ch;
        public static int i = 0;
        //public static string token;
        string lexeme;
        private string fileName;
        private StreamReader sr;
        public static bool hasDot = false;


        public enum SYMBOL { begint, modult, constt, proct, ist,
            ift, thent, elset, elsift, whilet, loopt,
            floatt, integert, chart, gett, putt, endt, 
            andt, eoft, unkownt,
            relopt, addopt, assignopt, multopt, lparent,
            rparent,commat,colont,semicolont,
            periodt,idt,literalt,numt };

        //Token object for token building
        public class Token
        {
            public SYMBOL token = SYMBOL.unkownt;
            public string lexeme;
            public int value;
            public float valueR;
            public string literal;

        }
        


        //Dictionary for checking reserved word tokens
        Dictionary<string, SYMBOL> reswords = 
            new Dictionary<string, SYMBOL>(StringComparer.OrdinalIgnoreCase);

        public lexicalScanner(string fileName)
        {
            this.fileName = fileName;
        }

        public lexicalScanner(string fileName, StreamReader sr) : this(fileName)
        {
            this.sr = sr;
        }

        //Gets next char and iterates position in file by 1
        public char getNextChar()
        {

            ch =(char)sr.Read();
            return ch;
        }
        //Gets next char but does NOT iterate position
        public char peekNextChar()
        {
            return (char)sr.Peek();
        }

        /// <summary>
        /// Checks first char and starts building the token
        /// </summary>
        /// <returns> Object of type "token" </returns>
        public Token getNextToken()
        {

            Token token = new Token();

            while (!sr.EndOfStream)
            {
                //Only peek! 
                ch = peekNextChar();
                if (!Char.IsWhiteSpace(ch) && Enum.IsDefined
                    (typeof(SYMBOL), SYMBOL.unkownt))
                {
                    //If peek was successful, get next char
                    ch = getNextChar();
                    processToken(token); //Process the token

                    return token;

                }
                //If newline or whitespace
                if (ch == 10 || Char.IsWhiteSpace(ch)) 
                {
                    ch = getNextChar();
                }
                else 
                {
                    return token;
                }
            }//end while eof
             //Final token to signify that eof was reached. (Maybe not necessary)
            token.token = SYMBOL.eoft; 
            return token;
        }
        /// <summary>
        /// ProcessToken. 
        /// Creates the lexeme. Checks first position 
        /// of lexeme to determine what to do.
        /// </summary>
        /// <param name="token"></param>
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
            else if (lexeme[0] == 60 || lexeme[0] == 62 || 
                     lexeme[0] == 61 || lexeme[0] == 47 ||
                     lexeme[0] == 58 || lexeme[0] == 40 || 
                     lexeme[0] == 41 || lexeme[0] == 44 || 
                     lexeme[0] == 59 || lexeme[0] == 34 ||
                     lexeme[0] == 46 || lexeme[0] == 43 ||
                     lexeme[0] == 42 )
                
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
                token.token = SYMBOL.unkownt;
                token.lexeme = lexeme;
            }
            // return lexeme;


        }


        /// <summary>
        /// Processes double tokens. 
        /// </summary>
        /// <param name="token"></param>
        private void processDoubleToken(Token token)
        {

            lexeme = lexeme + ch.ToString();
            token.lexeme = lexeme;
            getNextChar();

            if (lexeme[0] == 47 || lexeme[0] == 60 || lexeme[0] == 62)
            {
                token.token = SYMBOL.relopt;
            }
            else if(lexeme[0] == 58)
            {
                token.token = SYMBOL.assignopt;
            }

        }
        /// <summary>
        /// Single tokens.
        /// </summary>
        /// <param name="token"></param>
        private void processSingleToken(Token token)
        {
            
            //if =, >, <
       
            token.lexeme = lexeme;
            if (lexeme[0] == 61 || lexeme[0] == 60 || lexeme[0] == 62)
            {
                token.token = SYMBOL.relopt;
                
            }
            else if (lexeme[0] == 43 || lexeme[0] == 45)
            {
                token.token = SYMBOL.addopt;
                
            }
            else if(lexeme[0] == 42 || lexeme[0] == 47)
            {
                token.token = SYMBOL.multopt;
                
            }
            else if(lexeme[0] == 40)
            {
                token.token = SYMBOL.lparent;
            }
            else if (lexeme[0] == 41)
            {
                token.token = SYMBOL.rparent;
            }
            else if (lexeme[0] == 44)
            {
                token.token = SYMBOL.commat;
            }
            else if (lexeme[0] == 58)
            {
                token.token = SYMBOL.colont;
            }
            else if (lexeme[0] == 59)
            {
                token.token = SYMBOL.semicolont;
            }
            else if (lexeme[0] == 46)
            {
                token.token = SYMBOL.periodt;
            }
            else if (lexeme[0] == 34)
            {
                processStringLiteral(token);
            }


        }
        /// <summary>
        /// Processes word tokens. Iterates until it finds illegal character ()
        /// </summary>
        /// <param name="token"></param>
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

                token.token = SYMBOL.idt;
            }
            //If lexeme is a reserved word
          
            if(token.lexeme.Length > 17)
            {
                token.token = SYMBOL.unkownt;
            }


        }
        /// <summary>
        /// String literals, if singleToken detects the opening "
        /// </summary>
        /// <param name="token"></param>
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
                token.token = SYMBOL.literalt;
                token.literal = literal;
                token.lexeme = literal;
            }
            else
            {
                
                token.token = SYMBOL.unkownt;
              
                token.literal = literal;
                token.lexeme = literal;
            }

        }

        /// <summary>
        /// Num tokens. Iterates and looks for the . which signifies float/real
        /// </summary>
        /// <param name="token"></param>
        public void processNumToken(Token token)
        {
           // getNextChar();
            hasDot = false;
            string nums = lexeme[0].ToString();
            token.token = SYMBOL.numt;
            char p;
            if (char.IsWhiteSpace(ch)) // only one char
            {
                token.lexeme = nums;
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
                        token.token = SYMBOL.unkownt;
                        token.lexeme = nums;
                    }
                    else
                    {
                        token.lexeme = nums;
                        token.valueR = float.Parse(nums);
                    }
                        
                }

                else
                {

                    //Int32.TryParse(nums, out token.value);
                    token.lexeme = nums;
                    token.value = Int32.Parse(nums);

                }
                
                         
    }




        }

        /// <summary>
        /// Dictionary for reserved words
        /// </summary>
        public void createDictionary()
        {
            reswords.Add("begin", SYMBOL.begint);
            reswords.Add("module", SYMBOL.modult);
            reswords.Add("constant", SYMBOL.constt);
            reswords.Add("procedure", SYMBOL.proct);
            reswords.Add("is", SYMBOL.ist);
            reswords.Add("if", SYMBOL.ift);
            reswords.Add("then", SYMBOL.thent);
            reswords.Add("else", SYMBOL.elset);
            reswords.Add("elsif", SYMBOL.elsift);
            reswords.Add("while", SYMBOL.whilet);
            reswords.Add("loop", SYMBOL.loopt);
            reswords.Add("float", SYMBOL.floatt);
            reswords.Add("integer", SYMBOL.integert);
            reswords.Add("char", SYMBOL.chart);
            reswords.Add("get", SYMBOL.gett);
            reswords.Add("put", SYMBOL.putt);
            reswords.Add("end", SYMBOL.endt);
            reswords.Add("or", SYMBOL.addopt);
            reswords.Add("rem", SYMBOL.multopt);
            reswords.Add("mod", SYMBOL.multopt);
            reswords.Add("and", SYMBOL.multopt);


        }

        /// <summary>
        /// PrintToken method to show all tokens and its attribute
        /// </summary>
        /// <param name="token"></param>
        public void printToken(Token token)
        {
            string output = "";
            

            if (!String.IsNullOrEmpty(token.lexeme))
            {

                i++;
                if (token.token == SYMBOL.numt)
                {
                    if (lexicalScanner.hasDot == false)
                    {
                        output = string.Format("{0,-15}  {1,-15}", token.token, token.value);
                       
                    }

                    else
                       
                        output = string.Format("{0,-15}  {1,-15} ", token.token, token.valueR);
                }
                else if (token.token == SYMBOL.literalt)
                {
                    
                    output = string.Format("{0,-15}  {1,-15} ", token.token, token.lexeme);

                }
                else
                {
                     output = string.Format("{0,-15}  {1,-15}", token.token , token.lexeme);
                }

                Console.WriteLine(output);
            }

        } // end print token

    }
}
