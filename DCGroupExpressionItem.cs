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
    /// 表达式元素组。
    /// </summary>
    [System.Runtime.InteropServices.ComVisible(false)]
    public class DCGroupExpressionItem : DCExpressionItem
    {
        public DCGroupExpressionItem()
        {
 
        }

        private DCExpressoinItemList _Items = new DCExpressoinItemList();
        /// <summary>
        /// 子项目列表
        /// </summary>
        public DCExpressoinItemList Items
        {
            get
            {
                return _Items;
            }
            set
            {
                _Items = value;
            }
        }
        /// <summary>
        /// 执行表达式
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public override object Eval(IDCExpressionContext context)
        {
            if (this.Items != null && this.Items.Count > 0)
            {
                return this.Items[0].Eval(context);
            }
            return null;
        }

        internal override void AddSubItem(DCExpressionItem item)
        {
            if (this._Items == null)
            {
                this._Items = new DCExpressoinItemList();
            }
            this._Items.Add(item);
        }

        public override DCExpressionItem Clone()
        {
            DCGroupExpressionItem resut = (DCGroupExpressionItem)this.MemberwiseClone();
            if (this._Items != null)
            {
                resut._Items = this._Items.CloneDeeply();
            }
            return resut;
        }

        public override void ToDebugString(int indentLevel, StringBuilder str)
        {
            string preFix = new string(' ', indentLevel * 3);
            if (this._Items != null && this._Items.Count > 0)
            {
                str.AppendLine(preFix + "(");
                foreach (var item in this._Items)
                {
                    item.ToDebugString(indentLevel + 1, str);
                }
                str.AppendLine(preFix + ")");
            }
            else
            {
                str.AppendLine(preFix + "( )");
            }
        }
        public override string ToString()
        {
            return "Group()";
        }
    }
}
