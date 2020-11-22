using System;
using System.Collections.Generic;
using System.Text;

namespace infixtoreversepolish
{
    public class Tokenizer
    {
        string input = "";
        Queue<Token> tokenQueue = new Queue<Token>();
        Token currentToken = null;
        string currentSeq = "";
        TokenTypes currentType = TokenTypes.Null;
        Keywords keywords = new Keywords();

        void Clear()
        {
            tokenQueue.Clear();
            currentToken = null;
            currentSeq = "";
            currentType = TokenTypes.Null;
        }

        public Queue<Token> Tokenize(string input)
        {
            this.input = input;
            Clear();

            for (int i = 0; i < input.Length; i++)
            {
                char c = input[i];      
                
                if (c == ';')
                {
                    if (currentType != TokenTypes.Null)
                    {
                        ResolveCurrentToken(i);
                    }

                    currentType = TokenTypes.Semicolon;
                }
                else if (c >= 48 && c <= 57)
                {
                    if (currentType != TokenTypes.Number)
                    {
                        if (currentType != TokenTypes.Null)
                        {
                            ResolveCurrentToken(i);
                        }

                        currentType = TokenTypes.Number;                                
                    }
                }
                else if (c >= 40 && c <= 47)
                {
                   // if (currentType != TokenTypes.Operator) // [todo] vyřešit pomocí lookahead() operátory delší než 1 znak
                    {
                        if (currentType != TokenTypes.Null)
                        {
                            ResolveCurrentToken(i);
                        }

                        currentType = TokenTypes.Operator;
                    }
                }
                else if ((c >= 65 && c <= 90) || (c >= 97 && c <= 122))
                {
                    if (currentType != TokenTypes.Word)
                    {
                        currentType = TokenTypes.Word;
                        ResolveCurrentToken(i);
                    }
                }  
                
                if (char.IsWhiteSpace(c))
                {
                    if (currentType == TokenTypes.String)
                    {
                         currentSeq += c;
                    }
                }
                else
                {
                    currentSeq += c;
                }
            }

            ResolveCurrentToken(input.Length);

            return tokenQueue;
        }

        bool MatchOp(char c)
        {
            return c >= 40 && c <= 47;
        }

        bool MatchBracketEnd(char c)
        {
            return c == ')';
        }

        void ResolveCurrentToken(int currentIndex)
        {
            currentToken = new Token {Text = currentSeq, Type = currentType};

            if (currentType == TokenTypes.Operator)
            {
                if (currentSeq == "*")
                {
                    currentToken.Precedence = Precedence.Times;
                }
                else if (currentSeq == "+")
                {
                    if (currentIndex == 1 || (MatchOp(input[currentIndex - 2]) && !MatchBracketEnd(input[currentIndex - 2])))
                    {
                        currentToken.Precedence = Precedence.UnaryPlus;
                    }
                    else
                    {
                        currentToken.Precedence = Precedence.Plus;
                    }   
                }
                else if (currentSeq == "-")
                {
                    if (currentIndex == 1 || (MatchOp(input[currentIndex - 2]) && !MatchBracketEnd(input[currentIndex - 2])))
                    {
                        currentToken.Precedence = Precedence.UnaryMinus;
                    }
                    else
                    {
                        currentToken.Precedence = Precedence.Minus;
                    }  
                }
                else if (currentSeq == "/")
                {
                    currentToken.Precedence = Precedence.Divide;
                }
                else if (currentSeq == "(")
                {
                     currentToken.Precedence = Precedence.BracketStart;
                }
                else if (currentSeq == ")")
                {
                     currentToken.Precedence = Precedence.BracketEnd;
                }
            }
            else if (currentType == TokenTypes.Number)
            {
                currentToken.Value = int.Parse(currentSeq);
            }
            else if (currentType == TokenTypes.Semicolon)
            {

            }
            else if (currentType == TokenTypes.Word)
            {
                currentToken.Type = TokenTypes.Keyword;
            }

            currentSeq = "";

            if (currentType != TokenTypes.Word)
            {
                currentType = TokenTypes.Void;
            }

            if (currentToken.Text != "")
            {
                tokenQueue.Enqueue(currentToken);
            }

        }

    }
}
