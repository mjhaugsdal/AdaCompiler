
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assignment1
{
    public class SymTab
    {
        const int tableSize = 10;
        public enum varType { charType, intType, floatType, def }

        public class entry
        {
            public lexicalScanner.SYMBOL token = lexicalScanner.SYMBOL.unkownt;
            public string lexeme = "";
            public int depth = 0; 

        }

         class var : entry
        {
            public varType typeOfVar = varType.def;
            public int Offset = 0;
            public int size = 0 ;
        }

         class constant : entry
        {
            public varType typeOfConstant = varType.def;
             class intConstant : constant
            {
                public int value=0;

            }
             class floatConstant : constant
            {
                public float valueR =0;
            }
        } // end constant

         class function : entry 
        {
            public int sizeOfLocal =0;
            public  int numberOfParams =0;
            public varType returnType = varType.def;
            public LinkedList<varType> paramList = new LinkedList<varType>();

            public function()
            {

            }

        }
        
                LinkedList<entry> hashTable = new LinkedList<entry>();

                public  SymTab()
                {

                    for(int i =0; i<tableSize; i++)
                    { 
                        hashTable.AddLast(new entry());
                    }

                }


                public void writeTable()
                {
                    for(int i = 0; i<hashTable.Count; i++)
                    {
                        Console.WriteLine(hashTable.ElementAt(i).token);
                    }
                }
                public void insert(string lexeme, lexicalScanner.SYMBOL token, int depth)
                {
                    switch(token)
                    {
                        case (lexicalScanner.SYMBOL.proct):
                   

                            //entry.function temp = new entry.function();
                            hashTable.AddFirst(new function());

                            break;

                        default:
                            break;
                    }
                }
                


    }// emd class symTab
}



