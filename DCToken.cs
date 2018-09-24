/*
 
    都昌数值表达式引擎 DCSoft.Expression

 南京都昌信息科技有限公司 2018年 版权所有 
 公司网址 http://www.dcwriter.cn

 */
using System;
using System.Collections.Generic;
using System.Text;

namespace DCSoft.Expression
{
    /// <summary>
    /// 符号对象
    /// </summary>
    [System.Runtime.InteropServices.ComVisible(false)]
    internal class DCToken
    {
        public CharType Type = CharType.None;
        public string Text = null;
        public StringBuilder _Str = new StringBuilder();
        public override string ToString()
        {
            return this.Type + "\t:" + this.Text;
        }
        //public int Level
        //{
        //    get
        //    {
        //        if (this.Text == "*"
        //            || this.Text == "/"
        //            || this.Text == "%")
        //        {
        //            return 3;
        //        }
        //        if (this.Text == "+" || this.Text == "-")
        //        {
        //            return 2;
        //        }
        //        if (this.Type == CharType.MathOperator)
        //        {
        //            return 1;
        //        }
        //        if (this.Type == CharType.Symbol || this.Type == CharType.StringConst)
        //        {
        //            return 0;
        //        }
        //        if (this.Type == CharType.Spliter)
        //        {
        //            return -1;
        //        }

        //        return 0;
        //    }
        //}
    }

    /// <summary>
    /// 符号对象列表
    /// </summary>
    [System.Runtime.InteropServices.ComVisible(false)]
    internal class DCTokenList : List<DCToken>
    {
        public DCTokenList(string txt)
        {
            this.Parse(txt);
        }
        
