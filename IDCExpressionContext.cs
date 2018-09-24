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
    /// 表达式执行上下文对象
    /// </summary>
    [System.Runtime.InteropServices.ComVisible( false )]
    public interface IDCExpressionContext
    {
        /// <summary>
        /// 执行函数
        /// </summary>
        /// <param name="name">函数名</param>
        /// <param name="parameters">参数列表</param>
        /// <returns>函数返回值</returns>
        object ExecuteFunction(string name, object[] parameters);
        /// <summary>
        /// 获得变量值
        /// </summary>
        /// <param name="name">变量名</param>
        /// <returns>变量值</returns>
        object GetVariableValue(string name);
    }
}
