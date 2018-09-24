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
    /// 常量表达式元素。
    /// </summary>
    [System.Runtime.InteropServices.ComVisible( false )]
    public class DCConstExpressionItem : DCExpressionItem
    {
        public DCConstExpressionItem()
        {
        }
        /// <summary>
        /// 初始化对象
        /// </summary>
        /// <param name="v">数值</param>
        /// <param name="vt">数据类型</param>
        public DCConstExpressionItem(object v, DCValueType vt)
        {
            this._ValueType = vt;
            if (vt == DCValueType.String)
            {
                this._StringValue = (string)v;
            }
            else if (vt == DCValueType.Number)
            {
                this._NumberValue = (double)v;
            }
            else if (vt == DCValueType.Boolean)
            {
                this._BooleanValue = (bool)v;
            }
        }
        private string _StringValue = null;
        /// <summary>
        /// 字符串数值
        /// </summary>
        public string StringValue
        {
            get
            {
                return _StringValue;
            }
            set
            {
                _StringValue = value;
            }
        }

        private bool _BooleanValue = false;
        /// <summary>
        /// 布尔值数值
        /// </summary>
        public bool BooleanValue
        {
            get
            {
                return _BooleanValue;
            }
            set
            {
                _BooleanValue = value;
            }
        }

        private double _NumberValue = 0;
        /// <summary>
        /// 数字数值
        /// </summary>
        public double NumberValue
        {
            get
            {
                return _NumberValue;
            }
            set
            {
                _NumberValue = value;
            }
        }
        private DCValueType _ValueType = DCValueType.String;
        /// <summary>
        /// 数据类型
        /// </summary>
        public DCValueType ValueType
        {
            get
            {
                return _ValueType;
            }
            set
            {
                _ValueType = value;
            }
        }
        /// <summary>
        /// 执行表达式
        /// </summary>
        /// <param name="context">上下文对象</param>
        /// <returns>执行结果</returns>
        public override object Eval(IDCExpressionContext context)
        {
            if (this.ValueType == DCValueType.String)
            {
                return this._StringValue;
            }
            if (this.ValueType == DCValueType.Number)
            {
                return this._NumberValue;
            }
            if (this.ValueType == DCValueType.Boolean)
            {
                return this._BooleanValue;
            }
            throw new NotSupportedException(this.ValueType.ToString());
        }
        /// <summary>
        /// 调试输出文本
        /// </summary>
        /// <param name="indentLevel"></param>
        /// <param name="str"></param>
        public override void ToDebugString(int indentLevel, StringBuilder str)
        {
            string txt = new string(' ', indentLevel * 3);
            if (this.ValueType == DCValueType.Number)
            {
                txt = txt + "Number:" + this.NumberValue;
            }
            else if (this.ValueType == DCValueType.String)
            {
                txt = txt + "String:" + this.StringValue;
            }
            else
            {
                txt = txt + "Boolean:" + this.BooleanValue;
            }
            str.AppendLine(txt);
        }
        public override string ToString()
        {
            if (this.ValueType == DCValueType.Number)
            {
                return "Number:" + this.NumberValue;
            }
            else if (this.ValueType == DCValueType.String)
            {
                return "String:" + this.StringValue;
            }
            else if (this.ValueType == DCValueType.Boolean)
            {
                return "Boolean:" + this.BooleanValue;
            }
            return this.ValueType.ToString();
        }
    }

    
}
