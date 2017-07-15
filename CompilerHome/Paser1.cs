using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CompilerHome
{
    class Paser1
    {
        private int index = 0;
        private List<Token> list = new List<Token>();
        private Token current;
        string label = "";
        private int flageOpera = 0;

        private List<Var> gobal = new List<Var>();
        private List<Var> local = new List<Var>();
        private List<Function> pro = new List<Function>();
        private List<Function> func = new List<Function>();
        private List<Function> call = new List<Function>();

        private Stack mystack = new Stack();
        private ArrayList array_Infix = new ArrayList();
        private ArrayList array_postfix = new ArrayList();
        private ArrayList array_Tuple = new ArrayList();
        private int syntax = 0;
        private int addrs = 1;
        private string sl;

        private ArrayList arraySW = new ArrayList();
        public List<Inter> inter = new List<Inter>();

        private int countTemp = 0;
        private int countLable = 1;
        private int countAgu = 0;
        private int flagecall = 0;
        private int flagesw = 0;
        private Stack stackSW = new Stack();
        private string nameFP = "";



        private Stack stacklabel = new Stack();

        public Paser1()
        {

        }

        public Paser1(List<Token> list)
        {
            this.list = list;
        }

        public Token getToken()
        {
            return list[index++];
        }

        private string Temp()
        {
            return "T" + countTemp++;
        }

        private string Label()
        {
            return "L" + countLable++;
        }


        public void S()
        {

            current = getToken();
            if (current.Word == "int" || current.Word == "float" || current.Word == "string")
            {
                string id = "";
                string para = "";
                addrs = 1;
                T(ref id, ref para, "");
                P();
                F();
            }
            else if (current.Word == "#")
            {

            }
            else
            {
                printSystaxError("S()", current.Word);
            }
            if (current.Word == "#" && syntax == 0)
            {
                Console.WriteLine("Syntax True");
            }
        }

        private void P()
        {
            if (current.Type == "ID")
            {

                P1();
                P();
                if (current.Word == ";")
                {
                    current = getToken();
                }
            }
            else if (current.Word == "Func" || current.Word == "#")
            {

            }
            else
            {
                printSystaxError("P()", current.Word);
            }
        }

        private void P1()
        {
            if (current.Type == "ID")
            {
                string id_temp = "";
                string para = "";
                string name = "";
                string nameScope = "";
                name = current.Word;
                nameFP = "P";
                current = getToken();
                if (current.Word == "(")
                {
                    current = getToken();
                    addrs = 0;
                    sl = "Local0";
                    T(ref id_temp, ref para, name);
                    if (findDupilcate(name, "Gobal", ref nameScope) == true)
                    {
                        SemanticError("ProtoType Dupicate: ", name, "", "");
                        Environment.Exit(0);
                    }
                    /*if(findDupilcate(name, "P",ref nameScope) != true && findDupilcate(name, "Gobal",ref nameScope) != true)
                    {
                        pro.Add(new Function(name,para));
                    }
                    else
                    {
                        SemanticError("ProtoType Dupicate: ", name, "", "");
                        Environment.Exit(0);
                    }
                    */
                    // protoType ซ้ำได้ แต่paraห้ามซ้ำ
                    // เช็คpara 
                    foreach (var f in pro)
                    {
                        if (name == f.getName() && para == f.getPara())
                        {
                            SemanticError("ProtoType Dupicate para: ", name, "", "");
                            Environment.Exit(0);
                        }
                    }
                    pro.Add(new Function(name, para));
                    /*if ()
                    {
                        pro.Add(new Function(name, para));
                    }

                    else
                    {
                        SemanticError("ProtoType Dupicate: ", name, "", "");
                        Environment.Exit(0);
                    }
                    */
                    removeVar(sl);
                    if (current.Word == ")")
                    {
                        current = getToken();
                        if (current.Word == ";")
                        {

                            nameFP = "";
                            current = getToken();

                        }
                        else
                        {
                            printSystaxError("P1()", current.Word);
                        }
                    }
                    else
                    {
                        printSystaxError("P1()", current.Word);
                    }

                }
                else
                {
                    printSystaxError("P1()", current.Word);
                }

            }
            else
            {
                printSystaxError("P1()", current.Word);
            }
        }

        private void F()
        {
            string id_temp = "";
            string Type_temp = "";
            string name = "";
            string para = "";
            string nameFunction = "";
            string nameScope = "";
            nameFP = "F";
            if (current.Word == "Func")
            {

                current = getToken();
                name = current.Word;
                sl = "Local1";
                if (current.Type == "ID")
                {
                    /*
                    if (findDupilcate(name, "F",ref nameScope) == true)
                    {
                        Console.WriteLine("===========Function Duplication===========");
                        Console.WriteLine("Func " + name + "() ");
                        Environment.Exit(0);
                       
                    }*/
                    nameFunction = name;
                    current = getToken();
                    if (current.Word == "(")
                    {
                        current = getToken();
                        T3(ref para);

                        //Check name Function Declare 
                        /*if(checkFunction(name,para,"f", nameFunction))
                        {
                            func.Add(new Function(name,para));
                        }
                        else
                        {
                            SemanticError("Function Not Declare: ", name, "", "");
                            Environment.Exit(0);
                        }
                        */
                        bool nameDec = false;
                        foreach (var i in pro)
                        {
                            if (name == i.getName())
                            {
                                nameDec = true;
                            }
                        }
                        if (nameDec == false)
                        {
                            SemanticError("Function Not Declare: ", name, "", "");
                            Environment.Exit(0);
                        }
                        if (!checkFunction(name, para, "f", nameFunction))
                        {
                            Console.WriteLine("===========Function Paramiter not equal:===========");
                            Console.WriteLine("Func: " + name + " (" + para + ") " + "");
                            Environment.Exit(0);
                        }
                        else
                        {
                            // check Dup Function 
                            foreach (var f in func)
                            {
                                if (name == f.getName() && para == f.getPara())
                                {
                                    Console.WriteLine("===========Function Duplication===========");
                                    Console.WriteLine("Func " + name + "() ");
                                    Environment.Exit(0);
                                }
                            }

                            func.Add(new Function(name, para));
                        }
                        if (current.Word == ")")
                        {
                            current = getToken();
                            if (current.Word == "{")
                            {
                                current = getToken();
                                T(ref id_temp, ref Type_temp, nameFunction);
                                S1(nameFunction);
                                if (current.Word == "}")
                                {
                                    nameFP = "";
                                    removeVar(sl);
                                    //removeFunc(nameFunction);
                                    current = getToken();
                                    F();
                                }
                                else
                                {
                                    printSystaxError("F()", current.Word);
                                }
                            }
                            else
                            {
                                printSystaxError("F()", current.Word);
                            }
                        }
                        else
                        {
                            printSystaxError("F()", current.Word);
                        }

                    }
                    else
                    {
                        printSystaxError("F()", current.Word);
                    }
                }
                else
                {
                    printSystaxError("F()", current.Word);
                }
            }
            else if (current.Word == "#")
            {

            }
            else
            {
                printSystaxError("F()", current.Word);
            }
        }

        private void S1(string nameFunction)
        {

            if (current.Type == "ID")
            {
                flagesw = 0;
                A(nameFunction);
                S1(nameFunction);
            }
            else if (current.Word == "switch")
            {
                flagesw = 1;
                W(nameFunction);
                S1(nameFunction);
            }
            else if (current.Word == "call")
            {
                flagesw = 0;
                C(nameFunction);
                S1(nameFunction);
            }
            else if (current.Word == "}" || current.Word == "#" || current.Word == "case" || current.Word == "default")
            {

            }
            else
            {
                printSystaxError("S1()", current.Word);
            }


        }

        private void C(string nameFunction)
        {
            string para = "";
            string name = "";
            string Type = "";
            string nameScope = "";
            string nameCall = "";
            countAgu = 0;
            if (current.Word == "call")
            {
                current = getToken();
                if (current.Type == "ID")
                {
                    name = current.Word;
                    nameCall = name;
                    flagecall = 1;
                    /*
                    if (chckeCallDup(name))
                    {
                        Console.WriteLine("===========Call Dupilcate  : in Function===========");
                        Console.WriteLine("Func " + nameFunction + "() " + "call " + name + "()");
                        Environment.Exit(0);
                    }
                    */
                    /*if(name==nameFunction)
                    {
                        Console.WriteLine("===========Call Dupilcate  : in Function===========");
                        Console.WriteLine("Func " + nameFunction + "() " + "call " + name + "()");
                        Environment.Exit(0);
                    }
                    */

                    // check Delare in ProtoType ? 
                    /*if (findDupilcate(name, "P",ref nameScope) == false)
                    {

                        Console.WriteLine("===========Call not Delare  : in Function===========");
                        Console.WriteLine("Func " + nameFunction + "() " + "call "+name+"()");
                        Environment.Exit(0);
                    }
                    */
                    current = getToken();
                    if (current.Word == "(")
                    {
                        current = getToken();
                        E(ref para, current.Word, nameFunction, ref Type);

                        /*if (checkFunction(name, para,"c",nameFunction))
                        {
                            call.Add(new Function(name, para));
                        }
                        else
                        {
                            Console.WriteLine("===========Call not Delare  : in Function===========");
                            Console.WriteLine("Func " + nameFunction + "() " + "call " + name + "()");
                            Environment.Exit(0);
                           
                        }
                        */
                        //check Delare 
                        if (!checkFunction(name, para, "c", nameFunction))
                        {
                            Console.WriteLine("===========Call not Delare  : in Function===========");
                            Console.WriteLine("Func " + nameFunction + "() " + "call " + name + "()");
                            Environment.Exit(0);
                        }
                        call.Add(new Function(name, para));
                        if (current.Word == ")")
                        {
                            if (checkFunction(name, para, "c", nameFunction))
                            {
                                //call.Add(new Function(name, para));
                                /*
                                Console.WriteLine("===========Call Paramiter not equal in Function:===========");
                                Console.WriteLine("Func " + nameFunction + "() " + "Call: " + name + " (" + para + ") " + "");
                                Environment.Exit(0);*/
                            }
                            else
                            {
                                foreach (var p in pro)
                                {
                                    if (name != p.getName() && para != p.getPara())
                                    {
                                        /*Console.WriteLine("===========Call not Delare  : in Function===========");
                                        Console.WriteLine("Func " + nameFunction + "() " + "call " + name + "()");
                                        Environment.Exit(0);
                                        */
                                        Console.WriteLine("===========Call Paramiter not equal in Function:===========");
                                        Console.WriteLine("Func " + nameFunction + "() " + "Call: " + name + " (" + para + ") " + "");
                                        Environment.Exit(0);
                                        /*Console.WriteLine("===========Function Duplication===========");
                                        Console.WriteLine("Func " + name + "() ");
                                        Environment.Exit(0);
                                        */
                                    }
                                }

                                func.Add(new Function(name, para));
                            }
                            /*if (checkFunction(name, para, "c", nameFunction))
                            {
                                call.Add(new Function(name, para));
                            }
                            else
                            {
                                Console.WriteLine("===========Call not Delare  : in Function===========");
                                Console.WriteLine("Func " + nameFunction + "() " + "call " + name + "()");
                                Environment.Exit(0);

                            }
                            */
                            inter.Add(new Inter("CALL", "-", countAgu.ToString(), nameCall));
                            array_Infix.Clear();
                            array_postfix.Clear();

                            current = getToken();
                            if (current.Word == ";")
                            {
                                current = getToken();
                                flagecall = 0;
                                countAgu = 0;
                            }
                            else
                            {
                                printSystaxError("C()", current.Word);
                            }
                        }
                        else
                        {
                            printSystaxError("C()", current.Word);
                        }
                    }
                    else
                    {
                        printSystaxError("C()", current.Word);
                    }
                }
                else
                {
                    printSystaxError("C()", current.Word);
                }
            }
            else
            {
                printSystaxError("C()", current.Word);
            }
        }

        private void E(ref string para, string name, string nameFunction, ref string id)
        {

            string Type = "";
            string temp_ = "";
            if (current.Type == "ID" || current.Type == "int_const" || current.Type == "float_const" || current.Type == "string_const")
            {

                I(ref id, ref Type);
                if (Type == "int_const")
                {
                    Type = "int";
                }
                else if (Type == "float_const")
                {
                    Type = "float";
                }
                else if (Type == "string_const")
                {
                    Type = "string";
                }
                else
                {
                    findTypeID(id, ref Type, "Local");
                }

                para = para + Type;
                countAgu++;
                if (flagesw == 1)
                {
                    if (stackSW.Peek().ToString() != Type)
                    {
                        Console.WriteLine("===========Assign Not Macthe in Function===========");
                        Console.WriteLine("Func " + nameFunction + "() switch()" + id + " " + Type);
                        Environment.Exit(0);
                    }
                }
                if (current.Word == "," && flagecall == 1)
                {
                    infixToPostfix(array_Infix);
                    //printPostfix();
                    postfixToTuple(ref array_postfix, ref temp_);
                    inter.Add(new Inter("ARG", "-", "-", temp_));
                    array_Infix.Clear();
                    array_postfix.Clear();
                    Type = "";
                    name = "";
                }

                E1(ref para, id, Type, nameFunction, ref temp_);


            }
            else if (current.Word == ")")
            {

            }
            else
            {
                printSystaxError("E()", current.Word);
            }
        }

        private void E1(ref string para, string name, string Type1, string nameFunction, ref string temp_)
        {
            string id = "";
            string Type = "";
            string temppara = "";
            bool first = false;
            string Op = "";
            // notchage name , Type 
            if (current.Word == "+" || current.Word == "-" || current.Word == "*" || current.Word == "/")
            {
                O(ref Op);

                I(ref id, ref Type);
                if (Type == "int_const")
                {
                    Type = "int";
                }
                else if (Type == "float_const")
                {
                    Type = "float";
                }
                else if (Type == "string_const")
                {
                    Type = "string";
                }
                else
                {
                    findTypeID(id, ref Type, "Local");
                }
                if (Type == "string")
                {
                    if (Op == "/" || Op == "*")
                    {
                        Console.WriteLine("===========Operation String Not Macthe in Function===========");
                        Console.WriteLine("Func " + nameFunction + "() " + id + " " + Type);
                        Environment.Exit(0);
                    }
                }
                if (checekdVarDelare(Type, id) == false)
                {
                    Console.WriteLine("===========Var Not Delare in Function===========");
                    Console.WriteLine("Func " + nameFunction + "() " + id);
                    Environment.Exit(0);
                }
                if (Type1 != Type)
                {
                    Console.WriteLine("===========Assign Not Macthe in Function===========");
                    Console.WriteLine("Func " + nameFunction + "() " + id + " " + Type);
                    Environment.Exit(0);
                }
                if (flageOpera == 1)
                {

                    flageOpera = 0;
                }
                else
                {
                    para = para + Type1;
                }
                if (flagecall == 1 && current.Word == ")")
                {
                    infixToPostfix(array_Infix);
                    //printPostfix();
                    postfixToTuple(ref array_postfix, ref temp_);
                    inter.Add(new Inter("ARG", "-", "-", temp_));
                    array_Infix.Clear();
                    array_postfix.Clear();
                }
                else if (flagecall == 1 && current.Word == ",")
                {
                    infixToPostfix(array_Infix);
                    //printPostfix();
                    postfixToTuple(ref array_postfix, ref temp_);
                    inter.Add(new Inter("ARG", "-", "-", temp_));
                    array_Infix.Clear();
                    array_postfix.Clear();
                    Type1 = "";
                    name = "";
                }
                E1(ref para, name, Type1, nameFunction, ref temp_);
            }
            else if (current.Word == ",")
            {
                current = getToken();

                I(ref id, ref Type);

                if (Type == "int_const")
                {
                    Type = "int";
                }
                else if (Type == "float_const")
                {
                    Type = "float";
                }
                else if (Type == "string_const")
                {
                    Type = "string";
                }
                else
                {
                    findTypeID(id, ref Type, "Local");
                }
                if (checekdVarDelare(Type, id) == false)
                {
                    Console.WriteLine("===========Var Not Delare in Function===========");
                    Console.WriteLine("Func " + nameFunction + "() " + id);
                    Environment.Exit(0);
                }

                if (first == false)
                {
                    name = id;
                    Type1 = Type;
                }
                else if (first == true)
                {
                    if (Type1 != Type)
                    {
                        Console.WriteLine("===========Assign Not Macthe in Function===========");
                        Console.WriteLine("Func " + nameFunction + "() " + id + " " + Type);
                        Environment.Exit(0);
                    }
                }
                para = para + Type;
                temppara = para;
                countAgu++;
                if (flagecall == 1 && current.Word == ")")
                {
                    infixToPostfix(array_Infix);
                    //printPostfix();
                    postfixToTuple(ref array_postfix, ref temp_);
                    inter.Add(new Inter("ARG", "-", "-", temp_));
                    array_Infix.Clear();
                    array_postfix.Clear();
                }
                else if (flagecall == 1 && current.Word == ",")
                {
                    infixToPostfix(array_Infix);
                    //printPostfix();
                    postfixToTuple(ref array_postfix, ref temp_);
                    inter.Add(new Inter("ARG", "-", "-", temp_));
                    array_Infix.Clear();
                    array_postfix.Clear();
                    Type1 = "";
                    name = "";
                }
                E1(ref para, name, Type1, nameFunction, ref temp_);
            }
            //else if(current.Word==")" || current.Word ==";")
            else if (current.Word == ")" || current.Word == ";" || current.Word == ":")
            {

            }
            else
            {
                printSystaxError("E1()", current.Word);
            }
        }

        private void I(ref string id, ref string Type)
        {
            if (current.Type == "ID" || current.Type == "int_const" || current.Type == "float_const" || current.Type == "string_const")
            {
                array_Infix.Add(current.Word);
                if (current.Type == "ID")
                {
                    id = current.Word;
                    Type = "";

                }
                else
                {
                    //id = "";
                    id = current.Word;
                    Type = current.Type;

                }
                current = getToken();
            }
            else
            {
                printSystaxError("I()", current.Word);
            }
        }

        private void O(ref string op)
        {

            if (current.Word == "+" || current.Word == "-" || current.Word == "*" || current.Word == "/")
            {
                op = current.Word;
                array_Infix.Add(current.Word);
                current = getToken();
                flageOpera = 1;
            }
            else
            {
                printSystaxError("O()", current.Word);
            }
        }

        private void O1(ref string opera)
        {
            if (current.Word == ">" || current.Word == "<" || current.Word == ">=" || current.Word == "<=")
            {
                opera = current.Word;
                array_Infix.Add(current.Word);
                current = getToken();
            }
            else
            {
                printSystaxError("O1()", current.Word);
            }
        }

        private void A(string nameFunction)
        {
            array_Infix.Clear();
            array_postfix.Clear();
            string id = "";
            string Type = "";

            string name = "";
            string Type1 = "";
            string temp = "";
            string temp_ = "";
            string para = "";
            string nameScope = "";
            if (current.Type == "ID")
            {
                array_Infix.Add(current.Word);
                name = current.Word;

                // Check Var Delare in Gobal ? and Local1 (para)
                // first find var in Local1 
                // next find var in Gobal 
                if (findDupilcate(name, "Local1", ref nameScope) == false && findDupilcate(name, "Gobal", ref nameScope) == false)
                {
                    Console.WriteLine("===========Var Not Delare in Func:===========");
                    Console.WriteLine("Func " + nameFunction + "() " + " " + name + " " + Type);
                    Environment.Exit(0);
                }

                //findTypeID(name, ref Type1,nameScope);
                findTypeID(name, ref Type1, "Local");
                current = getToken();
                if (current.Word == "=")
                {
                    array_Infix.Add(current.Word);
                    current = getToken();
                    I(ref id, ref Type);
                    if (Type == "ID")
                    {
                        if (findDupilcate(id, "Local1", ref nameScope) == false)
                        {
                            Console.WriteLine("===========Var Not Delare in Func:===========");
                            Console.WriteLine("Func " + nameFunction + "() " + " " + name + " " + Type);
                            Environment.Exit(0);
                        }
                        else
                        {
                            findTypeID(id, ref Type, nameScope);

                        }
                    }
                    findTypeID(id, ref Type, "Local");
                    if (Type1 != Type)
                    {
                        Console.WriteLine("===========Assign Not Macthe in Function===========");
                        Console.WriteLine("Func " + nameFunction + "() " + id + " " + Type);
                        Environment.Exit(0);
                    }
                    E1(ref para, name, Type1, nameFunction, ref temp_);
                    if (current.Word == ";")
                    {

                        infixToPostfix(array_Infix);
                        //printPostfix();
                        postfixToTuple(ref array_postfix, ref temp);
                        array_Infix.Clear();
                        array_postfix.Clear();
                        current = getToken();
                    }
                    else
                    {
                        printSystaxError("A()", current.Word);
                    }
                }
                else
                {
                    printSystaxError("A()", current.Word);
                }
            }
            else
            {
                printSystaxError("A()", current.Word);
            }
        }

        private void W(string nameFunction)
        {
            string id = "";
            string Type = "";
            string para = "";
            string temp_ = "";

            flagesw = 1;
            array_Infix.Clear();

            if (current.Word == "switch")
            {
                current = getToken();
                if (current.Word == "(")
                {
                    current = getToken();
                    I(ref id, ref Type);

                    if (checekdVarDelare(Type, id) == false)
                    {
                        Console.WriteLine("===========Var Not Delare in Function===========");
                        Console.WriteLine("Func " + nameFunction + "() " + id);
                        Environment.Exit(0);
                    }
                    findTypeID(id, ref Type, "Local");
                    stackSW.Push(Type.ToString());
                    E1(ref para, id, Type, nameFunction, ref temp_);
                    if (current.Word == ")")
                    {

                        arraySW.AddRange(array_Infix);
                        infixToPostfix(array_Infix);
                        //printPostfix();
                        postfixToTuple(ref array_postfix, ref temp_);
                        array_Infix.Clear();
                        array_postfix.Clear();

                        current = getToken();
                        if (current.Word == "{")
                        {
                            current = getToken();
                            W1(nameFunction, temp_);

                            W3(nameFunction);
                            if (current.Word == "}")
                            {
                                current = getToken();
                                while (stacklabel.Count != 0)
                                {
                                    inter.Add(new Inter("LBL", "-", "-", stacklabel.Pop().ToString()));

                                }
                                if (stackSW.Count != 0)
                                {
                                    stackSW.Pop();
                                }
                            }
                            else
                            {
                                printSystaxError("W()", current.Word);
                            }
                        }
                        else
                        {
                            printSystaxError("W()", current.Word);
                        }
                    }
                    else
                    {
                        printSystaxError("W()", current.Word);
                    }

                }
                else
                {
                    printSystaxError("W()", current.Word);
                }
            }
            else
            {
                printSystaxError("W()", current.Word);
            }
        }

        private void W1(string nameFunction, string temp_)
        {
            ArrayList arrayTemp = new ArrayList();
            string para = "";
            string name = "";
            string opera = "";
            string id = "";
            string temp = "";
            string labelJMPF = "";
            string labelJMP = "";
            if (current.Word == "case")
            {
                current = getToken();
                array_Infix.Add(temp_);
                O1(ref opera);
                E(ref para, name, nameFunction, ref id);

                if (current.Word == ":")
                {

                    arrayTemp.AddRange(array_Infix);
                    infixToPostfix(arrayTemp);
                    //printPostfix();
                    postfixToTuple(ref array_postfix, ref temp);
                    array_Infix.Clear();
                    array_postfix.Clear();
                    current = getToken();

                    label = Label();
                    inter.Add(new Inter("JMPF", temp, "-", label));
                    labelJMPF = label;
                    S1(nameFunction);

                    flagesw = 1;
                    label = Label();
                    inter.Add(new Inter("JMP", temp, "-", label));
                    stacklabel.Push(label);
                    labelJMP = label;
                    inter.Add(new Inter("LBL", "-", "-", labelJMPF));
                    W2(nameFunction, temp_);

                }
                else
                {
                    printSystaxError("W1", current.Word);
                }
            }
            else
            {
                printSystaxError("W1", current.Word);
            }
        }

        private void W2(string nameFunction, string temp_)
        {

            if (current.Word == "case")
            {
                W1(nameFunction, temp_);
                W2(nameFunction, temp_);
            }
            else if (current.Word == "default" || current.Word == "}" || current.Word == ",")
            {

            }
            else
            {
                printSystaxError("W2()", current.Word);
            }


        }

        private void W3(string nameFunction)
        {
            if (current.Word == "default")
            {
                current = getToken();
                if (current.Word == ":")
                {
                    current = getToken();
                    S1(nameFunction);
                }
                else
                {
                    printSystaxError("W3()", current.Word);
                }

            }
            else if (current.Word == "}")
            {

            }
            else
            {
                printSystaxError("W3()", current.Word);
            }
        }

        private void T(ref string id, ref string para, string nameFunction)
        {
            if (current.Word == "int" || current.Word == "float" || current.Word == "string")
            {
                string id_temp = "";
                string Type_temp = "";

                T1(ref Type_temp);
                para = para + Type_temp;
                if (current.Type == "ID")
                {
                    id = current.Word;
                    chckeDuplicate(id, Type_temp, addrs, nameFunction);
                    current = getToken();
                    T2(id, Type_temp, ref para, nameFunction);

                    if (current.Word == ";")
                    {
                        current = getToken();

                        T(ref id_temp, ref para, nameFunction);
                    }
                    else
                    {
                        printSystaxError("T()", current.Word);
                    }
                }
                else
                {
                    printSystaxError("T()", current.Word);
                }
            }
            else if (current.Type == "ID" || current.Word == "switch" || current.Word == "call" || current.Word == ";" || current.Word == "}" || current.Word == "#" || current.Word == "Func" || current.Word == ")")
            {

            }
            else
            {
                printSystaxError("T()", current.Word);
            }
        }

        private void T1(ref string Type)
        {

            if (current.Word == "int" || current.Word == "float" || current.Word == "string")
            {
                Type = current.Word;
                current = getToken();
            }
            else
            {
                printSystaxError("T1()", current.Word);
            }
        }

        private void T2(string id, string Type, ref string para, string nameFunction)
        {

            if (current.Word == ",")
            {

                current = getToken();
                id = current.Word;
                if (current.Type == "ID")
                {
                    chckeDuplicate(id, Type, addrs, nameFunction);
                    para = para + Type;
                    current = getToken();
                    T2(id, Type, ref para, nameFunction);
                }
                else
                {
                    printSystaxError("T2()", current.Word);
                }
            }
            else if (current.Word == ";")
            {

            }
            else
            {
                printSystaxError("T2()", current.Word);
            }

        }

        private void T3(ref string para)
        {
            string Type = "";
            string id = "";
            string nameFunction = "";
            if (current.Word == "int" || current.Word == "float" || current.Word == "string")
            {
                T1(ref Type);
                para = Type;
                if (current.Type == "ID")
                {
                    id = current.Word;
                    chckeDuplicate(id, Type, addrs, nameFunction);
                    current = getToken();
                    T3(ref para);
                }
                else
                {
                    printSystaxError("T3()", current.Word);
                }
            }
            else if (current.Word == ",")
            {
                current = getToken();
                T1(ref Type);
                para = para + Type;
                if (current.Type == "ID")
                {
                    id = current.Word;
                    chckeDuplicate(id, Type, addrs, nameFunction);
                    current = getToken();
                    T3(ref para);
                }
                else
                {
                    printSystaxError("T3()", current.Word);
                }

            }
            else if (current.Word == ")")
            {

            }
            else
            {
                printSystaxError("T3()", current.Word);
            }
        }

        // Check name Duplicate in Local and Gobal
        // addrs = 1 is Gobal 
        // addrs = 0 
        private void chckeDuplicate(string id, string Type, int addrs, string nameFunction)
        {
            string nameScope = "";
            if (this.addrs == 1)
            {
                if (findDupilcate(id, "Gobal", ref nameScope))
                {
                    SemanticError("Var Duplicate: ", id, Type, "Gobal");
                    Environment.Exit(0);
                }
                else
                {
                    gobal.Add(new Var(id, Type, "Gobal"));
                }
            }
            else
            {
                if (nameFP == "F")
                {
                    if (findDupilcate(id, "P", ref nameScope))
                    {
                        Console.WriteLine("===========Var Duplicate in Function===========");
                        Console.WriteLine("Func " + nameFunction + "() " + id);
                        Environment.Exit(0);
                    }
                    if (findDupilcate(id, "Local", ref nameScope) == true)
                    {
                        //Console.WriteLine("===========Var Not Delare in Function===========");
                        Console.WriteLine("===========Var Duplicate in Function===========");
                        Console.WriteLine("Func " + nameFunction + "() " + id);
                        Environment.Exit(0);
                    }
                    else
                    {
                        local.Add(new Var(id, Type, sl));
                    }
                }
                else if (nameFP == "P")
                {
                    if (findDupilcate(id, "Local", ref nameScope))
                    {
                        Console.WriteLine("===========Var Duplicate in ProtoType===========");
                        Console.WriteLine(nameFunction + "(" + id + ")");
                        Environment.Exit(0);
                    }
                    else
                    {
                        local.Add(new Var(id, Type, sl));
                    }

                }
            }
        }

        //Check VarDelare in Local and Gobal
        // 
        private bool checekdVarDelare(string Type, string id)
        {
            if (Type == "")
            {
                for (int i = 0; i <= local.Count - 1; i++)
                {
                    if (id == local[i].getId_name())
                    {
                        return true;
                    }
                }

                for (int i = 0; i <= gobal.Count - 1; i++)
                {
                    if (id == gobal[i].getId_name())
                    {
                        return true;
                    }

                }
            }
            else
            {
                return true;
            }
            return false;
        }

        //Check Para Function ,Call 
        // Para in Function , para in Call 
        private bool checkFunction(string name, string para, string v, string nameFunction)
        {
            bool flagepara = false;
            bool flagepara_ = false;
            if (v == "f")
            {

                for (int i = 0; i <= pro.Count - 1; i++)
                {
                    if (name == pro[i].getName() && para == pro[i].getPara())
                    {
                        return true;
                    }
                    else if (name == pro[i].getName() && para != pro[i].getPara())
                    {
                        if (para != pro[i].getPara())
                        {
                            flagepara = false;
                        }
                        else
                        {
                            flagepara = true;
                        }
                        /*Console.WriteLine("===========Function Paramiter not equal:===========");
                        Console.WriteLine("Func: " + name + " (" + para + ") " + "");
                        Environment.Exit(0);
                        */
                    }
                }
                return false;
                /*
                if(flagepara==false)
                {
                    Console.WriteLine("===========Function Paramiter not equal:===========");
                    Console.WriteLine("Func: " + name + " (" + para + ") " + "");
                    Environment.Exit(0);
                }*/

            }
            else if (v == "c")
            {
                for (int i = 0; i <= pro.Count - 1; i++)
                {
                    if (name == pro[i].getName() && para == pro[i].getPara())
                    {
                        return true;
                    }
                    else if (name == pro[i].getName() && para != pro[i].getPara())
                    {
                        /*
                        if (para != pro[i].getPara())
                        {
                            flagepara_ = false;
                        }
                        else
                        {
                            flagepara_ = true;
                        }
                        */
                        /*Console.WriteLine("===========Call Paramiter not equal in Function:===========");
                        Console.WriteLine("Func "+ nameFunction +"() "+"Call: " + name + " (" + para + ") " + "");
                        Environment.Exit(0);
                        return false;*/
                    }
                }
            }

            /*
            if (flagepara_ == false)
            {
                Console.WriteLine("===========Function Paramiter not equal:===========");
                Console.WriteLine("Func: " + name + " (" + para + ") " + "");
                Environment.Exit(0);
            }
            */
            return false;
        }


        //Find name Delare,Var Dupilcate?
        //ProtoType Dupilcate var Gobla? , ProtoType Dupilcate 
        //Func Delare,Func Dupilcate
        //Call Delare in ProtoType? 
        //Var Delare,Var Dupilcate
        private bool findDupilcate(string Word, string v, ref string nameScope)
        {
            if (v == "Gobal")
            {
                foreach (var g in gobal)
                {
                    if (Word == g.getId_name())
                    {
                        nameScope = v;
                        return true;
                    }

                }
                return false;

            }
            else if (v == "Local" || v == "Local1")
            {
                foreach (var l in local)
                {
                    if (Word == l.getId_name())
                    {
                        nameScope = v;
                        return true;
                    }
                }
                return false;
            }
            else if (v == "P")
            {
                foreach (var f in pro)
                {
                    if (Word == f.getName())
                    {
                        nameScope = v;
                        return true;
                    }
                }
                return false;
            }
            /*
            else if (v == "P")
            {
                foreach (var f in pro)
                {
                    if (Word == f.getName())
                    {
                        nameScope = v;
                        return true;
                    }
                }
                return false;
            }*/
            else if (v == "F")
            {
                foreach (var f in func)
                {
                    if (Word == f.getName())
                    {
                        nameScope = v;
                        return true;
                    }
                }
                return false;
            }
            return false;
        }


        // Find name for get Type
        // find Var in Gobla next find Local
        // find Var in Local next find Gobal  
        private void findTypeID(string id, ref string Type, string nameScope)
        {
            bool falge = false;
            // id 
            if (Type == "")
            {
                if (nameScope == "Gobal")
                {
                    foreach (var i in gobal)
                    {
                        if (i.getId_name() == id)
                        {
                            Type = i.getType_name();
                            falge = true;
                            break;
                        }
                    }
                    if (falge == false)
                    {
                        foreach (var i in local)
                        {
                            if (i.getId_name() == id)
                            {
                                Type = i.getType_name();
                                break;

                            }
                        }

                    }
                }
                else if (nameScope == "Local")
                {
                    foreach (var i in local)
                    {
                        if (i.getId_name() == id)
                        {
                            Type = i.getType_name();
                            falge = true;
                            break;

                        }
                    }
                    if (falge == false)
                    {
                        foreach (var i in gobal)
                        {
                            if (i.getId_name() == id)
                            {
                                Type = i.getType_name();
                                break;

                            }
                        }
                    }
                }
            }
            else
            {
                if (Type == "int_const")
                {
                    Type = "int";
                }
                else if (Type == "float_const")
                {
                    Type = "float";
                }
                else if (Type == "string_const")
                {
                    Type = "string";
                }

            }
        }

        private bool chckeCallDup(string name)
        {
            for (int i = 0; i <= call.Count - 1; i++)
            {
                if (name == call[i].getName())
                {
                    return true;
                }
            }
            return false;
        }

        //removVar int Local0 ProtoType
        private void removeVar(string s)
        {
            for (int i = local.Count - 1; i >= 0; i--)
            {
                if (local[i].getStatus() == s)
                {
                    local.RemoveAt(i);
                }
            }
        }

        private void removeFunc(string nameFunction)
        {
            for (int i = func.Count - 1; i >= 0; i--)
            {
                if (nameFunction == func[i].getName())
                {
                    func.RemoveAt(i);
                }
            }
        }

        private void infixToPostfix(ArrayList arrInfix)
        {

            string postfix = "";
            string temp = "";
            foreach (string infix in arrInfix)
            {

                if (infix == "+" || infix == "-" || infix == "*" || infix == "/" || infix == "=" || infix == ">" || infix == "<" || infix == ">=" || infix == "<=")
                {
                    while (mystack.Count != 0 && priority(mystack.Peek().ToString()) <= priority(infix))
                    {
                        temp = mystack.Pop().ToString();
                        array_postfix.Add(temp);
                        postfix = postfix + temp;
                    }
                    mystack.Push(infix);

                }
                else
                {
                    postfix = postfix + infix;
                    array_postfix.Add(infix);
                }
            }
            while (mystack.Count != 0)
            {
                temp = mystack.Pop().ToString();
                postfix = postfix + temp;
                array_postfix.Add(temp);
            }
            mystack.Clear();
        }

        private void postfixToTuple(ref ArrayList array, ref string temp)
        {
            string L = "";
            string R = "";
            string opcode = "";
            string newtemp = "";

            foreach (string i in array)
            {
                if (i == "+" || i == "-" || i == "*" || i == "/" || i == ">" || i == "<" || i == "<=" || i == ">=" || i == "=")
                {
                    if (i == "=")
                    {
                        opcode = i;
                        L = mystack.Pop().ToString();
                        R = mystack.Pop().ToString();
                        addTuple(opcode, L, "-", R);
                        inter.Add(new Inter(opcode, L, "-", R));
                        //codegen.Add(new Codegen(opcode,"R",R) );


                    }
                    else
                    {

                        newtemp = Temp();
                        temp = newtemp;
                        opcode = i;
                        L = mystack.Pop().ToString();
                        R = mystack.Pop().ToString();
                        mystack.Push(newtemp);
                        addTuple(opcode, R, L, newtemp);
                        inter.Add(new Inter(opcode, R, L, temp));
                    }

                }
                else
                {
                    mystack.Push(i);
                    temp = i;
                }
            }
            mystack.Clear();
        }

        private void addTuple(string opcode, string L, string R, string temp)
        {
            array_Tuple.Add(opcode);
            array_Tuple.Add(L);
            array_Tuple.Add(R);
            array_Tuple.Add(temp);
        }

        private int priority(string i)
        {
            if (i == "*" || i == "/")
            {
                return 1;
            }
            else if (i == "+" || i == "-")
            {
                return 2;
            }
            else if (i == ">" || i == "<" || i == ">=" || i == "<=")
            {
                return 3;
            }
            else if (i == "=")
            {
                return 4;
            }
            return 0;
        }

        public void printTable()
        {
            Console.WriteLine("=================Gobal==========");
            Console.WriteLine("Name\t\t" + "Type");
            foreach (var g in gobal)
            {
                Console.WriteLine("" + g.getId_name() + "\t\t" + g.getType_name());
            }
            Console.WriteLine("=================================");

            Console.WriteLine("=================Lobal==========");
            Console.WriteLine("Name\t\t" + "Type");
            foreach (var l in local)
            {
                Console.WriteLine("" + l.getId_name() + "\t\t" + l.getType_name());
            }
            Console.WriteLine("=================================");
            Console.WriteLine("==============ProtoType==========");
            Console.WriteLine("Name\t\t");
            foreach (var p in pro)
            {
                Console.WriteLine("" + p.getName());
            }
            Console.WriteLine("=================================");

            Console.WriteLine("=================Function==========");
            Console.WriteLine("Name\t\t");
            foreach (var f in func)
            {
                Console.WriteLine("" + f.getName());
            }
            Console.WriteLine("=================================");

            Console.WriteLine("=================Call===========");
            Console.WriteLine("Name\t\t");
            foreach (var f in call)
            {
                Console.WriteLine("" + f.getName());
            }
            Console.WriteLine("=================================");
        }

        public void printPostfix()
        {
            string post = "";
            string inf = "";
            Console.WriteLine("================Intfix==================");
            foreach (var i in array_Infix)
            {
                inf = inf + i;
            }
            Console.WriteLine(inf);
            Console.WriteLine("=======================================");
            Console.WriteLine("================Postfix================");
            foreach (var i in array_postfix)
            {

                post = post + i;
            }
            Console.WriteLine(post);
            Console.WriteLine("=======================================");
        }

        private void printSystaxError(string st1, string st2)
        {
            syntax = 1;
            Console.WriteLine("Systax Error " + st1 + ": " + st2);
            Environment.Exit(0);

        }

        public void printTuple()
        {
            int count = 0;
            Console.WriteLine("================Tuple==================");
            foreach (string i in array_Tuple)
            {
                if (count == 4)
                {
                    Console.WriteLine();
                    Console.Write(i + "\t");
                    count = 1;
                }
                else
                {
                    Console.Write(i + "\t");
                    count++;
                }

            }
            Console.WriteLine();
            Console.WriteLine("=======================================");


        }

        private void SemanticError(string v, string id, string Type, string ad)
        {
            Console.WriteLine("===========" + v + "===========");
            Console.WriteLine("" + Type + " " + id + " " + ad);
        }

        public void printInter()
        {
            Console.WriteLine("================Intermediate============");
            foreach (var i in inter)
            {
                Console.WriteLine(i.getOP() + "\t" + i.getOpr1() + "\t" + i.getOpr2() + "\t" + i.getResult());// +"\t" +i.getOpr1()+"\t"+i.getOpr2+"\t"+i.getResult);
            }
            Console.WriteLine("=======================================");
        }

        class Function
        {
            string name;
            string para;

            public Function(string name, string para)
            {
                this.name = name;
                this.para = para;
            }

            public String getName()
            {
                return name;
            }
            public String getPara()
            {
                return para;
            }
        }

        class Var
        {
            string id_name;
            string Type_name;
            string status;


            public Var(string v1, string v2, string v3)
            {
                this.id_name = v1;
                this.Type_name = v2;
                this.status = v3;
            }


            public String getId_name()
            {
                return id_name;
            }
            public String getType_name()
            {
                return Type_name;
            }
            public String getStatus()
            {
                return status;
            }
        }
    }
}
class Inter
{

