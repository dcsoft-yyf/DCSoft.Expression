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
    /// 数值类型
    /// </summary>
    [System.Runtime.InteropServices.ComVisible( false )]
    public enum DCValueType
    {
        /// <summary>
        /// 数字
        /// </summary>
        Number,
        /// <summary>
        /// 字符串
        /// </summary>
        String,
        /// <summary>
        /// 布尔值
        /// </summary>
        Boolean
    }
}
