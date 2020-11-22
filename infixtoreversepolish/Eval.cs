using System;
using System.Collections.Generic;
using System.Text;

namespace infixtoreversepolish
{

    public class EvalResult
    {
        public string Input {get; set;}
        public string InputAsRpn {get; set;}
        public int ResultValue {get; set;}
        public string Result {get; set;}
    }

    public class Eval
    {
        string input = "";
        Stack<Token> tokenStack = new Stack<Token>();
        Queue<Token> tokenQueue = new Queue<Token>();
        Token currentToken = null;
        string currentSeq = "";
        TokenTypes currentType = TokenTypes.Null;
        Precedence stackTopPrecedence = Precedence.None;
        List<Token> rpnList = new List<Token>();
        Stack<Token> rpnStack = new Stack<Token>();
        Tokenizer tokenizer = new Tokenizer();

        public EvalResult Evaluate(string input)
        {
            EvalResult res = new EvalResult();

            this.input = input;
            Clear();

            tokenQueue = tokenizer.Tokenize(input);

            // 2. Fronta tokenů -> RPN list
            while (tokenQueue.Count > 0)
            {
                Token t = tokenQueue.Dequeue();

                if (t.Type == TokenTypes.Number)
                {
                    rpnList.Add(t);
                }
                else if (t.Type == TokenTypes.Operator)
                {
                    if (tokenStack.Count == 0 || (t.Type == TokenTypes.Operator && t.Precedence == Precedence.BracketStart))
                    {
                        if (t.Precedence >= Precedence.BracketStart)
                        {
                            tokenStack.Push(t);
                        }
                    }
                    else
                    {
                        Token peeked = tokenStack.Peek();

                        int topPrecedence = PrecedenceToPrecedenceGroup(peeked.Precedence);
                        int myPrecedence = PrecedenceToPrecedenceGroup(t.Precedence);

                        if (myPrecedence > topPrecedence)
                        {
                            if (t.Precedence >= Precedence.BracketStart)
                            {
                                tokenStack.Push(t);
                            }
                        }
                        else
                        {
                            while (tokenStack.Count > 0)
                            {
                                Token top = tokenStack.Pop();
                                if (top.Precedence > Precedence.BracketStart)
                                {
                                     rpnList.Add(top);
                                }                             
                                
                                if (tokenStack.Count == 0)
                                {
                                    if (t.Precedence >= Precedence.BracketStart)
                                    {
                                        tokenStack.Push(t);
                                    }
                                    break;
                                }
                                else
                                {
                                    topPrecedence = PrecedenceToPrecedenceGroup(tokenStack.Peek().Precedence);
                                    if (myPrecedence > topPrecedence)
                                    {
                                        if (t.Precedence >= Precedence.BracketStart)
                                        {
                                            tokenStack.Push(t);
                                        }
                                        break;
                                    }
                                }
                            }
                        }
                    }
                }
            }

            if (tokenStack.Count > 0)
            {
                while (tokenStack.Count > 0)
                {
                    Token t = tokenStack.Pop();

                    if (t.Type == TokenTypes.Operator)
                    {
                        if (t.Precedence > Precedence.BracketStart)
                        {
                            rpnList.Add(t);
                        }
                    }
                    else
                    {
                        rpnList.Add(t);
                    }
                }
            }

            
            string dbgRpn = "";

            foreach (Token t in rpnList)
            {
                dbgRpn += t.Text + " ";
            }

            res.InputAsRpn = dbgRpn;

            // 3. evaluace RPN stacku
            int result = 0;
            for (int i = 0; i < rpnList.Count; i++)
            {
                Token t = rpnList[i];

                if (t.Type == TokenTypes.Number)
                {
                    rpnStack.Push(t);
                }
                else if (t.Type == TokenTypes.Operator)
                {        
                    // unární operátory
                    if (t.Precedence == Precedence.UnaryMinus || t.Precedence == Precedence.UnaryPlus)
                    {
                        Token un = rpnStack.Pop();
                        Token unRet = EvalUnaryOp(un, t);
                        rpnStack.Push(unRet);
                        continue;
                    }

                    // binární operátory
                    Token t1 = rpnStack.Pop();
                    Token t2 = rpnStack.Pop();
                    Token ret = EvalBinaryOp(t2, t1, t);
                    rpnStack.Push(ret);
                }
            }

            res.Input = this.input;
            
            Token stackTop = rpnStack.Pop();
            res.Result = stackTop.Text;
            res.ResultValue = int.Parse(stackTop.Text);

            return res;
        } 

        void Clear()
        {
            tokenStack.Clear();
            tokenQueue.Clear();
            currentToken = null;
            currentSeq = "";
            currentType = TokenTypes.Null;
            stackTopPrecedence = Precedence.None;
            rpnList.Clear();
            rpnStack.Clear();
        }

        Token EvalUnaryOp(Token arg1, Token op)
        {
            Token ret = new Token();
            int val = 0;

            if (op.Type == TokenTypes.Operator)
            {
                if (op.Precedence == Precedence.UnaryPlus)
                {
                    val = arg1.Value;
                }
                else if (op.Precedence == Precedence.UnaryMinus)
                {
                    val = -arg1.Value;
                }
            }

            ret.Precedence = Precedence.None;
            ret.Type = TokenTypes.Number;
            ret.Text = val.ToString();
            ret.Value = val;
            return ret;
        }

        Token EvalBinaryOp(Token arg1, Token arg2, Token op)
        {
            Token ret = new Token();
            int val = 0;

            if (op.Type == TokenTypes.Operator)
            {
                if (op.Precedence == Precedence.Plus)
                {
                    val = arg1.Value + arg2.Value;
                }
                else if (op.Precedence == Precedence.Divide)
                {
                     val = arg1.Value / arg2.Value;
                }
                else if (op.Precedence == Precedence.Times)
                {
                    val = arg1.Value * arg2.Value;
                }
                else if (op.Precedence == Precedence.Minus)
                {
                    val = arg1.Value - arg2.Value;
                }
            }

            ret.Precedence = Precedence.None;
            ret.Type = TokenTypes.Number;
            ret.Text = val.ToString();
            ret.Value = val;
            return ret;
        }

        int PrecedenceToPrecedenceGroup(Precedence precedence)
        {
            if (precedence == Precedence.None)
            {
                return -1;
            }
            
            if (precedence == Precedence.Eq)
            {
                return 0;
            }

            if (precedence == Precedence.BracketStart || precedence == Precedence.BracketEnd)
            {
                return 1;
            }

            if (precedence == Precedence.Plus || precedence == Precedence.Minus)
            {
                return 2;
            }

            if (precedence == Precedence.Tilde || precedence == Precedence.Times || precedence == Precedence.Divide)
            {
                return 3;
            }

            if (precedence == Precedence.Power)
            {
                return 4;
            }

            if (precedence == Precedence.UnaryPlus || precedence == Precedence.UnaryMinus)
            {
                return 5;
            }

            return -1;
        }
    }
}
