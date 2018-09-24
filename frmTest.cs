/*
 
    都昌数值表达式引擎 DCSoft.Expression

 南京都昌信息科技有限公司 2018年 版权所有 
 公司网址 http://www.dcwriter.cn

 */
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Reflection;

namespace DCSoft.Expression
{
    /// <summary>
    /// 测试功能窗口
    /// </summary>
    [System.Runtime.InteropServices.ComVisible(false)]
    public partial class frmTest : Form
    {
        static class Program
        {
            /// <summary>
            /// 应用程序的主入口点。
            /// </summary>
            [STAThread]
            static void Main()
            {
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new frmTest());
            }
        }

        public frmTest()
        {
            InitializeComponent();
        }


        private void Form1_Load(object sender, EventArgs e)
        {
            cboExpression.Items.Add("SIN(99+123)");
            cboExpression.Items.Add("A*99.1+MAX(-11,-10)");
            cboExpression.Items.Add("FIND('7048dcb034d94ce6bfd750c3f1672096',[zz])>=0");
            cboExpression.Items.Add("SUM(A1:B3)");
            cboExpression.Items.Add("[A1]-[B2]");
            cboExpression.Items.Add("A+B+C+在三+是+213");
            cboExpression.Items.Add("A+B*(C+D*(E-F))-999");
            cboExpression.Items.Add("-A+B+C+D");
            cboExpression.Items.Add("A+B*C-D");
            cboExpression.Items.Add("10+8>-9");
        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            DCTokenList tokens = new DCTokenList(cboExpression.Text);
            StringBuilder result = new StringBuilder();
            for (int iCount = 0; iCount < tokens.Count; iCount++)
            {
                result.AppendLine(iCount.ToString("00") + ":" + tokens[iCount].ToString());
            }
            DCExpression exp = new DCExpression(cboExpression.Text);
            result.AppendLine(exp.ToDebugString());
            MyContext c = new MyContext();
            object vresult = exp.Eval(c);
            result.AppendLine("运算结果:" + vresult);
            txtResult.Text = result.ToString();
        }

        private class MyContext : IDCExpressionContext
        {
            public object ExecuteFunction(string name, object[] parameters)
            {
                name = name.ToUpper();
                try
                {
                    switch (name)
                    {
                        case "SIN": return Math.Sin(Convert.ToDouble(parameters[0]));
                        case "MAX":
                            return Math.Max(
                                    Convert.ToDouble(parameters[0]),
                                    Convert.ToDouble(parameters[1]));
                        default:
                            MessageBox.Show("不支持的函数" + name);
                            return 0;
                    }
                }
                catch (System.Exception ext)
                {
                    MessageBox.Show("执行函数" + name + " 错误:" + ext.Message);
                    return 0;
                }
            }

            public object GetVariableValue(string name)
            {
                name = name.ToUpper();
                switch (name)
                {
                    case "A": return 1.5;
                    case "B": return 2.3;
                    case "C": return -99;
                    case "D": return 998;
                    case "E": return 123.5;
                    case "F": return 3.1415;
                    default:
                        MessageBox.Show("不支持的参数名:" + name);
                        return 0;
                }
            }
        }
    }
}