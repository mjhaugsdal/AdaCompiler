using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assignment1
{
    class rdp
    {
   
        bool error = false;
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
        internal lexicalScanner.Token parse()
        {
            token = lx.getNextToken();
            //Got the tokens!
            /* START FOLLOWING THE GRAMMAR RULES!
             * 
             * prog
             * ->
             * proct, idt, Args, is
             * DeclerativePart
             * Procedures
             * begin
             * SeqOfStatements
             * end, idt.
             * 
             * NOTE: Changes will come after friday the 10th
             */
            //proct
            //
            
           
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

        private void seqOfStatements()
        {
            //EMPTY
            //throw new NotImplementedException();
        }

        private void procedures()
        {
            //-> Prog Procedures | e
            //throw new NotImplementedException();
        }

        private void declPart()
        {
            switch(token.token)
            {
                case (lexicalScanner.SYMBOL.idt):
                    if (error != true)
                        idList();
                    //match(lexicalScanner.SYMBOL.idt);
                    match(lexicalScanner.SYMBOL.colont);
                    if (error != true)
                        typeMark();
                    match(lexicalScanner.SYMBOL.semicolont);
                    if (error != true)
                        declPart();
                    break;
                default:
                    Console.WriteLine("empty");
                    break;
            }

            //throw new NotImplementedException();
        }

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
                case (lexicalScanner.SYMBOL.chart):
                    match(lexicalScanner.SYMBOL.chart);
                    break;
                case (lexicalScanner.SYMBOL.constt):

                    throw new NotImplementedException();
                    match(lexicalScanner.SYMBOL.constt);
                    match(lexicalScanner.SYMBOL.assignopt);
                    value();
                    break;
            }
        }

        private void value()
        {
            //Numberical Literal

            match(lexicalScanner.SYMBOL.numt);



            //throw new NotImplementedException();
        }

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
                    Console.WriteLine("empty");

                    break;

            }
        }

        private void argList()
        {
            if (error != true)
                mode();
            //call idList()

            //match colont
            // match(lexicalScanner.SYMBOL.colont);
            //call TypeMark()
            //Call MoreArgs()
            // throw new NotImplementedException();
        }

        private void idList()
        {


            //
            //throw new NotImplementedException();
        }

        private void mode()
        {/*
            switch (token.token)
            {
                case ():
                    match(lexicalScanner.SYMBOL.lparent);
                    if (error != true)
                        argList();
                    match(lexicalScanner.SYMBOL.rparent);
                    break;
                default:
                    //empty 
                    Console.WriteLine("empty");

                    break;

            }
            */
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
                     
                    //Console.WriteLine(lexicalScanner.ln);
                    //sr.ReadToEnd();
                    //throw new Exception("Error in line " + lexicalScanner.ln + ". Expected token: " + desiredToken + ", Recieved: " + token.token);
                    Console.WriteLine("Error in line " + lexicalScanner.ln + ". Expected token: " + desiredToken + ", Recieved: " + token.token);

                    error = true;
                }

                else
                {
                    Console.WriteLine("MATCHED " + desiredToken + " AND " + token.token);
                    token = lx.getNextToken();
                }
            }
            else
            {
                token.token = lexicalScanner.SYMBOL.eoft;
            }
        }

       
    }//End class RDP
}
