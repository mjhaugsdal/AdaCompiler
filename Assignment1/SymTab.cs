

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;




namespace Assignment1
{
    class SymTab<T>
    {
        public class Node
        {
            private Node next;
            private T data;
            public Node(T t)
            {
                next = null;
                data = t;
            }

            public Node Next
            {
                get { return next; }
                set { next = value; }
            }
            public T Data
            {
                get { return data; }
                set { data = value; }
            }

        }

        private Node head;
        public SymTab()
        {
            head = null;
        }

        public void addHead(T t)
        {
            Node n = new Node(t);
            n.Next = head;
            head = n;

        }

    }
}





/*
    class SymbTab<T>
    {

        public enum varType { charType, intType, floatType }

        class paramNode
        {
            public varType typeOfParam;
            public paramNode next;
        }
        public class paramList
        {
            private paramNode head;

            public void addFirst(varType typeOfVar)
            {
                paramNode newNode = new paramNode();

                newNode.typeOfParam = typeOfVar;
                newNode.next = head;

            }
        }

        public class Node
        {

            private Node next;
            private T entryType;

            public Node (T t)
            {
                next = null;
                entryType = t;
            }

            struct var
            {
                lexicalScanner.SYMBOL token;
                string lexeme;
                int depth;
                varType typeOfVar;
                int Offset;
                int size;
            }
            struct realConstant
            {
                lexicalScanner.SYMBOL token;
                string lexeme;
                int depth;
                varType typeOfVar;
                int Offset;
                float valueR;
            }
            struct intConstant
            {
                lexicalScanner.SYMBOL token;
                string lexeme;
                int depth;
                varType typeOfVar;
                int Offset;
                int value;
            }
            struct function
            {
                lexicalScanner.SYMBOL token;
                string lexeme;
                int depth;
                int sizeOfLocal;
                int numberOfParameters;
                varType returnType;
                paramList prList;

            }
        }

        private Node head;
        //Constructor
        public SymbTab()
        {
            head = null;

        }

        public void insert(T t)
        {
            Node n = new Node(t);

        }


    }
}*/