        private int _Position = -1;
        public DCToken Current
        {
            get
            {
                if (_Position >= 0 && _Position < this.Count)
                {
                    return this[_Position];
                }
                return null;
            }
        }
        public DCToken NextItem
        {
            get
            {
                if (_Position >= 0 && this._Position < this.Count - 1)
                {
                    return this[_Position + 1];
                }
                return null;
            }
        }
        public void ResetPosition()
        {
            this._Position = -1;
        }
        public bool MoveNext()
        {
            this._Position++;
            if (this._Position >= 0 && this._Position < this.Count)
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// 获得所有操作符字符串
        /// </summary>
        /// <returns></returns>
        public void Parse(string text)
        {
            this.Clear();
            if (text == null || text.Length == 0)
            {
                return;
            }

            DCToken currentToken = null;
            StringMode sm = StringMode.None;

            for (int position = 0; position < text.Length; position++)
            {
                char c = text[position];
                if (sm == StringMode.SingleQuotes || sm == StringMode.DoubleQuotes)
                {
                    // 正在定义字符串
                    if (c == '\\')
                    {
                        // 开始转义
                        const string EigthDigs = "01234567";
                        const string HexDigs = "0123456789ABCDEF";
                        char nextC = NextChar(text, position);
                        if (EigthDigs.IndexOf(nextC) >= 0)
                        {
                            // 三位八进制数字
                            string v = NextChars(text, position, 3);
                            if (v != null && v.Length == 3)
                            {
                                position += 3;
                                int num = ParseNumber(v, EigthDigs);
                                currentToken._Str.Append((char)num);
                            }
                            else
                            {
                                throw new System.Exception("长度不够:" + text);
                            }
                        }
                        else if (nextC == 'x')
                        {
                            // 两个十六进制
                            position++;
                            string v = NextChars(text, position, 2);
                            if (v != null && v.Length == 2)
                            {
                                v = v.ToUpper();
                                position += 2;
                                int num = ParseNumber(v, HexDigs);
                                currentToken._Str.Append((char)num);
                            }
                            else
                            {
                                throw new System.Exception("长度不够:" + text);
                            }
                        }
                        else if (nextC == 'a')
                        {
                            currentToken._Str.Append('\a');
                        }
                        else if (nextC == 'b')
                        {
                            currentToken._Str.Append('\b');
                        }
                        else if (nextC == 'n')
                        {
                            currentToken._Str.Append('\n');
                        }
                        else if (nextC == 'r')
                        {
                            currentToken._Str.Append('\r');
                        }
                        else if (nextC == 'v')
                        {
                            currentToken._Str.Append('\v');
                        }
                        else if (nextC == '"')
                        {
                            currentToken._Str.Append('"');
                        }
                        else if (nextC == '\'')
                        {
                            currentToken._Str.Append('\'');
                        }
                        else
                        {
                            throw new System.Exception("不支持的转移:" + nextC);
                        }
                    }
                    else if (sm == StringMode.SingleQuotes && c == '\'')
                    {
                        // 结束定义单引号字符串
                        currentToken._Str.Append(c);
                        currentToken = null;
                        sm = StringMode.None;
                    }
                    else if (sm == StringMode.DoubleQuotes && c == '"')
                    {
                        // 结束定义双引号字符串
                        currentToken._Str.Append(c);
                        currentToken = null;
                        sm = StringMode.None;
                    }
                    else
                    {
                        // 添加正常字符
                        currentToken._Str.Append(c);
                    }
                }
                else
                {
                    // 不是正在定义字符串
                    if (c == '\'')
                    {
                        // 开始定义单引号字符串
                        currentToken = new DCToken();
                        currentToken.Type = CharType.StringConst;
                        currentToken._Str.Append(c);
                        sm = StringMode.SingleQuotes;
                        this.Add(currentToken);
                    }
                    else if (c == '"')
                    {
                        // 开始定义双引号字符串
                        currentToken = new DCToken();
                        currentToken.Type = CharType.StringConst;
                        currentToken._Str.Append(c);
                        sm = StringMode.DoubleQuotes;
                        this.Add(currentToken);
                    }
                    else if (char.IsWhiteSpace(c))
                    {
                        // 遇到空白字符则过滤掉空白字符
                        currentToken = null;
                        for (; position < text.Length; position++)
                        {
                            if (char.IsWhiteSpace(text[position]) == false)
                            {
                                position--;
                                break;
                            }
                        }
                    }
                    else
                    {
                        // 普通字符
                        if (c == '(')
                        {
                            currentToken = new DCToken();
                            currentToken._Str.Append(c);
                            currentToken.Type = CharType.CurLeft;
                            this.Add(currentToken);
                            currentToken = null;
                        }
                        else if (c == ')')
                        {
                            currentToken = new DCToken();
                            currentToken._Str.Append(c);
                            currentToken.Type = CharType.CurRight;
                            this.Add(currentToken);
                            currentToken = null;
                        }
                        else
                        {
                            CharType ct = GetChartType(c);

                            if (currentToken == null || currentToken.Type != ct)
                            {
                                currentToken = new DCToken();
                                currentToken.Type = ct;
                                this.Add(currentToken);
                            }
                            currentToken._Str.Append(c);
                        }
                    }
                }
            }//for
            if (currentToken != null && currentToken._Str.Length > 0)
            {
                if (this.Contains(currentToken) == false)
                {
                    this.Add(currentToken);
                }
            }
            foreach (var item in this)
            {
                if (item._Str != null)
                {
                    item.Text = item._Str.ToString();
                    item._Str = null;
                }
            }
        }

        /// <summary>
        /// 文本转换为数字
        /// </summary>
        /// <param name="txt">文本</param>
        /// <param name="digs">数字字符</param>
        /// <returns>转换后的数字</returns>
        private int ParseNumber(string txt, string digs)
        {
            int v = 0;
            for (int iCount = 0; iCount < txt.Length; iCount++)
            {
                int i = digs.IndexOf(txt[iCount]);
                if (i < 0)
                {
                    throw new System.InvalidCastException(digs + ":" + txt);
                }
                v = v * digs.Length + i;
            }
            return v;
        }
        /// <summary>
        /// 获得下一个字符
        /// </summary>
        /// <returns></returns>
        private char NextChar(string text, int position)
        {
            if (position < text.Length - 1)
            {
                return text[position + 1];
            }
            return char.MinValue;
        }

        private string NextChars(string text, int position, int len)
        {
            if (position < text.Length - len)
            {
                string v = text.Substring(position, len);
                return v;
            }
            return null;
        }


        /// <summary>
        /// 获得字符类型
        /// </summary>
        /// <param name="c">字符</param>
        /// <param name="isVB">是否为ＶＢ语法</param>
        /// <returns>字符类型</returns>
        private CharType GetChartType(char c)
        {
            if (c == '$')
            {
                return CharType.Symbol;
            }

            if (c == '(')
            {
                return CharType.CurLeft;
            }
            if (c == ')')
            {
                return CharType.CurRight;
            }
            // 为了保持兼容性，不支持方括号。
            //if (c == '[')
            //{
            //    return CharType.SquLeft;
            //}
            //if( c == ']')
            //{
            //    return CharType.SquRight;
            //}
            if (c == ',')
            {
                return CharType.Spliter;
            }
            if (c == '+' 
                || c == '-'
                || c == '*'
                || c == '/'
                || c =='%'
                || c == '\\')
            {
                return CharType.MathOperator;
            }
            if ( c == '&' || c == '^' || c == '|'  || c == '='
                || c == '>' || c == '<')
            {
                return CharType.LogicOperator;
            }
            else if (char.IsWhiteSpace(c))
            {
                return CharType.Whitespace;
            }
            if (c == ':'
                || c == '!'
                || c == '.'
                || char.IsLetterOrDigit(c)
                || char.IsSymbol(c)
                || c == '[' || c == ']')
            {
                return CharType.Symbol;
            }
            return CharType.Symbol;
        }
    }


    internal enum StringMode
    {
        /// <summary>
        /// 无
        /// </summary>
        None,
        /// <summary>
        /// 单引号字符串
        /// </summary>
        SingleQuotes,
        /// <summary>
        /// 多引号字符串
        /// </summary>
        DoubleQuotes
    }

    internal enum CharType
    {
        None,
        /// <summary>
        /// 标识符
        /// </summary>
        Symbol,
        /// <summary>
        /// 数学运算符
        /// </summary>
        MathOperator,
        /// <summary>
        /// 逻辑运算符
        /// </summary>
        LogicOperator,
        /// <summary>
        /// 左边的圆括号
        /// </summary>
        CurLeft,
        /// <summary>
        /// 右边的圆括号
        /// </summary>
        CurRight,
        /// <summary>
        /// 左边的方括号
        /// </summary>
        SquLeft,
        /// <summary>
        /// 右边的方括号
        /// </summary>
        SquRight,
        /// <summary>
        /// 空白字符
        /// </summary>
        Whitespace,
        /// <summary>
        /// 分隔字符
        /// </summary>
        Spliter,
        /// <summary>
        /// 字符串
        /// </summary>
        StringConst
    }

}
