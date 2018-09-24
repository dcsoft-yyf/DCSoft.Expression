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
    /// 函数表达式项目
    /// </summary>
    [System.Runtime.InteropServices.ComVisible(false)]
    public class DCFunctionExpressionItem : DCExpressionItem
    {
        public DCFunctionExpressionItem()
        {
        }

        private string _Name = null;
        /// <summary>
        /// 名称
        /// </summary>
        public string Name
        {
            get
            {
                return _Name;
            }
            set
            {
                _Name = value;
            }
        }

        private DCExpressoinItemList _Parameters = new DCExpressoinItemList();
        /// <summary>
        /// 参数列表
        /// </summary>
        public DCExpressoinItemList Parameters
        {
            get
            {
                return _Parameters;
            }
            set
            {
                _Parameters = value;
            }
        }
        /// <summary>
        /// 执行表达式
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public override object Eval(IDCExpressionContext context)
        {
            object[] pvs = null;
            if (_Parameters != null && _Parameters.Count > 0)
            {
                pvs = new object[this._Parameters.Count];
                for (int iCount = 0; iCount < this._Parameters.Count; iCount++)
                {
                    pvs[iCount] = this._Parameters[iCount].Eval(context);
                }
            }
            object result = context.ExecuteFunction(this.Name, pvs);
            return result;
        }

        internal override void AddSubItem(DCExpressionItem item)
        {
            if (_Parameters == null)
            {
                _Parameters = new DCExpressoinItemList();
            }
            _Parameters.Add(item);
        }

        public override DCExpressionItem Clone()
        {
            DCFunctionExpressionItem item = (DCFunctionExpressionItem)this.MemberwiseClone();
            if (this._Parameters != null)
            {
                item._Parameters = this._Parameters.CloneDeeply();
            }
            return item;
        }
        public override void ToDebugString(int indentLevel, StringBuilder str)
        {
            string preFix = new string(' ', indentLevel * 3);
            if (this.Parameters != null && this.Parameters.Count > 0)
            {
                str.AppendLine(preFix + "function " + this.Name);
                str.AppendLine(preFix + "(");
                foreach (var item in this.Parameters)
                {
                    item.ToDebugString(indentLevel + 1, str);
                }
                str.AppendLine(preFix + ")");
            }
            else
            {
                str.AppendLine(preFix + this.Name + "( )");
            }
        }
        public override string ToString()
        {
            return "function " + this.Name + "( )";
        }
    }
    /// <summary>
    /// 函数类型
    /// </summary>
    [System.Runtime.InteropServices.ComVisible(false)]
    public enum DCFunctionType
    {
        /// <summary>
        /// 字符串函数
        /// </summary>
        String,
        /// <summary>
        /// 数值函数
        /// </summary>
        Number,
        /// <summary>
        /// 聚合函数
        /// </summary>
        Group
    }
}
