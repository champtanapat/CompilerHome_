using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CompilerHome
{
    class Paser
    {
        private List<Token> list;
        private Token current;
        private int index = 0;

        //-- Var duplicate
        private int go_local = 1;
        private List<VarType> gobal_ = new List<VarType>();
        private List<VarType> local_ = new List<VarType>();
        private List<Function> func_  = new List<Function>();

        private string nameFunc = "";

        //ArrayList arrIn = new ArrayList(); 
        //ArrayList arrayPost = new ArrayList();

        //-- Infix to Postfix to 4Tuple
        ArrayList arrayTemp = new ArrayList();
        ArrayList arrayTuple = new ArrayList();
        ArrayList arrayPostfix_ = new ArrayList();
        private ArrayList arr = new ArrayList();
        List<Tuple> arrayTuple_ = new List<Tuple>();
        public List<Inter> inter = new List<Inter>();
        //public List<Codegen> arrayGen = new List<Codegen>();

        public ArrayList arrP = new ArrayList();


        private int countLabel = 0;
        private string label;
        private int counttemp = 0;
        Stack<string> mystack = new Stack<string>();
        //-- end 
        
        
        private List<VarType> arrNew = new List<VarType>();



        public Paser(List<Token> list)
        {
            this.list = list;
        }
        
        public Token getToken()
        {
            return list[index++];
        }

        public void S()
        {
            string type = "";
            string id = "";
            current = getToken();
            if (current.Word == "BEGIN_PROGRAM")
            {
                current = getToken();
                go_local = 1;
                T(ref type, ref id);
                F();
                if(current.Word == "END_PROGRAM")
                {
                    current = getToken();
                    if (current.Word == ";")
                    {
                        current = getToken();
                        if (current.Word == "$")
                        {
                            Console.WriteLine("Syntax True");
                        }
                        else
                        {
                            Syntax("S()", current.Word);
                        }
                    }
                    else
                    {
                        Syntax("S()", current.Word);
                    }
                }
                else
                {
                    Syntax("S()", current.Word);
                }

            }
            else if (current.Word == "$")
            {
                Console.WriteLine("Syntax True");
            }
            else
            {
                Syntax("S()", current.Word);
            }
            /*
            if (current.Word == "$" && syntax == 0)
            {
                Console.WriteLine("Syntax True");
            }*/
        }

        private void T(ref string type, ref string id)
        {


            if (current.Word == "int" || current.Word == "float" || current.Word == "string")
            {
                string type_temp = "";
                string id_temp = "";

                A(ref type_temp);
                type = type_temp;
                id = current.Word;
                ID();
                if (go_local == 1)
                {
                    if (findDuplicate(type, id, "Global"))
                    {
                        gobal_.Add(new VarType(type,id, "Global"));
                    }
                    else
                    {
                        Console.WriteLine("Var Global is duplicate: " + type + "\t" + id);
                        Environment.Exit(0);
                    }
                }
                else
                {
                    if (findDuplicate(type, id, "Local"))
                    {
                        local_.Add(new VarType(type, id, "Local"));
                    }
                    else
                    {
                        SemanticDeclare(id);
                        Environment.Exit(0);
                    }
                }

                B(type_temp);

                if (current.Word == ";")
                {

                    current = getToken();
                    T(ref type, ref id);
                }
                else
                {
                    Syntax("T()", current.Word);
                }
            }
            else if (current.Word == "Func" || current.Word == "END_PROGRAM" || current.Word == "call" || current.Type == "ID" || current.Word == "while"
                || current.Word == "end" || current.Word == "}"|| current.Word == "if" )
            {

            }
            else
            {
                Syntax("T()", current.Word);

            }
        }

        private void A(ref string temp_type)
        {

            if (current.Word == "int" || current.Word == "float" || current.Word == "string" )
            {
                temp_type = current.Word;
                current = getToken();
            }
            else
            {
                Syntax("A()", current.Word);
            }
        }

        private void B(string type_temp)
        {
           
            string id = "";
            string type = "";

            if (current.Word == ",")
            {
                current = getToken();
                id = current.Word;
                ID();

                
                type = type_temp;

                if (go_local == 1)
                {
                    if (findDuplicate(type, id, "Global"))
                    {
                        gobal_.Add(new VarType(type,id, "Global"));
                    }
                    else
                    {
                        Console.WriteLine("Var Global is duplicate: " + type + "\t" + id);
                        Environment.Exit(0);
                    }
                }
                else
                {
                    if (findDuplicate(type, id, "Local"))
                    {
                        local_.Add(new VarType(type, id, "Local"));
                    }
                    else
                    {
                        Semantic(nameFunc, type, id);
                        Environment.Exit(0);
                    }
                }
                B(type_temp);
            }
            else if (current.Word == ";")
            {
                
            }
            else
            {

                Syntax("B()", current.Word);
            }
        }

        private void F()
        {

            string id = "";
            string type = "";
            nameFunc = "";
            string para = "";
            if (current.Word == "Func")
            {
                string id_temp = "";
                string type_temp = "";
                current = getToken();
                id = current.Word;
                ID();
                if (!findDuplicate(type, id, "Func"))
                {
                    Console.WriteLine("Func "+id + " var is duplicate: ");
                    Environment.Exit(0);
                }
                nameFunc = id; 

                if (current.Word == "(")
                {
                    current = getToken();
                    T1(ref type,nameFunc,ref para);
                    if (current.Word == ")")
                    {
                        current = getToken();
                        if (findPara(para,id))
                        {
                            func_.Add(new Function(id, para));
                        }
                        else
                        {
                            
                            Semantic(nameFunc);
                            Environment.Exit(0);
                        }
                        if (current.Word == "begin")
                        {
                            current = getToken();
                            go_local = 0;
                            T(ref type_temp, ref id_temp);
                            type = type_temp;
                            id = id_temp;
                            C(type, id);
                            if (current.Word == "end")
                            {
                                current = getToken();
                                if (current.Word == ";")
                                {
                                    current = getToken();
                                    removeVar();
                                    F();
                                }
                                else
                                {
                                    Syntax("F()", current.Word);
                                }
                            }
                            else
                            {
                                Syntax("F()", current.Word);
                            }
                        }
                        else
                        {
                            Syntax("F()", current.Word);
                        }

                    }
                    else
                    {
                        Syntax("F()", current.Word);
                    }
                } //"("
                else
                {
                    Syntax("F()", current.Word);
                }
            }
            else if (current.Word == "$" || current.Word == "END_PROGRAM")
            {

            }
            else
            {
                Syntax("F()", current.Word);
            }
        }
        
        
        private void C(string type, string id)
        {
            string namecall = "";
            if (current.Word == "call")
            {
                current = getToken();
                id = current.Word;
                namecall = id; 
                ID();
                if (current.Word == "(")
                {
                    current = getToken();
                    P();
                    if (current.Word == ")")
                    {
                        current = getToken();
                        if (current.Word == ";")
                        {
                            current = getToken();
                            C(type, id);
                        }
                        else
                        {
                            Syntax("C()", current.Word);
                        }
                    }
                    else
                    {
                        Syntax("C()", current.Word);
                    }
                }
                else
                {
                    Syntax("C()", current.Word);
                }

            }
            else if (current.Type == "ID")
            {

                D();
                C(type, id);

            }
            else if (current.Word == "while")
            {
                W();
                C(type, id);
            }
            else if (current.Word == "if")
            {
                G();
                C(type, id);

            }
            else if (current.Word == "end" || current.Word == "}")
            {

            }
            else
            {
                Syntax("C()", current.Word);
            }
        }
        
        private void ID()
        {
            string id= "";
            if (current.Type == "ID")
            {
                id = current.Word;
                current = getToken();
            }
            else
            {
                Syntax("ID()", current.Word);
            }
        }

        private void D()
        {
            arrayTemp.Clear();
            arrayPostfix_.Clear();
            string id = "";
            string id_temp = "";
            string type = "";

            string name_ = "";
            string type_ = "";
            if (current.Type == "ID")
            {
                
                name_ = current.Word;
                arrayTemp.Add(name_);
                current = getToken();
                type_ = getVarType(name_);

                if (!checkVar_Declare(name_))
                {
                    SemanticDeclare(name_);
                    Environment.Exit(0);
                }
                
                if (current.Word == "=")
                {
                    arrayTemp.Add(current.Word);
                    current = getToken();
                    id = current.Word;
                    I(ref id, ref type,name_,type_);
                    if (type != type_)
                    {
                        Semantic(id, type);
                        Environment.Exit(0);
                    }
                    //id_temp = id; 
                    name_ = id;
                    type_ = type;
                    E(ref id_temp,type ,name_,type_);
                    id = id_temp;

                    infix_Postfix(arrayTemp);
                    printIn_Postfix();
                    string temp_ = "";
                    postfix_Tuple(arrayPostfix_, ref temp_);

                    if (current.Word == ";")
                    {
                        current = getToken();
                    }
                    else
                    {
                        Syntax("D()", current.Word);
                    }
                }
                else
                {
                    Syntax("D()", current.Word);
                }

            }
            else
            {
                Syntax("D()", current.Word);
            }
        }

        private string getVarType(string id)
        {
            foreach (var local in local_)
            {
                if (id == local.Name_var)
                {
                    return local.Type_var;
                }
            }

            foreach (var gobal in gobal_)
            {
                if (id == gobal.Name_var)
                {
                    return gobal.Type_var;
                }
            }

            
            return "";
        }

        private void SemanticDeclare(string id)
        {
            Console.WriteLine("==================( SemanticError )==================");
            Console.WriteLine("Func " + nameFunc + " var is not Declare : " + id);
            Console.WriteLine("=====================================================");
        }

        private void E(ref string id,string type,string name_,string type_)
        {
            
            if (current.Word == "*" || current.Word == "/" || current.Word == "-" || current.Word == "+" || current.Word == "not"
                || current.Word == "and" || current.Word == "or" || current.Word == ">" || current.Word == ">=" || current.Word == "<" || current.Word == ">" || current.Word == "<=")
            {
                if (current.Word == "and")
                {
                    current.Word = "&&";
                    arrayTemp.Add("&&");
                    current = getToken();
                }
                else if (current.Word == "or")
                {
                    current.Word = "||";
                    arrayTemp.Add("||");
                    current = getToken();
                }
                else if (current.Word == "not")
                {
                    current.Word = "||";
                    arrayTemp.Add("||");
                    current = getToken();

                }
                else
                {
                    if (type == "string")
                    {
                        if (current.Word == "+" || current.Word == "-")
                        {

                        }
                        else
                        {
                            
                            SemanticString(current.Word, type);
                            Environment.Exit(0);
                        }
                    }
                    arrayTemp.Add(current.Word);
                    current = getToken();
                }
                
                I(ref id, ref type, name_, type_);
                E(ref id,type,name_,type_);
            }
            else if (current.Word == ";" || current.Word == ")")
            {
                
            }
            else
            {
                Syntax("E()", current.Word);
            }

        }


        private void I(ref string id, ref string type,string name_ , string type_)
        {
            
            if (current.Type == "ID")
            {
                id = current.Word;
                arrayTemp.Add(current.Word);
                ID();
                if (!checkVar_Declare(id))
                {
                    SemanticDeclare(id);
                    Environment.Exit(0);
                }
                type = getVarType(id);
                if (type != type_ && type_!= "")
                {
                    Semantic(id, type);
                    Environment.Exit(0);
                }
            }
            else if (current.Type == "int_const" || current.Type == "float_const" || current.Type == "string_const")
            {

                arrayTemp.Add(current.Word);
                id = current.Word;
                if (current.Type == "int_const")
                {
                    type = "int";
                }
                else if(current.Type == "float_const")
                {
                    type = "float";
                }
                else if(current.Type == "string_const")
                {
                    type = "string";
                }
                if (type != type_ && type_ != "")
                {
                    Semantic(id, type);
                    Environment.Exit(0);
                }
                current = getToken();
            }
            else
            {
                Syntax("I()", current.Word);
            }
        }

       

        private void W()
        {
            
            arrayTemp.Clear();
            arrayPostfix_.Clear();
            string temp_ = "";
            string tempLabel_1 = "";
            string tempLabel_2 = "";

            string id = "";
            string type = "";
            string name_ = "";
            string type_ = "";
            if (current.Word == "while")
            {
                current = getToken();
                if (current.Word == "(")
                {
                    current = getToken();
                    I(ref id, ref type,name_,type_);
                    name_ = id;
                    type_ = type;
                    E(ref id, type,name_, type_);

                    infix_Postfix(arrayTemp);
                    printIn_Postfix();

                    label = newlabel();
                    inter.Add(new Inter("LBL", "-", "-", label));
                    tempLabel_1 = label;
                    postfix_Tuple(arrayPostfix_, ref temp_);

                    label = newlabel();
                    inter.Add(new Inter("JMPF", temp_, "-", label));
                    tempLabel_2 = label;
                    current = getToken();
                    if (current.Word == "{")
                    {
                        current = getToken();
                        C(type, id);
                        label = newlabel();

                        if (current.Word == "}")
                        {
                            current = getToken();
                            inter.Add(new Inter("JMP", "-", "-", tempLabel_1));
                           inter.Add(new Inter("LBL", "-", "-", tempLabel_2));
                        }
                        else
                        {
                            Syntax("W()", current.Word);
                        }
                    }
                    else
                    {
                        Syntax("W()", current.Word);
                    }

                }
                else
                {
                    Syntax("W()", current.Word);
                }

            }
            else if(current.Word == "call" || current.Type == "ID" || current.Word == "while" || current.Word == "if" || current.Word == "end" || current.Word == "}")
            {

            }
            else
            {
                Syntax("W()", current.Word);
            }
        }


        private void G()
        {
            string type = "";
            string id = "";
            string name_ = "";
            string type_ = "";
            if (current.Word == "if")
            {
                current = getToken();
                if (current.Word == "(")
                {
                    current = getToken();
                    I(ref id, ref type,name_,type_);
                    name_ = id;
                    type_ = type;
                    E(ref id,type,name_, type_);
                    if (current.Word == ")")
                    {
                        current = getToken();
                        if (current.Word == "{")
                        {
                            current = getToken();
                            C(type, id);
                            if (current.Word == "}")
                            {
                                current = getToken();
                                G1();
                                C(type,id);
                            }
                            else
                            {
                                Syntax("G()", current.Word);
                            }
                        }
                        else
                        {
                            Syntax("G()", current.Word);
                        }

                    }
                    else
                    {
                        Syntax("G()", current.Word);
                    }
                }
                else
                {
                    Syntax("G()", current.Word);
                }

            }
            else if (current.Word == "}")
            {

            }
            else
            {
                Syntax("G()", current.Word);
            }
        }

        private void G1()
        {
            string type = "";
            string id = "";
            if (current.Word == "else")
            {
                current = getToken();
                if (current.Word == "{")
                {
                    current = getToken();
                    C(type, id);
                    if (current.Word == "}")
                    {
                        current = getToken();
                    }
                    else
                    {
                        Syntax("G1()", current.Word);
                    }
                }
                else
                {
                    Syntax("G1()", current.Word);
                }
            }
            else if (current.Word == "end" || current.Word == "}")
            {

            }
            else
            {
                Syntax("G1()", current.Word);
            }
        }

        private void P()
        {
            string id = "", type = "";
            string name_ = "", type_ = "";
            if (current.Type == "ID" || current.Type == "int_const" || current.Type == "float_const" || current.Type == "string_const")
            {
                I(ref id,ref type,name_,type_);
                P1();
            }
            else if(current.Word == ")")
            {

            }
            else
            {
                Syntax("P1()", current.Word);
            }
            
        }

        private void P1()
        {
            string type = "", id = "";
            string name_ = "", type_ = "";
            if(current.Word == "*" || current.Word == "/" || current.Word == "-" || current.Word == "+")
            {
                O();
                I(ref id,ref type,name_,type_);
                P1();
            }
            else if(current.Word == ",")
            {
                current = getToken();
                I(ref id, ref type, name_, type_);
                P1();
            }
            else if(current.Word==")")
            {

            }
            else
            {
                Syntax("P1()", current.Word);
            }
        }

        private void O()
        {
            if(current.Word=="*"||current.Word=="/" ||current.Word == "-" || current.Word == "+")
            {
                current = getToken();
            }
            else
            {
                Syntax("O()", current.Word);
            }
        }

        private void T1(ref string type,string nameFunc,ref string para)
        {

           string id = "";
           string type_temp = "";
           if(current.Word=="int" || current.Word=="float" || current.Word =="string")
            {
                A(ref type_temp);
                type = type_temp;
                id = current.Word;
                ID();
                if(findDuplicate(type,id,"Local"))
                {
                    local_.Add(new VarType(type,id,"Local"));
                }
                else
                {
                    Console.WriteLine("Func " + nameFunc + "\t");
                    Console.WriteLine("Parameter is duplicate: " + type + " " + id);
                    Environment.Exit(0);
                }
                para = para + type;
                T1(ref type, nameFunc, ref para);
                
            }
           else if(current.Word==",")
            {
                current = getToken();
                A(ref type_temp);
                type = type_temp;
                id = current.Word;
                ID();
                if (findDuplicate(type, id, "Local"))
                {
                    local_.Add(new VarType(type, id, "Local"));
                }
                else
                {
                    Console.WriteLine("Func "+ nameFunc+"\t");
                    Console.WriteLine("Parameter is duplicate: " + type + " " + id);
                    Environment.Exit(0);
                }
                para = para + type;
                T1(ref type, nameFunc, ref para);
            }
           else if(current.Word==")")
            {
                
            }
           else
            {
                Syntax("T1()", current.Word);
            }
        }


        private void Semantic(string nameFunc)
        {
            Console.WriteLine("==================( SemanticError )==================");
            Console.WriteLine("Func " + nameFunc + " parameter is duplicate : ");
            Console.WriteLine("=====================================================");
        }

        private void Semantic(string nameFunc, string type, string id)
        {
            Console.WriteLine("==================( SemanticError )===================");
            Console.WriteLine("Func " + nameFunc + " var is  duplicate : " + type + " " + id);
            Console.WriteLine("=====================================================");
        }

        private void Semantic(string id, string type)
        {
            Console.WriteLine("==================( SemanticError )===================");
            Console.WriteLine("Func " + nameFunc + " var type is  not equal : " + type + " " + id);
            Console.WriteLine("=====================================================");
        }

        private void SemanticString(string id, string type)
        {
            Console.WriteLine("==================( SemanticError )===================");
            Console.WriteLine("Func " + nameFunc + " var String Operation : " + id + " " + type);
            Console.WriteLine("=====================================================");
        }
        private void removeVar()
        {
            for (int i = local_.Count - 1; i >= 0; i--)
            {
                local_.RemoveAt(i);
            }

        }

        private bool findPara(string para, string id)
        {

            foreach (var func in func_)
            {
                if (id == func.Name_func && para == func.Para_func)
                {

                    return false;
                }
            }
            return true;

        }

        //-- Infix To Postfix 
        public void infix_Postfix(ArrayList array_Infix)
        {
            string postfix = "";
            string s_infix = "";
            array_Infix = arrayTemp;
            postfix = "";
            s_infix = "";

            string temp_ = "";
            foreach (string infix in array_Infix)
            {

                if (infix == "||" || infix == "&&" || infix == "+" || infix == "-" || infix == "*" || infix == "/" || infix == ">" || infix == "<" || infix == "<=" || infix == ">=" || infix == "=")
                {

                    while (mystack.Count != 0 && priority(mystack.Peek().ToString()) <= priority(infix))
                    {
                        postfix = postfix + mystack.Peek();
                        arrayPostfix_.Add(mystack.Peek());
                        mystack.Pop();
                    }
                    mystack.Push(infix);
                }
                else
                {
                    s_infix = s_infix + infix;
                    postfix = postfix + infix;
                    arrayPostfix_.Add(infix);
                }
            }// end loop for  
            while (mystack.Count != 0)
            {
                temp_ = mystack.Pop();
                postfix = postfix + temp_;
                arrayPostfix_.Add(temp_);
            }

        }

        //-- Postfxi To 4Tuple
        public void postfix_Tuple(ArrayList arrayTree, ref string temp_)
        {
            string opcode = "";
            string R = "";
            string L = "";
            string temp = "";
            string infix = "";
            foreach (string i in arrayTree)
            {
                infix = infix + i;
                if (i == "||" || i == "&&" || i == "+" || i == "-" || i == "*" || i == "/" || i == ">" || i == "<" || i == "<=" || i == ">=" || i == "=")
                {
                    if (i == "=")
                    {
                        opcode = "=";
                        R = mystack.Pop();
                        //L = "-";
                        L = mystack.Pop();
                        addListTuple(opcode, R, "_", L);
                        inter.Add(new Inter(opcode, R, "-", L));


                    }
                    else
                    {
                        opcode = i;
                        R = mystack.Pop();
                        L = mystack.Pop();
                        temp = tempT();
                        addListTuple(opcode, L, R, temp);
                        inter.Add(new Inter(opcode, L, R, temp));
                        mystack.Push(temp);
                        temp_ = temp;

                    }
                }

                else
                {
                    mystack.Push(i);
                }

            }// end loop for  

            mystack.Clear();
        }


        //-- new USE
        public void printIn_Postfix()
        {
            string postfix = "";
            string infix_ = "";
            Console.WriteLine("=================( Infix )==============");
            //arrayInfix
            foreach (string infix in arrayTemp)
            {
                infix_ = infix_ + infix;
            }
            Console.WriteLine("infix : " + infix_);
            Console.WriteLine("========================================");



            Console.WriteLine("=================( Postfix )============");

            foreach (string i in arrayPostfix_)
            {
                postfix = postfix + i;

            }//end loop
            Console.WriteLine("postfix: " + postfix);
            Console.WriteLine("========================================");
        }

        //-- new print tuple
        public void printTupleNew()
        {
            Console.WriteLine("=================( Tuple )==============");
            foreach (var i in arrayTuple_)
            {
                Console.WriteLine(i.op + "\t" + i.opr1 + "\t" + i.opr2 + "\t" + i.result);
            }

            Console.WriteLine("========================================");
            Console.WriteLine();

        }

        public void printTuple()
        {
            int count = 1;
            Console.WriteLine("=================( Tuple )==============");
            foreach (string i in arrayTuple)
            {

                if (count == 5)
                {
                    if (i == ";" || i == ")")
                    {
                        count = 1;
                        Console.WriteLine();
                        Console.WriteLine("========================================");

                    }
                    else
                    {
                        Console.WriteLine();
                        Console.Write("" + i + "\t");
                        count = 1;
                        count++;
                    }
                }
                else
                {
                    Console.Write("" + i + "\t");
                    count++;
                }

            }// end loop 
            Console.WriteLine();
        }

        public int priority(string ch)
        {
            int temp = 0;
            if (ch == "*" || ch == "/")
            {
                temp = 1;
            }
            else if (ch == "+" || ch == "-")
            {
                temp = 2;
            }
            else if (ch == ">" || ch == "<" || ch == "<=" || ch == ">=")
            {
                temp = 3;
            }
            else if (ch == "=")
            {
                temp = 5;
            }
            else if (ch == "&&" || ch == "&")
            {
                temp = 4;
            }
            else if (ch == "||" || ch == "|")
            {
                temp = 6;
            }
            return temp;
        }

        private string tempT()
        {
            counttemp++;
            return "T" + counttemp;
        }

        public string newlabel()
        {
            countLabel++;
            return "L" + countLabel;
        }

        public void addListTuple(string opcode, string L, string R, string temp)
        {
            arrayTuple.Add(opcode);
            arrayTuple.Add(L);
            arrayTuple.Add(R);
            arrayTuple.Add(temp);
            arrayTuple_.Add(new Tuple(opcode, L, R, temp));
        }

        //-- Print
        private void Syntax(string terminal, string word)
        {
            
            Console.WriteLine("Syntax error at Non Terminal \t" + terminal + ":\t" + word);
        }


        public void printDuplicat()
        {
            Console.WriteLine("==========Global==============");
            Console.WriteLine("Type\t\t" + "ID");
            foreach (var gobal in gobal_)
            {
                //Console.WriteLine(gobal.getTypeVar() + "\t\t" + gobal.Name_var);
            }
            Console.WriteLine("=============================");
            Console.WriteLine("==========Local==============");
            Console.WriteLine("Type\t\t" + "ID");
            foreach (var local in local_)
            {
                //Console.WriteLine(local.getTypeVar() + "\t\t" + local.Name_var);
            }
            Console.WriteLine("=============================");
            
        }


        public bool findDuplicate(string type_find, string id_find, string addr)
        {

            if (addr == "Global")
            {
                foreach (var gobal in gobal_)
                {
                    if (id_find == gobal.Name_var)
                    {

                        
                        return false;
                    }
                }
                return true;

            }
            else if (addr == "Local" || addr == "Local1")
            {
                foreach (var local in local_)
                {
                    if (id_find == local.Name_var)
                    {
                        return false;
                    }
                }
                foreach (var func in func_)
                {
                    if (id_find == func.Name_func)
                    {
                        return false;
                    }
                }
                return true;
            }
            else if(addr == "Func")
            {
                foreach (var gobal in gobal_)
                {
                    if (id_find == gobal.Name_var)
                    {
                        return false;
                    }
                }

                foreach (var local in local_)
                {
                    if (id_find == local.Name_var)
                    {
                        return false;
                    }
                }
                /*
                foreach (var func in func_)
                {
                    if (id_find == func.Name_func)
                    {
                        return false;
                    }
                }
                return true;
                */
            }
            return true;
        }

        public void printInter()
        {
            Console.WriteLine("===========( Intermediate) =============");
            foreach (var a in inter)
            {
                Console.WriteLine("" + a.op + "\t" + a.opr1 + "\t" + a.opr2 + "\t" + a.result);
            }
            Console.WriteLine("========================================");
        }

        public void printshow(string stword)
        {
            Console.WriteLine("" + stword);
        }

        public bool checkVar_Declare(string s)
        {
            foreach (var gobal in gobal_)
            {
                if (s == gobal.Name_var)
                {
                    return true;
                }
            }
            foreach (var local in local_)
            {
                if (s == local.Name_var)
                {
                    return true;
                }
            }
            return false;
        }

        class Tuple
        {
            public string op;
            public string opr1;
            public string opr2;
            public string result;
            public Tuple(string op, string opr1, string opr2, string result)
            {
                this.op = op;
                this.opr1 = opr1;
                this.opr2 = opr2;
                this.result = result;
            }

        }

        class Function
        {
            string name;
            string para;

            public Function(string name_, string para_)
            {
                Name_func = name_;
                Para_func = para_;
            }

            public string Name_func
            {
                get
                {
                    return name;
                }

                set
                {
                    name = value;
                }
            }

            public string Para_func
            {
                get
                {
                    return para;
                }

                set
                {
                    para = value;
                }
            }
            
        }

        class VarType
        {
            private string name_var;
            private string type_var;
            private string addr;

            public VarType(string type_temp, string id_temp, string local_temp)
            {
                this.type_var = type_temp;
                this.name_var = id_temp;
                this.addr = local_temp;
            }
            public string Name_var
            {
                get
                {
                    return name_var;
                }

                set
                {
                    name_var = value;
                }
            }

            public string Type_var
            {
                get
                {
                    return type_var;
                }

                set
                {
                    type_var = value;
                }
            }
            

            public String getStatus()
            {
                return addr;
            }

            
        }
    }


    class Inter
    {
        public string op;
        public string opr1;
        public string opr2;
        public string result;

        public string getOP()
        {
            return op;
        }
        public Inter(string op, string opr1, string opr2, string result)
        {
            this.op = op;
            this.opr1 = opr1;
            this.opr2 = opr2;
            this.result = result;

            switch (op)
            {
                case "=":
                    this.op = "MOV";

                    break;
                case "+":
                    this.op = "ADD";
                    break;
                case "-":
                    this.op = "SUB";
                    break;
                case "*":
                    this.op = "MULT";
                    break;
                case "/":
                    this.op = "DIV";
                    break;
                case "||":
                    this.op = "OR";
                    break;
                case "&&":
                    this.op = "AND";
                    break;
                case ">":
                    this.op = "CMPGT";
                    break;
                case "<":
                    this.op = "CMPLT";
                    break;

            }
        }
    }
    
    /*
    class Codegen
    {
        public string opr1;
        public string opr2;
        //public string result;
        private List<Inter> list_inter;
        private List<Codegen> list_code = new List<Codegen>();


        public Codegen(List<Inter> inter)
        {
            this.list_inter = inter;
        }

        public Codegen(string opr1, string opr2, string result)
        {
            // this.op = op;
            this.opr1 = opr1;
            this.opr2 = opr2;
            //this.result = result;
        }

        public Codegen(string opr1, string opr2)
        {
            this.opr1 = opr1;
            this.opr2 = opr2;
        }
        public void input_Codegen()
        {
            int i = 0;
            while (i < list_inter.Count)
            {
                switch (list_inter[i].op)
                {
                    case "MOV":
                        list_code.Add(new Codegen("LOAD  R", list_inter[i].opr1));
                        list_code.Add(new Codegen("STORE R", list_inter[i].result));
                        break;
                    case "ADD":
                        list_code.Add(new Codegen("LOAD  R", list_inter[i].opr1));
                        list_code.Add(new Codegen("ADD   R", list_inter[i].opr2));
                        list_code.Add(new Codegen("STORE R", list_inter[i].result));

                        break;
                    case "SUB":

                        list_code.Add(new Codegen("LOAD  R", list_inter[i].opr1));
                        list_code.Add(new Codegen("SUB   R", list_inter[i].opr2));
                        list_code.Add(new Codegen("STORE R", list_inter[i].result));

                        break;
                    case "MULT":
                        list_code.Add(new Codegen("LOAD  R", list_inter[i].opr1));
                        list_code.Add(new Codegen("MULT   R", list_inter[i].opr2));
                        list_code.Add(new Codegen("STORE R", list_inter[i].result));
                        break;

                    case "DIV":
                        list_code.Add(new Codegen("LOAD  R", list_inter[i].opr1));
                        list_code.Add(new Codegen("DIV   R", list_inter[i].opr2));
                        list_code.Add(new Codegen("STORE R", list_inter[i].result));
                        break;

                    case "OR":
                        list_code.Add(new Codegen("LOAD  R", list_inter[i].opr1));
                        list_code.Add(new Codegen("OR   R", list_inter[i].opr2));
                        list_code.Add(new Codegen("STORE R", list_inter[i].result));
                        break;

                    case "AND":
                        list_code.Add(new Codegen("LOAD  R", list_inter[i].opr1));
                        list_code.Add(new Codegen("AND   R", list_inter[i].opr2));
                        list_code.Add(new Codegen("STORE R", list_inter[i].result));
                        break;

                    case "CMPGT":
                        list_code.Add(new Codegen("LOAD  R", list_inter[i].opr1));
                        list_code.Add(new Codegen("CMPGT R", list_inter[i].opr2));
                        list_code.Add(new Codegen("STORE R", list_inter[i].result));

                        break;
                    case "CMPLT":
                        list_code.Add(new Codegen("LOAD  R", list_inter[i].opr1));
                        list_code.Add(new Codegen("CMPLT R", list_inter[i].opr2));
                        list_code.Add(new Codegen("STORE R", list_inter[i].result));
                        break;

                    case "LBL":
                        list_code.Add(new Codegen(list_inter[i].result, ":"));
                        break;
                    case "JMP":
                        list_code.Add(new Codegen("JMP", list_inter[i].result));
                        break;
                    case "JMPF":
                        list_code.Add(new Codegen("LOAD  R", list_inter[i].opr1));
                        list_code.Add(new Codegen("STORE R", list_inter[i].result));

                        break;
                }// end switch();
                i++;
            }
        }

        public void printCodegen()
        {
            Console.WriteLine("===========( Code gen) =================");
            foreach (var i in list_code)
            {
                Console.WriteLine(i.opr1 + "\t    " + i.opr2 + "\t");
            }

            Console.WriteLine("========================================");
        }
        */
    

}