    string opcode;
    string opr1;
    string opr2;
    string result;
    public Inter(string opcode, string opr1, string opr2, string result)
    {

        switch (opcode)
        {
            case "=":
                this.opcode = "MOV";
                break;
            case "+":
                this.opcode = "ADD";
                break;
            case "-":
                this.opcode = "SUB";
                break;
            case "*":
                this.opcode = "MULT";
                break;
            case "/":
                this.opcode = "DIV";
                break;
            case ">":
                this.opcode = "CMPGT";
                break;
            case "<":
                this.opcode = "CMPLT";
                break;
            case ">=":
                this.opcode = "CMPGET";
                break;
            case "<=":
                this.opcode = "CMPLET";
                break;
            default:
                this.opcode = opcode;
                break;
        }
        this.opr1 = opr1;
        this.opr2 = opr2;
        this.result = result;
    }

    public string getOP()
    {
        return opcode;
    }
    public string getOpr1()
    {
        return opr1;
    }
    public string getOpr2()
    {
        return opr2;
    }
    public string getResult()
    {
        return result;
    }



}

class LabelTable
{
    public string label;
    public string value;

    public LabelTable(string label, string value)
    {
        this.label = label;
        this.value = value;
    }

    public String getLabel()
    {
        return label;
    }

    public String getValue()
    {
        return value;
    }

}

