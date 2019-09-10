/* 
 * Copyright (C) 2012-2019, Mathos Project.
 * All rights reserved.
 * 
 * Please see the license file in the project folder
 * or go to https://github.com/MathosProject/Mathos-Parser/blob/master/LICENSE.md.
 * 
 * Please feel free to ask me directly at my email!
 *  artem@artemlos.net
 */

using System;

namespace Mathos.Parser.Scripting
{
    public sealed class ScriptParserException : Exception
    {
        public ScriptParserException() : base()
        {
        }

        public ScriptParserException(string message) : base(message)
        {
        }

        public ScriptParserException(string message, Exception innerException) : base(message, innerException)
        {
        }

        public ScriptParserException(int line, Exception innerException) : base(innerException.GetType().Name + " on line " + line + ", " + Environment.NewLine + innerException.Message, innerException)
        {
        }
    }
}