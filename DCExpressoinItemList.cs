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
    /// 表达式对象列表
    /// </summary>
    [System.Runtime.InteropServices.ComVisible(false)]
    public class DCExpressoinItemList : List<DCExpressionItem>
    {
        public DCExpressoinItemList()
        {
        }

        public DCExpressoinItemList CloneDeeply()
        {
            DCExpressoinItemList list = new DCExpressoinItemList();
            foreach (var item in this)
            {
                list.Add(item.Clone());
            }
            return list;
        }

        internal void ToDebugString(int indentLevel, StringBuilder str)
        {
            foreach (var item in this)
            {
                item.ToDebugString(indentLevel, str);
            }
        }
    }
}