class Codegen
{
    private List<Inter> inter;
    private List<Codegen> codegen = new List<Codegen>();
    private List<LabelTable> lablel = new List<LabelTable>();
    string opcode;
    string opr1;
    string opr2;
    string result;
    int countT_ = 0;
    public String getAddrs(string label_)
    {

        for (int i = 0; i < lablel.Count; i++)
        {
            if (label_ == lablel[i].getLabel())
            {
                //sg = lablel[i].getValue() - 1; 
                return lablel[i].getValue();
            }

        }
        return "";
    }
    public Codegen(List<Inter> inter)
    {
        this.inter = inter;

        foreach (var i in inter)
        {
            if (i.getOP() == "LBL")
            {
                lablel.Add(new LabelTable(i.getResult(), "" + countT_));
            }
            else if (i.getOP() == "JMP")
            {
                countT_ = countT_ + 1;
            }
            else if (i.getOP() == "JMPF")
            {
                countT_ = countT_ + 2;
            }
            else
            {
                countT_ = countT_ + 3;
            }

        }


        foreach (var i in inter)
        {
            switch (i.getOP())
            {

                case "MOV":
                    this.opcode = "MOV";
                    codegen.Add(new Codegen("LOAD", "R", i.getOpr1(), ""));
                    codegen.Add(new Codegen("STORE", "R", i.getResult(), ""));
                    break;
                case "ADD":
                    this.opcode = "ADD";
                    codegen.Add(new Codegen("LOAD", "R", i.getOpr1(), ""));
                    codegen.Add(new Codegen("ADD", "R", i.getOpr2(), ""));
                    codegen.Add(new Codegen("STORE", "R", i.getResult(), ""));
                    break;
                case "SUB":
                    this.opcode = "SUB";
                    codegen.Add(new Codegen("LOAD", "R", i.getOpr1(), ""));
                    codegen.Add(new Codegen("SUB", "R", i.getOpr2(), ""));
                    codegen.Add(new Codegen("STORE", "R", i.getResult(), ""));
                    break;
                case "MULT":
                    this.opcode = "MULT";
                    codegen.Add(new Codegen("LOAD", "R", i.getOpr1(), ""));
                    codegen.Add(new Codegen("MUL", "R", i.getOpr2(), ""));
                    codegen.Add(new Codegen("STORE", "R", i.getResult(), ""));
                    break;
                case "DIV":
                    this.opcode = "DIV";
                    codegen.Add(new Codegen("LOAD", "R", i.getOpr1(), ""));
                    codegen.Add(new Codegen("DIV", "R", i.getOpr2(), ""));
                    codegen.Add(new Codegen("STORE", "R", i.getResult(), ""));
                    break;
                case "CMPGT":
                    this.opcode = "CMPGT";
                    codegen.Add(new Codegen("LOAD", "R", i.getOpr1(), ""));
                    codegen.Add(new Codegen("CMPGT", "R", i.getOpr2(), ""));
                    codegen.Add(new Codegen("STORE", "R", i.getResult(), ""));
                    break;
                case "CMPLT":
                    this.opcode = "CMPLT";
                    codegen.Add(new Codegen("LOAD", "R", i.getOpr1(), ""));
                    codegen.Add(new Codegen("CMPLT", "R", i.getOpr2(), ""));
                    codegen.Add(new Codegen("STORE", "R", i.getResult(), ""));
                    break;
                case "CMPGET":
                    this.opcode = "CMPGET";
                    codegen.Add(new Codegen("LOAD", "R", i.getOpr1(), ""));
                    codegen.Add(new Codegen("CMPGET", "R", i.getOpr2(), ""));
                    codegen.Add(new Codegen("STORE", "R", i.getResult(), ""));
                    break;
                case "CMPLET":
                    this.opcode = "CMPLET";
                    codegen.Add(new Codegen("LOAD", "R", i.getOpr1(), ""));
                    codegen.Add(new Codegen("CMPLET", "R", i.getOpr2(), ""));
                    codegen.Add(new Codegen("STORE", "R", i.getResult(), ""));
                    break;
                case "LBL":
                    break;
                    string sct;/*
                    sct = i.getResult().Replace("L","");
                    codegen.Add(new Codegen(sct, ":", "", ""));
                    break;*/
                case "JMPF":
                    codegen.Add(new Codegen("LOAD", "R", i.getOpr1(), ""));
                    /*codegen.Add(new Codegen("STORE", "R", i.getResult(), ""));*/
                    codegen.Add(new Codegen("JUMPF", getAddrs(i.getResult()), "", ""));
                    break;
                case "JMP":
                    /*codegen.Add(new Codegen("LOAD", "R", i.getOpr1(), ""));
                    codegen.Add(new Codegen("STORE", "R", i.getResult(), ""));*/
                    codegen.Add(new Codegen("JUMP", getAddrs(i.getResult()), "", ""));
                    break;
                default:
                    break;
            }

        }

    }


    public Codegen(string opcode, string opr1, string opr2, string result)
    {
        this.opcode = opcode;
        this.opr1 = opr1;
        this.opr2 = opr2;
        this.result = result;


    }
    int countI = 0;
    public string getOP()
    {
        return opcode;
    }
    public string getOpr1()
    {
        return opr1;
    }
    public string getOpr2()
    {
        return opr2;
    }
    public string getRe()
    {
        return result;
    }
    public void printCodegen()
    {
        foreach (var i in codegen)
        {
            Console.WriteLine("" + (countI++) + "\t" + i.getOP() + "\t" + i.getOpr1() + "\t" + i.getOpr2() + "\t" + i.getRe());
            //Console.WriteLine("" + i.getOP() + "\t" + i.getOpr1() + "\t" + i.getOpr2() + "\t" + i.getRe());
        }
        Console.WriteLine("" + (countI++) + "\t");
    }
    public void printLabe()
    {
        Console.WriteLine("========================");
        foreach (var i in lablel)
        {
            Console.WriteLine(i.getLabel() + "\t " + i.getValue());
        }
    }
}



    }
}
