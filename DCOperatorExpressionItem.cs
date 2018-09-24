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
    /// 操作符元素
    /// </summary>
    [System.Runtime.InteropServices.ComVisible(false)]
    public class DCOperatorExpressionItem : DCExpressionItem
    {
        public DCOperatorExpressionItem()
        {
        }

        private string _Text = null;
        /// <summary>
        /// 文本
        /// </summary>
        public string Text
        {
            get
            {
                return _Text;
            }
            set
            {
                _Text = value;
            }
        }
        private DCOperatorType _Operator = DCOperatorType.None;
        /// <summary>
        /// 表达式类型
        /// </summary>
        public DCOperatorType Operator
        {
            get
            {
                return _Operator;
            }
            set
            {
                _Operator = value;
            }
        }

        private DCExpressionItem _Left = null;
        /// <summary>
        /// 左边的元素
        /// </summary>
        public DCExpressionItem Left
        {
            get
            {
                return _Left;
            }
            set
            {
                _Left = value;
            }
        }

        private DCExpressionItem _Right = null;
        /// <summary>
        /// 右边的元素
        /// </summary>
        public DCExpressionItem Rigth
        {
            get
            {
                return _Right;
            }
            set
            {
                _Right = value;
            }
        }

        private bool _IsLogicExpression = false;
        /// <summary>
        /// 是否为逻辑运算
        /// </summary>
        public bool IsLogicExpression
        {
            get
            {
                return this._IsLogicExpression;
            }
            set
            {
                this._IsLogicExpression = value;
            }
        }

        /// <summary>
        /// 执行表达式
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public override object Eval(IDCExpressionContext context)
        {
            switch (this.Operator)
            {
                #region 逻辑运算

                case DCOperatorType.And:
                    {
                        //逻辑与
                        bool v1 = GetBooleanValue(this.Left, context, false);
                        if( v1 == false )
                        {
                            return false;
                        }
                        bool v2 = GetBooleanValue(this.Rigth, context, false);
                        return v2;
                    }
                case DCOperatorType.Or:
                    {
                        //逻辑或
                        bool v1 = GetBooleanValue(this.Left, context, false);
                        if( v1 )
                        {
                            return true;
                        }
                        bool v2 = GetBooleanValue(this.Rigth, context, false);
                        return v2;
                        //return v1 && v2;
                    }
                #endregion
                #region 数值比较

                case DCOperatorType.Bigger:
                    {
                        // 大于
                        return GetEqualResult(context) > 0;
                    }
                case DCOperatorType.BiggerOrEqual:
                    {
                        // 大于等于
                        return GetEqualResult(context) >= 0;
                    }
                case DCOperatorType.Equal:
                    {
                        // 等于
                        return GetEqualResult(context) == 0;
                    }
                case DCOperatorType.Less:
                    {
                        // 大于等于
                        return GetEqualResult(context) < 0;
                    }
                case DCOperatorType.LessOrEqual:
                    {
                        // 大于等于
                        return GetEqualResult(context) <= 0;
                    }
                case DCOperatorType.Unequal:
                    {
                        // 不等于
                        return GetEqualResult(context) != 0;
                    }

                #endregion

                #region 数学运算 

                case DCOperatorType.Plus:
                    {
                        // 加法
                        double v1 = GetDoubleValue(this.Left, context, 0);
                        double v2 = GetDoubleValue(this.Rigth, context, 0);
                        return v1 + v2;
                    }
                case DCOperatorType.Minus:
                    {
                        // 减法
                        double v1 = GetDoubleValue(this.Left, context, 0);
                        double v2 = GetDoubleValue(this.Rigth, context, 0);
                        return v1 - v2;
                    }
                case DCOperatorType.Multi:
                    {
                        // 乘法
                        double v1 = GetDoubleValue(this.Left, context, 0);
                        double v2 = GetDoubleValue(this.Rigth, context, 0);
                        return v1 * v2;
                    }
                case DCOperatorType.Negative:
                    {
                        // 负数
                        double v1 = 0;
                        if (this.Rigth != null)
                        {
                            v1 = GetDoubleValue(this.Rigth, context, 0);
                        }
                        else if (this.Left != null)
                        {
                            v1 = GetDoubleValue(this.Left, context, 0);
                        }
                        return -v1;
                    }
                case DCOperatorType.Division:
                    {
                        // 除法
                        double v1 = GetDoubleValue(this.Left, context, 0);
                        double v2 = GetDoubleValue(this.Rigth, context, 0);
                        if (v2 == 0)
                        {
                            return double.NaN;
                        }
                        else
                        {
                            return v1 / v2;
                        }
                    }
                case DCOperatorType.Mod:
                    {
                        // 求模
                        double v1 = GetDoubleValue(this.Left, context, 0);
                        double v2 = GetDoubleValue(this.Rigth, context, 0);
                        if (v2 == 0)
                        {
                            return double.NaN;
                        }
                        else
                        {
                            return v1 % v2;
                        }
                    }

                #endregion

                default:
                    throw new NotSupportedException(this.Operator.ToString());
            }
         }

        /// <summary>
        /// 获得数值比较结果
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        private int GetEqualResult(IDCExpressionContext context)
        {
            object v1 = this.Left == null ? null : this.Left.Eval(context);
            object v2 = this.Rigth == null ? null : this.Rigth.Eval(context);
            if (v1 == v2)
            {
                return 0;
            }
            if ((v1 is int || v1 is double) && (v2 is int || v2 is double))
            {
                // 明确进行数字比较
                double d1 = Convert.ToDouble(v1);
                double d2 = Convert.ToDouble(v2);
                return d1.CompareTo(d2);
            }
            if (v1 is string && v2 is string)
            {
                // 明确进行字符串比较
                return string.Compare((string)v1, (string)v2);
            }

            if (v1 is float && v2 is float)
            {
                return ((float)v1).CompareTo((float)v2);
            }
            if (v1 is double && v2 is double)
            {
                return ((double)v1).CompareTo((double)v2);
            }

            bool hasContent1 = v1 != null && DBNull.Value.Equals(v1) == false ;
            bool hasContent2 = v2 != null && DBNull.Value.Equals(v2) == false ;

            if (hasContent1 && hasContent2  )
            {
                try
                {
                    // 两个数据都不为空
                    var t1 = v1.GetType();
                    var t2 = v2.GetType();
                    if (t1 == t2)
                    {
                        // 类型一致，则进行比较
                        return System.Collections.Comparer.Default.Compare(v1, v2);
                    }
                    // 类型不一样,则设置标准数据类型
                    var targetVT = (DCValueType)Math.Min((int)GetValueType(t1), (int)GetValueType(t2));
                    if (targetVT == DCValueType.Boolean)
                    {
                        bool b1 = Convert.ToBoolean(v1);
                        bool b2 = Convert.ToBoolean(v2);
                        return b1.CompareTo(b2);
                    }
                    else if (targetVT == DCValueType.Number)
                    {
                        double dbl1 = Convert.ToDouble(v1);
                        double dbl2 = Convert.ToDouble(v2);
                        return dbl1.CompareTo(dbl2);
                    }
                    else
                    {
                        string str1 = Convert.ToString(v1);
                        string str2 = Convert.ToString(v2);
                        return str1.CompareTo(str2);
                    }
                }
                catch (System.Exception ext)
                {
                    System.Diagnostics.Debug.WriteLine(ext.ToString());
                }
            }
            return hasContent1.CompareTo(hasContent2);
        }

        private DCValueType GetValueType(Type t)
        {
            if (t == typeof(string))
            {
                return DCValueType.String;
            }
            if (t == typeof(bool))
            {
                return DCValueType.Boolean;
            }
            if (t.IsValueType)
            {
                return DCValueType.Number;
            }
            return DCValueType.Number;
        }

        private double GetDoubleValue(DCExpressionItem item , IDCExpressionContext context , double defaultValue =0 )
        {
            if (item == null)
            {
                return defaultValue;
            }
            object v = item.Eval(context);
            return DCExpression.ToDouble(v , defaultValue);
        }
        private bool GetBooleanValue(DCExpressionItem item, IDCExpressionContext context , bool defaultValue = false )
        {
            if (item == null)
            {
                return defaultValue;
            }
            object v = item.Eval(context);
            return DCExpression.ToBoolean(v);
        }
        private string GetStringValue(DCExpressionItem item, IDCExpressionContext context )
        {
            if (item == null)
            {
                return null;
            }
            object v = item.Eval(context);
            return DCExpression.ToString(v);
        }

        internal override void AddSubItem(DCExpressionItem item)
        {
            if (this._Left == null)
            {
                this._Left = item;
            }
            else if (this._Right == null)
            {
                this._Right = item;
            }
            else
            {
                throw new NotSupportedException("无法添加超过2个子项目。");
            }
        }
        public override DCExpressionItem Clone()
        {
            DCOperatorExpressionItem result = (DCOperatorExpressionItem)this.MemberwiseClone();
            if (this._Left != null)
            {
                result._Left = this._Left.Clone();
            }
            if (this._Right != null)
            {
                result._Right = this._Right.Clone();
            }
            return result;
        }
        public override void ToDebugString(int indentLevel, StringBuilder str)
        {
            string preFix = new string(' ', indentLevel * 3);
            str.AppendLine(preFix + this.Operator + "(" + this.Text + ")" + ( this.IsLogicExpression ? "[logic]":""));
            if (this.Left != null)
            {
                str.AppendLine(preFix + " >Left:");
                this.Left.ToDebugString(indentLevel + 1, str);
            }
            if (this.Rigth != null)
            {
                str.AppendLine(preFix + " >Right:");
                this.Rigth.ToDebugString(indentLevel + 1, str);
            }
        }
        public override string ToString()
        {
            return this.Operator.ToString() + ":" + this.Text ;
        }
    }

    /// <summary>
    /// 操作符类型
    /// </summary>
    [System.Runtime.InteropServices.ComVisible(false)]
    public enum DCOperatorType
    {
        /// <summary>
        /// 无效操作
        /// </summary>
        None,
        /// <summary>
        /// 加法
        /// </summary>
        Plus,
        /// <summary>
        /// 减法
        /// </summary>
        Minus,
        /// <summary>
        /// 乘法
        /// </summary>
        Multi,
        /// <summary>
        /// 除法
        /// </summary>
        Division,
        /// <summary>
        /// 求模
        /// </summary>
        Mod,
        /// <summary>
        /// 逻辑与
        /// </summary>
        And,
        /// <summary>
        /// 逻辑或
        /// </summary>
        Or,
        /// <summary>
        /// 大于
        /// </summary>
        Bigger,
        /// <summary>
        /// 大于等于
        /// </summary>
        BiggerOrEqual,
        /// <summary>
        /// 小于
        /// </summary>
        Less ,
        /// <summary>
        /// 小于等于
        /// </summary>
        LessOrEqual,
        /// <summary>
        /// 等于
        /// </summary>
        Equal,
        /// <summary>
        /// 不等于
        /// </summary>
        Unequal,
        /// <summary>
        /// 求负数
        /// </summary>
        Negative
    }
}
