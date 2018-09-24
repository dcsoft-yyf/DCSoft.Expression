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
    /// 抽象的表达式元素类型。所有的表达式元素都是从这个类型派生出来的。
    /// </summary>
    [System.Runtime.InteropServices.ComVisible(false)]
    public abstract class DCExpressionItem
    {
        /// <summary>
        /// 初始化对象
        /// </summary>
        public DCExpressionItem()
        {
        }
        /// <summary>
        /// 执行表达式
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public virtual object Eval(IDCExpressionContext context)
        {
            return null;
        }

        private int _Priority = 0;
        /// <summary>
        /// 运算优先级。数值越大则优先级越高。
        /// </summary>
        internal int Priority
        {
            get
            {
                return _Priority;
            }
            set
            {
                _Priority = value;
            }
        }

        private bool _Collapsed = false;
        /// <summary>
        /// 已经收缩了的表达式
        /// </summary>
        internal bool Collapsed
        {
            get
            {
                return _Collapsed;
            }
            set
            {
                _Collapsed = value;
            }
        }
        /// <summary>
        /// 复制对象
        /// </summary>
        /// <returns>复制品</returns>
        public virtual DCExpressionItem Clone()
        {
            throw new NotSupportedException("Clone()");
        }

        internal virtual void AddSubItem(DCExpressionItem item)
        {
        }

        public virtual void ToDebugString(int indentLevel, StringBuilder str)
        {
        }
    }
}
