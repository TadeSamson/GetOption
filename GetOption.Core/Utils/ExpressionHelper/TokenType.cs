using System;
using System.Collections.Generic;
using System.Text;

namespace GetOption.Core.Utils.ExpressionHelper
{
    internal enum TokenType { PropertyString, Expresssion, BinaryOperator = 8, AssignmentOperator = 9, OpenBracket = 1, CloseBracket =10 }
}
