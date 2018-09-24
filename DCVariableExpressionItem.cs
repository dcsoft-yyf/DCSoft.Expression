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
    /// 变量元素
    /// </summary>
    [System.Runtime.InteropServices.ComVisible( false )]
    public class DCVariableExpressionItem : DCExpressionItem
    {
        /// <summary>
        /// 初始化对象
        /// </summary>
        public DCVariableExpressionItem()
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

        /// <summary>
        /// 执行表达式，根据上下文获取变量值
        /// </summary>
        /// <param name="context">上下文对象</param>
        /// <returns>变量值</returns>
        public override object Eval(IDCExpressionContext context)
        {
            return context.GetVariableValue(this.Name);
        }
        /// <summary>
        /// 输出调试用字符串
        /// </summary>
        /// <param name="indentLevel">层次</param>
        /// <param name="str">字符串对象</param>
        public override void ToDebugString(int indentLevel, StringBuilder str)
        {
            str.AppendLine(new string(' ', indentLevel * 3) + "VAR:" + this.Name);
        }

        /// <summary>
        /// 返回表示对象的字符串
        /// </summary>
        /// <returns>字符串</returns>
        public override string ToString()
        {
            return "VAR:" + this.Name;
        }
    }
}
