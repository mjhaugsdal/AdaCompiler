using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/// <summary>
/// Name: Markus Johan Haugsdal
/// Class: CSC 446 Compiler Construction
/// Assignment: 2
/// Due Date: 13.02.2017
/// Instructor: Hamer
/// 
/// Description: Recursive descent parser. Determines if the source file follows correct grammar.
/// 
/// </summary>

namespace Assignment1
{
    class rdp
    {
   
        public static bool error = false;
        private lexicalScanner.Token token;
        private lexicalScanner lx;
        private StreamReader sr;

        public rdp(lexicalScanner.Token token)
        {
            this.token = token;
        }

        public rdp(lexicalScanner.Token token, lexicalScanner lx) : this(token)
        {
            this.lx = lx;
        }

        public rdp(lexicalScanner.Token token, lexicalScanner lx, StreamReader sr) : this(token, lx)
        {
            this.sr = sr;
        }

        //Parse
        /// <summary>
        /// Prog -> ..
        /// Parses a program starting with procedure
        /// </summary>
        /// <param name="firstToken"></param>
        /// <returns>A eoft</returns>
        internal lexicalScanner.Token parse(lexicalScanner.Token firstToken)
        {
            //Sets the first token
            token.token = firstToken.token;
            while (token.token == lexicalScanner.SYMBOL.commentt)
            {
                //Ignore the comments!
                token = lx.getNextToken();
            }

            match(lexicalScanner.SYMBOL.proct);
            //idt
            match(lexicalScanner.SYMBOL.idt);
            //Args
            if(error != true)
                pArgs();
            //is
            match(lexicalScanner.SYMBOL.ist);
            //DeclarativePart
            if (error != true)
                declPart();
            //Procedures
            if (error != true)
                procedures();
            //Match begint
            match(lexicalScanner.SYMBOL.begint);
            //Not implemented. Sequence of statements.
            if (error != true)
                seqOfStatements();

            match(lexicalScanner.SYMBOL.endt);
            match(lexicalScanner.SYMBOL.idt);
            match(lexicalScanner.SYMBOL.semicolont);

           
            return token;

        }
        /// <summary>
        /// NOT IMPLEMENTED YET
        /// </summary>
        private void seqOfStatements()
        {
            //EMPTY, for next assignment!
            //throw new NotImplementedException();
        }
        /// <summary>
        /// Procedure grammar rule, calls new parse.
        /// </summary>
        private void procedures()
        {

            //-> Prog Procedures | e
            switch (token.token)
            {
                case (lexicalScanner.SYMBOL.proct):
                    if (error != true)
                    {
                        parse(token);
                        procedures();
                    }

                    break;
                default:
                    //Lambda / empty
                    break;
            }

        }

        /// <summary>
        /// DeclerativePart grammar rule. 
        /// </summary>
        private void declPart()
        {
            switch(token.token)
            {
                case (lexicalScanner.SYMBOL.idt):
                    if (error != true)
                    {
                        idList();
                    }
                    match(lexicalScanner.SYMBOL.colont);
                    if (error != true)
                        typeMark();
                    match(lexicalScanner.SYMBOL.semicolont);
                    if (error != true)
                        declPart();
                    break;
                default:
                    //Lambda / empty
                    break;
            }
        }
        /// <summary>
        /// TypeMark Grammar rule
        /// </summary>
        private void typeMark()
        {
            switch(token.token)
            {
                case (lexicalScanner.SYMBOL.integert):
                    match(lexicalScanner.SYMBOL.integert);
                    break;
                case (lexicalScanner.SYMBOL.floatt):
                    match(lexicalScanner.SYMBOL.floatt);
                    break;
               /* case (lexicalScanner.SYMBOL.realt):
                    match(lexicalScanner.SYMBOL.realt);
                    break;*/
                case (lexicalScanner.SYMBOL.chart):
                    match(lexicalScanner.SYMBOL.chart);
                    break;
                case (lexicalScanner.SYMBOL.constt):

                    match(lexicalScanner.SYMBOL.constt);
                    match(lexicalScanner.SYMBOL.assignopt);
                    if(error != true)
                        value();

                    break;
            }
        }

        /// <summary>
        /// Checks if token has value (number token)
        /// </summary>
        private void value()
        {
            //Numberical Literal
            match(lexicalScanner.SYMBOL.numt);
            
        }

        /// <summary>
        /// Function for grammar rule Args -> ( ArgList ) | e
        /// </summary>
        private void pArgs()
        {

            switch(token.token)
            {
                case (lexicalScanner.SYMBOL.lparent):
                    match(lexicalScanner.SYMBOL.lparent);
                    if (error != true)
                        argList();
                    match(lexicalScanner.SYMBOL.rparent);
                    break;
                default:
                    //empty 
                    

                    break;

            }
        }
        /// <summary>
        /// ArgList grammar rule
        /// </summary>
        private void argList()
        {
            if (error != true)
                mode();
            if(error != true)
                idList();
            match(lexicalScanner.SYMBOL.colont);

            if (error != true)
                typeMark();
            if (error != true)
                moreArgs();
        }

        private void moreArgs()
        {
            switch(token.token)
            { 
                case (lexicalScanner.SYMBOL.semicolont):
                    match(lexicalScanner.SYMBOL.semicolont);
                    if(error!= true)
                        argList();
                    break;
                default:
                    //empty / lambda
                    break;
            }
        }
        /// <summary>
        /// IdList grammar rule
        /// </summary>
        private void idList()
        {
            match(lexicalScanner.SYMBOL.idt);
            idListTail();

            //
            //throw new NotImplementedException();
        }
        /// <summary>
        /// Trailing IdList to remove left recursion
        /// </summary>
        private void idListTail()
        {
            switch(token.token)
                {
                case (lexicalScanner.SYMBOL.commat):
                    match(lexicalScanner.SYMBOL.commat);
                    match(lexicalScanner.SYMBOL.idt);
                    if(error != true)
                        idListTail();
                    break;
                default:
                    //Empty / lambda
                    break;
                }
        }
        /// <summary>
        /// Mode grammar rule
        /// </summary>
        private void mode()
        {
            switch (token.token)
            {
                case (lexicalScanner.SYMBOL.intt):
                    match(lexicalScanner.SYMBOL.intt);
                    break;
                case (lexicalScanner.SYMBOL.outt):
                    match(lexicalScanner.SYMBOL.outt);
                    break;
                case (lexicalScanner.SYMBOL.inoutt):
                    match(lexicalScanner.SYMBOL.inoutt);
                    break;
                default:
                    //empty 
              
                    break;
            }
            
        }
        

        /// <summary>
        /// Matches grammar rule to current token. If there is a match, it gets the next token.
        /// 
        /// </summary>
        /// <param name="token"></param>
        private void match(lexicalScanner.SYMBOL desiredToken)
        {
            if (error != true)
            {            
                if (desiredToken != token.token)
                {
                     
                    //Alternative to using bool error for every match... not suitable for grading
                    //throw new Exception("Error in line " + lexicalScanner.ln + ". Expected token: " + desiredToken + ", Recieved: " + token.token);
                    Console.WriteLine("Error in line " + lexicalScanner.ln + ". Expected token: " + desiredToken + ", Recieved: " + token.token);

                    error = true;
                }

                else
                {
                    
                    token = lx.getNextToken();
                    //Ignore the comments!
                    while(token.token == lexicalScanner.SYMBOL.commentt)
                    {
                        token = lx.getNextToken();
                    }
                }
            }
            else
            {
                token.token = lexicalScanner.SYMBOL.eoft;
            }
        }
    }//End class RDP
}
