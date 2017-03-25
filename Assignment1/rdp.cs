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
        //SymTab.entryType et;
        SymTab.entry ptr;
        //SymTab.entry.var varPtr;


       
        

        //SymTab.entry.var v = new SymTab.entry.var();

        //Objects containing data for symbol table inserts
        //SymTab.function func;
        //SymTab.var var;
        //SymTab.constant.floatConstant fc;
        //SymTab.constant.intConstant ic;

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


            checkForDups();
            st.insert(token.lexeme, lexicalScanner.SYMBOL.proct, depth);
            ptr = st.lookUp(token.lexeme);
            match(lexicalScanner.SYMBOL.idt);

            SymTab.entry.function f = new SymTab.entry.function();

            //Console.WriteLine(f.token);
            //Args
            if (error != true)
                pArgs(ref f);
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

        private void typeMark(ref SymTab.entryType typeOfEntry, ref SymTab.varType typeOfVar)
        {
            switch(token.token)
            {
                case (lexicalScanner.SYMBOL.integert):
                    typeOfEntry = SymTab.entryType.varEntry;
                    typeOfVar = SymTab.varType.intType;
                    match(lexicalScanner.SYMBOL.integert);
                    
                    break;
                case (lexicalScanner.SYMBOL.floatt):
                    typeOfEntry = SymTab.entryType.varEntry;
                    typeOfVar = SymTab.varType.floatType;
                    match(lexicalScanner.SYMBOL.floatt);
                
                    break;
               /* case (lexicalScanner.SYMBOL.realt):
                    match(lexicalScanner.SYMBOL.realt);
                    break;*/
                case (lexicalScanner.SYMBOL.chart):
                    typeOfEntry = SymTab.entryType.varEntry;
                    typeOfVar = SymTab.varType.charType;
                    match(lexicalScanner.SYMBOL.chart);
                  

                    break;
                case (lexicalScanner.SYMBOL.constt):
                    typeOfEntry = SymTab.entryType.constEntry;
                    match(lexicalScanner.SYMBOL.constt);
                    match(lexicalScanner.SYMBOL.assignopt);
                    

                    if (error != true)
                        value(ref typeOfVar);
                    
                    break;
            }




        }

       
        /// <summary>
        /// Checks if token has value (number token)
        /// </summary>
        private void value(ref SymTab.varType typeOfVar)
        {
            //Numberical Literal
            if(token.valueR.HasValue)
            {
                //We have a float!
                typeOfVar = SymTab.varType.floatType;
            }
            else
            {
                typeOfVar = SymTab.varType.intType;
                //We have a integer!
            }

            match(lexicalScanner.SYMBOL.numt);
            
        }

        /// <summary>
        /// Function for grammar rule Args -> ( ArgList ) | e
        /// </summary>
        private void pArgs(ref SymTab.entry.function f)
        {
            //f.token = token.token;
            //Console.WriteLine(f.token);
            switch(token.token)
            {
                case (lexicalScanner.SYMBOL.lparent):
                    match(lexicalScanner.SYMBOL.lparent);
                    if (error != true)
                        argList(ref f);
                    match(lexicalScanner.SYMBOL.rparent);
                    break;
                default:
                    //empty 
                    break;
            }



        }

        private void argList(ref SymTab.entry.function f)
        {

            

            if (error != true)
                mode(ref f);
            //match(lexicalScanner.SYMBOL.idt);

            if (error != true)
            {
                idList();

            }
                
            if (error != true)
                moreArgs(ref f);

        }

        private void moreArgs(ref SymTab.entry.function f)
        {
            switch(token.token)
            { 
                case (lexicalScanner.SYMBOL.semicolont):
                    match(lexicalScanner.SYMBOL.semicolont);
                    if(error!= true)
                        argList(ref f);
                    break;
                default:
                    //empty / lambda
                    break;
            }
        }

        private void idList()
        {
            SymTab.entry ptr;
            SymTab.entry.var v = new SymTab.entry.var();

            switch (token.token)
            {
                case (lexicalScanner.SYMBOL.idt):
                    checkForDups();
                    st.insert(token.lexeme, token.token, depth);
                    ptr = st.lookUp(token.lexeme);

                    match(lexicalScanner.SYMBOL.idt);
                    if (error != true)
                        idListTail(ptr);
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

        private void idListTail(SymTab.entry ptr)
        {
            switch(token.token)
                {
                case (lexicalScanner.SYMBOL.commat):
                    match(lexicalScanner.SYMBOL.commat);

                    //Action 1
                    checkForDups();
                    st.insert(token.lexeme, token.token, depth);
                    ptr = st.lookUp(token.lexeme);

                    match(lexicalScanner.SYMBOL.idt);
                    if(error != true)

                        idListTail(ptr);
                    break;
                case(lexicalScanner.SYMBOL.colont):

                    SymTab.entry.var v = new SymTab.entry.var();

                    match(lexicalScanner.SYMBOL.colont);    
                    if (error != true)
                        typeMark(ref v.typeOfEntry, ref v.typeOfVar);

                    

                    break;
                }

        }

        private void mode(ref SymTab.entry.function f)
        {
            switch (token.token)
            {
                case (lexicalScanner.SYMBOL.intt):
                    f.mode = lexicalScanner.SYMBOL.intt;
                    match(lexicalScanner.SYMBOL.intt);
                    break;
                case (lexicalScanner.SYMBOL.outt):
                    f.mode = lexicalScanner.SYMBOL.outt;
                    match(lexicalScanner.SYMBOL.outt);
                    break;
                case (lexicalScanner.SYMBOL.inoutt):
                    f.mode = lexicalScanner.SYMBOL.inoutt;
                    match(lexicalScanner.SYMBOL.inoutt);
                    break;
                default:
                    //empty 
              
                    break;
            }
            
        }
        
        void checkForDups()
        {
            
            SymTab.entry eptr;
           

            eptr = st.lookUp(token.lexeme);

            //Console.WriteLine("Found: " + eptr.lexeme);

            if (eptr.depth == depth && eptr.lexeme == token.lexeme)
            {
                Console.WriteLine("Error - Multiple declaration of \"" + eptr.lexeme + "\" on line " + lexicalScanner.ln);

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
