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
    /// 表达式对象，为顶级API类型。
    /// </summary>
    [System.Runtime.InteropServices.ComVisible(false)]
    public class DCExpression
    {
        //private static Dictionary<string, DCExpression> _Instances 
        //    = new Dictionary<string, DCExpression>();
        ///// <summary>
        ///// 使用缓存的创建对象
        ///// </summary>
        ///// <param name="text">表达式文本</param>
        ///// <returns>创建的对象</returns>
        //public static DCExpression CreateUseBuffer(string text)
        //{
        //    if (text == null || text.Length == 0)
        //    {
        //        throw new ArgumentNullException("text");
        //    }
        //    if (_Instances.ContainsKey(text))
        //    {
        //        return _Instances[text];
        //    }
        //    DCExpression exp = new DCExpression(text);
        //    _Instances[text] = exp;
        //    return exp;
        //}
        ///// <summary>
        ///// 清空内部的缓存区
        ///// </summary>
        //public static void ClearBuffer()
        //{
        //    _Instances.Clear();
        //}

        /// <summary>
        /// 初始化对象
        /// </summary>
        public DCExpression()
        {
        }
        /// <summary>
        /// 初始化对象
        /// </summary>
        /// <param name="txt">表达式文本</param>
        public DCExpression(string txt)
        {
            this.Parse(txt);
        }

        private string _Text = null;
        /// <summary>
        /// 表达式原始文本
        /// </summary>
        public string Text
        {
            get
            {
                return _Text;
            }
        }

        private DCExpressionItem _RootItem = null;
        /// <summary>
        /// 根表达式元素对象
        /// </summary>
        public DCExpressionItem RootItem
        {
            get
            {
                return _RootItem;
            }
        }
        /// <summary>
        /// 执行表达式
        /// </summary>
        /// <param name="context">上下文对象</param>
        /// <returns>运算结果</returns>
        public object Eval(IDCExpressionContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }
            if (this._RootItem == null)
            {
                throw new NullReferenceException("this.RootItem");
            }
            object result = this._RootItem.Eval(context);
            if (result is Array)
            {
                Array arr = (Array)result;
                if (arr.Length > 0)
                {
                    return arr.GetValue(0);
                }
                else
                {
                    return null;
                }
            }
            return result;
        }
        /// <summary>
        /// 转换为调试用的文本
        /// </summary>
        /// <returns></returns>
        public string ToDebugString()
        {
            if (this._RootItem == null)
            {
                return "";
            }
            StringBuilder str = new StringBuilder();
            str.AppendLine("Text:" + this.Text);
            str.AppendLine("------------------");
            this._RootItem.ToDebugString(0, str);
            return str.ToString();
        }
        /// <summary>
        /// 解析表达式
        /// </summary>
        /// <param name="text"></param>
        public void Parse(string text)
        {
            this._Text = text;
            DCTokenList tokens = new DCTokenList(text);
            this._RootItem = new DCGroupExpressionItem();
            ParseItem(this._RootItem, tokens);
        }
        /// <summary>
        /// 解析表达式项目
        /// </summary>
        /// <param name="rootItem"></param>
        /// <param name="tokens"></param>
        private void ParseItem(DCExpressionItem rootItem, DCTokenList tokens)
        {
            List<DCExpressionItem> items = new List<DCExpressionItem>();
            while (tokens.MoveNext())
            {
                DCExpressionItem newItem = null;
                DCToken token = tokens.Current;
                if (token.Type == CharType.Symbol)
                {
                    // 根据关键字 And Or 进行修复。
                    if (string.Equals(token.Text, "And", StringComparison.CurrentCultureIgnoreCase))
                    {
                        token.Type = CharType.MathOperator;
                        token.Text = "&&";
                    }
                    else if (string.Equals(token.Text, "or", StringComparison.CurrentCultureIgnoreCase))
                    {
                        token.Type = CharType.MathOperator;
                        token.Text = "||";
                    }
                }
                if (token.Type == CharType.Symbol)
                {
                    // 标识符
                    DCToken next = tokens.NextItem;
                    if (next != null && next.Type == CharType.CurLeft)
                    {
                        // 函数调用
                        tokens.MoveNext();
                        DCFunctionExpressionItem func = new DCFunctionExpressionItem();
                        func.Name = token.Text;
                        ParseItem(func, tokens);
                        newItem = func;
                        newItem.Priority = 0;
                    }
                    else
                    {
                        if (string.Compare(token.Text, "true", true) == 0)
                        {
                            // 布尔值常量
                            newItem = new DCConstExpressionItem(true, DCValueType.Boolean);
                        }
                        else if (string.Compare(token.Text, "false", true) == 0)
                        {
                            // 布尔值常量
                            newItem = new DCConstExpressionItem(false, DCValueType.Boolean);
                        }
                        else
                        {
                            double dbl = 0;
                            if (double.TryParse(token.Text, out dbl))
                            {
                                // 数字常量
                                newItem = new DCConstExpressionItem(dbl, DCValueType.Number);
                            }
                            else
                            {
                                // 引用的变量
                                DCVariableExpressionItem var = new DCVariableExpressionItem();
                                var.Name = token.Text;
                                newItem = var;
                            }
                        }
                        newItem.Priority = 0;
                    }
                }
                else if (token.Type == CharType.StringConst)
                {
                    // 字符串常量
                    var strV = token.Text;
                    if (strV != null && strV.Length >= 2)
                    {
                        if (strV[0] == '\'' || strV[0] == '"')
                        {
                            strV = strV.Substring(1, strV.Length - 2);
                        }
                    }
                    newItem = new DCConstExpressionItem(strV, DCValueType.String);
                }
                else if (token.Type == CharType.CurLeft)
                {
                    // 左圆括号，则进行分组
                    DCGroupExpressionItem group = new DCGroupExpressionItem();
                    newItem = group;
                    newItem.Priority = 0;
                    ParseItem(group, tokens);
                }
                else if (token.Type == CharType.Spliter
                    || token.Type == CharType.CurRight)
                {
                    // 分隔符号或者右圆括号则退出分组
                    if (items == null || items.Count == 0)
                    {
                        throw new System.Exception("项目分组无效:" + this.Text);
                    }
                    if (items != null && items.Count > 0)
                    {
                        DCExpressionItem item = CollpaseItems(items);
                        rootItem.AddSubItem(item);
                    }
                    items = new List<DCExpressionItem>();
                    if (token.Type == CharType.CurRight)
                    {
                        //退出分组
                        break;
                    }
                    else
                    {
                        continue;
                    }
                }
                else if (token.Type == CharType.MathOperator
                    || token.Type == CharType.LogicOperator)
                {
                    // 操作符号

                    DCOperatorExpressionItem math = new DCOperatorExpressionItem();
                    math.Text = token.Text;
                    switch (token.Text)
                    {
                        case "+":
                            math.Operator = DCOperatorType.Plus;
                            math.Priority = 1;
                            break;
                        case "-":
                            math.Operator = DCOperatorType.Minus;
                            math.Priority = 1;
                            break;
                        case "*":
                            math.Operator = DCOperatorType.Multi;
                            math.Priority = 2;
                            break;
                        case "/":
                            math.Operator = DCOperatorType.Division;
                            math.Priority = 2;
                            break;
                        case ">":
                            math.Operator = DCOperatorType.Bigger;
                            math.Priority = 0;
                            math.IsLogicExpression = true;
                            break;
                        case ">=":
                            math.Operator = DCOperatorType.BiggerOrEqual;
                            math.Priority = 0;
                            math.IsLogicExpression = true;
                            break;
                        case "=":
                            math.Operator = DCOperatorType.Equal;
                            math.Priority = 0;
                            math.IsLogicExpression = true;
                            break;
                        case "==":
                            math.Operator = DCOperatorType.Equal;
                            math.Priority = 0;
                            math.IsLogicExpression = true;
                            break;
                        case "<":
                            math.Operator = DCOperatorType.Less;
                            math.Priority = 0;
                            math.IsLogicExpression = true;
                            break;
                        case "<=":
                            math.Operator = DCOperatorType.LessOrEqual;
                            math.Priority = 0;
                            math.IsLogicExpression = true;
                            break;
                        case "%":
                            math.Operator = DCOperatorType.Mod;
                            math.Priority = 2;
                            break;
                        case "||":
                            math.Operator = DCOperatorType.Or;
                            math.Priority = -1;
                            math.IsLogicExpression = true;
                            break;
                        case "&&":
                            math.Operator = DCOperatorType.And;
                            math.Priority = -1;
                            math.IsLogicExpression = true;
                            break;
                        case "!=":
                            math.Operator = DCOperatorType.Unequal;
                            math.Priority = 0;
                            math.IsLogicExpression = true;
                            break;
                        default:
                            throw new NotSupportedException("无效操作符:" + token.Text);
                    }
                    newItem = math;
                }
                else
                {
                    throw new NotSupportedException(token.Type + ":" + token.Text);
                }
                items.Add(newItem);
            }//while
            DCExpressionItem item2 = CollpaseItems(items);
            if (item2 != null)
            {
                rootItem.AddSubItem(item2);
            }
        }

        /// <summary>
        /// 表达式元素列表的收缩
        /// </summary>
        /// <param name="items"></param>
        /// <returns></returns>
        private DCExpressionItem CollpaseItems(List<DCExpressionItem> items)
        {
            if (items.Count == 1)
            {
                return items[0];
            }
            if (items.Count == 0)
            {
                return null;
            }
            // 特别处理减法操作
            for (int iCount = 0; iCount < items.Count; iCount++)
            {
                if (items[iCount] is DCOperatorExpressionItem && iCount < items.Count - 1)
                {
                    DCOperatorExpressionItem item = (DCOperatorExpressionItem)items[iCount];
                    if (item.Operator == DCOperatorType.Minus)
                    {
                        bool isNegative = false;
                        if (iCount == 0)
                        {
                            //  处于第一的位置，必然是负数操作。
                            isNegative = true;
                        }
                        else
                        {
                            DCExpressionItem preItem = items[iCount - 1];
                            if (preItem is DCOperatorExpressionItem)
                            {
                                DCOperatorExpressionItem preOper = (DCOperatorExpressionItem)preItem;
                                if (preOper.IsLogicExpression)
                                {
                                    // 前面的元素是逻辑判断元素，是负数操作。
                                    isNegative = true;
                                }
                            }
                            else if (preItem is DCGroupExpressionItem)
                            {
                                // 前面的是表达式组元素，为负数操作。
                                isNegative = true;
                            }
                        }
                        if (isNegative)
                        {
                            // 将减法转换为负数运算
                            item.Operator = DCOperatorType.Negative;
                            item.Rigth = items[iCount + 1];
                            items.RemoveAt(iCount + 1);
                            item.Priority = 0;
                            item.Collapsed = true;
                            //iCount--;
                        }
                    }//if
                }//if
            }//for
            // 此处进行多次循环，每次循环将优先级最高的操作符元素提升层次。
            // 一般来说到最后只剩下一个操作符元素。
            while (items.Count > 1)
            {
                DCOperatorExpressionItem maxPriorityItem = null;
                int maxIndex = -1;
                int len = items.Count;
                // 首先找到优先级最高的操作符。
                for (int iCount = 0; iCount < len; iCount++)
                {
                    DCExpressionItem item = items[iCount];
                    if (item.Collapsed == false && item is DCOperatorExpressionItem)
                    {
                        if (maxPriorityItem == null || maxPriorityItem.Priority < item.Priority)
                        {
                            maxPriorityItem = (DCOperatorExpressionItem)item;
                            maxIndex = iCount;
                        }
                    }
                }//for
                if (maxPriorityItem == null)
                {
                    // 没找到要操作的运算符，则退出循环.
                    break;
                }

                if (maxIndex < items.Count - 1)
                {
                    // 吞并右边的项目
                    maxPriorityItem.Rigth = items[maxIndex + 1];
                    items.RemoveAt(maxIndex + 1);
                }
                if (maxIndex > 0)
                {
                    // 吞并左边的项目
                    maxPriorityItem.Left = items[maxIndex - 1];
                    items.RemoveAt(maxIndex - 1);
                }
                //if (maxPriorityItem.Operator == DCOperatorType.Minus
                //    && maxPriorityItem.Left == null
                //    && maxPriorityItem.Rigth != null)
                //{
                //    // 将减法操作转换为负数操作
                //    maxPriorityItem.Operator = DCOperatorType.Negative;
                //}
                // 设置运算符已经被处理过了。
                maxPriorityItem.Collapsed = true;
            }//while
            if (items.Count > 0)
            {
                return items[0];
            }
            return null;
        }

        /// <summary>
        /// 复制对象
        /// </summary>
        /// <returns>复制品</returns>
        public DCExpression Clone()
        {
            DCExpression result = (DCExpression)this.MemberwiseClone();
            if (this._RootItem != null)
            {
                result._RootItem = this._RootItem.Clone();
            }
            return result;
        }

        internal static bool ToBoolean(object obj, bool defaultValue = false)
        {
            if (obj == null || DBNull.Value.Equals(obj))
            {
                return defaultValue;
            }
            if (obj is bool)
            {
                return (bool)obj;
            }
            if (obj is string)
            {
                if (string.Compare((string)obj, "true", true) == 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            if (obj is float)
            {
                float v = (float)obj;
                if (float.IsNaN(v) || v == 0)
                {
                    return false;
                }
                return true;
            }
            if (obj is double)
            {
                double v = (double)obj;
                if (double.IsNaN(v) || v == 0)
                {
                    return false;
                }
                return true;
            }
            if (obj is int)
            {
                int v = (int)obj;
                if (v == NaN || v == 0)
                {
                    return false;
                }
                return true;
            }
            if (obj.GetType().IsValueType)
            {
                try
                {
                    int v = Convert.ToInt32(obj);
                    if (v == NaN || v == 0)
                    {
                        return false;
                    }
                    return true;
                }
                catch (System.Exception ext)
                {
                    return defaultValue;
                }
            }
            if (typeof(Array).IsInstanceOfType(obj))
            {
                Array arr = (Array)obj;
                if (arr.Length > 0)
                {
                    object v2 = arr.GetValue(0);
                    return ToBoolean(v2, defaultValue);
                }
            }
            return defaultValue;
        }

        /// <summary>
        /// 表示非数字的数值
        /// </summary>
        internal const int NaN = 2147439148;
        /// <summary>
        /// 将对象转换为字符串值
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        internal static string ToString(object obj)
        {
            if (obj == null || DBNull.Value.Equals(obj))
            {
                return null;
            }
            if (obj.GetType().IsArray)
            {
                StringBuilder str = new StringBuilder();
                foreach (object item in ((System.Collections.IEnumerable)obj))
                {
                    if (item != null)
                    {
                        string v = ToString(item);
                        if (string.IsNullOrEmpty(v) == false)
                        {
                            str.Append(v);
                        }
                    }
                }
                return str.ToString();
            }
            return Convert.ToString(obj);
        }
        /// <summary>
        /// 将对象转换为浮点数
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        internal static double ToDouble(object obj, double defaultValue = 0)
        {
            if (obj is float)
            {
                if (float.IsNaN((float)obj))
                {
                    return defaultValue;
                }
                else
                {
                    return (double)obj;
                }
            }
            if (obj is double)
            {
                if (double.IsNaN((double)obj))
                {
                    return defaultValue;
                }
                else
                {
                    return (double)obj;
                }
            }
            if (obj != null && obj.GetType().IsArray)
            {
                foreach (object item in ((System.Collections.IEnumerable)obj))
                {
                    if (item != null)
                    {
                        double v2 = ToDouble(item, double.NaN);
                        if (double.IsNaN(v2) == false)
                        {
                            return v2;
                        }
                    }
                }
                return defaultValue;
            }
            if (obj is string)
            {
                string s = (string)obj;
                if (string.IsNullOrEmpty(s))
                {
                    return defaultValue;
                }
                double v2 = defaultValue;
                if (double.TryParse(s, out v2))
                {
                    return v2;
                }
                else
                {
                    return defaultValue;
                }
            }
            double dbl = Convert.ToDouble(obj);
            if (double.IsNaN(dbl))
            {
                return defaultValue;
            }
            else
            {
                return dbl;
            }
        }
    }
}