
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

        LinkedList<entry>[] hashTable = new LinkedList<entry> [tableSize];


       // LinkedList<entry> hashTable = new LinkedList<entry>();

        public  SymTab()
        {

            for(int i =0; i<tableSize; i++)
            {
                //hashTable[i].AddFirst(new entry()); 

        //hashTable.AddLast(new entry());
            }

        }


        public void writeTable()
        {
            for(int i = 0; i<hashTable.Length; i++)
            {
                for(int j=0; j<hashTable[i].Count; j++)
                Console.WriteLine(hashTable[i].ElementAt(j));
            }
        }


        public void insert(string lexeme, lexicalScanner.SYMBOL token, int depth)
        {
                


        }

        public void lookUp()
        {

        }
        public void deleteDepth()
        {

        }
        public void hash()
        {

        }
            
            
            /*
        switch(token)
        {
                case (lexicalScanner.SYMBOL.proct):
                    Console.Write("FUNCTION");

                    function tFunc = new function();
                    tFunc.depth = depth;
                    tFunc.lexeme = lexeme;
                    tFunc.token = lexicalScanner.SYMBOL.proct;
                    

                    hashTable.AddFirst(tFunc);

                    break;
                //entry.function temp = new entry.function();
               

                case(lexicalScanner.SYMBOL.integert):

                    var tVar = new var();
                    tVar.depth = depth;
                    tVar.token = lexicalScanner.SYMBOL.integert;
                    tVar.typeOfVar = varType.intType;
                    hashTable.AddFirst(tVar);
                    break;

                case (lexicalScanner.SYMBOL.constt):

                default:
                    break;
        }*/
        //}
                


    }// emd class symTab
}



