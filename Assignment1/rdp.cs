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
   
        public static bool error = false;
        public int depth = 1;
        private lexicalScanner.Token token;
        private lexicalScanner lx;
        private StreamReader sr;
        private SymTab st;
        SymTab.entryType et;
        SymTab.entry ptr;
   

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

        public rdp(lexicalScanner.Token token, lexicalScanner lx, StreamReader sr, SymTab st) : this(token, lx, sr)
        {
            this.st = st;
        }


        //Parse
        internal lexicalScanner.Token parse(lexicalScanner.Token firstToken)
        {
            
            //Sets the first token
            token.token = firstToken.token;
            while (token.token == lexicalScanner.SYMBOL.commentt)
            {
                token = lx.getNextToken();
            }
            //token = lx.getNextToken();


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

           // et = SymTab.entryType.functionEntry;// We are entering a function to the table!

            //Check for duplicates!!!
            ptr = st.lookUp(token.lexeme);
            //If dup is found... multiple declaration!
            
            //else If NOT found
            st.insert(token.lexeme, lexicalScanner.SYMBOL.proct, depth);
            ptr = st.lookUp(token.lexeme);

            Console.WriteLine(ptr.depth);
            Console.WriteLine(ptr.lexeme);
            Console.WriteLine(ptr.token);

            //Start entering a function!
            SymTab.function t = new SymTab.function();






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
            switch (token.token)
            {
                case (lexicalScanner.SYMBOL.proct):
                    if(error != true)
                        parse(token);
                    break;
                default:
                    //Lambda / empty
                    break;
            }

        }

         
        private void declPart()
        {
            switch(token.token)
            {
                case (lexicalScanner.SYMBOL.idt):
                    if (error != true)
                    {
                        idList();
                    }
                    /*match(lexicalScanner.SYMBOL.colont);
                    if (error != true)
                        typeMark();*/
                    match(lexicalScanner.SYMBOL.semicolont);
                    if (error != true)
                        declPart();
                    break;
                default:
                    //Lambda / empty
                    break;
            }
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

        private void argList()
        {
            if (error != true)
                mode();
            //match(lexicalScanner.SYMBOL.idt);

            if (error != true)
                idList();
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

        private void idList()
        {
            switch(token.token)
            {
                case (lexicalScanner.SYMBOL.idt):

                    match(lexicalScanner.SYMBOL.idt);
                    if (error != true)
                        idListTail();
                    if (error != true)
                        idList();
                    break;

                default:
                    //empty
                    break;
            }



            //
            //throw new NotImplementedException();
        }

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
                case(lexicalScanner.SYMBOL.colont):
                    match(lexicalScanner.SYMBOL.colont);    
                    if (error != true)
                        typeMark();

                    break;
                }
        }

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
                     
                    //Console.WriteLine(lexicalScanner.ln);
                    //sr.ReadToEnd();
                    //throw new Exception("Error in line " + lexicalScanner.ln + ". Expected token: " + desiredToken + ", Recieved: " + token.token);
                    Console.WriteLine("Error in line " + lexicalScanner.ln + ". Expected token: " + desiredToken + ", Recieved: " + token.token);

                    error = true;
                }

                else
                {
                   // Console.WriteLine("MATCHED " + desiredToken + " AND " + token.token);
                    token = lx.getNextToken();
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
