using System;
using System.Collections.Generic;
using System.Text;

namespace GetOption.Core.Utils.ExpressionHelper
{

    internal class Token
    {
        public object value { get; set; }
        public TokenType TokenType { get; set; }

        // operator token such as >=, <= cannot be passed for now. Parser will be updted later to enable look behind.

        internal static List<Token> OperatorTokens
        {
            get
            {
                return new List<Token>()
                {
            new Token() { value= "&&", TokenType= TokenType.BinaryOperator },
            new Token() { value= "||", TokenType= TokenType.BinaryOperator },
            new Token() { value= "=", TokenType= TokenType.AssignmentOperator },
            new Token() { value= "!=", TokenType= TokenType.AssignmentOperator },
            new Token() { value= "<", TokenType= TokenType.AssignmentOperator },
            //new Token() { value= "<=", TokenType= TokenType.AssignmentOperator },
            new Token() { value= ">", TokenType= TokenType.AssignmentOperator },
            //new Token() { value= ">=", TokenType= TokenType.AssignmentOperator },
            new Token() { value= "%", TokenType= TokenType.AssignmentOperator },
            new Token() { value= "(", TokenType= TokenType.OpenBracket },
            new Token() { value= ")", TokenType= TokenType.CloseBracket }
                };

            }
        }

    }
}
