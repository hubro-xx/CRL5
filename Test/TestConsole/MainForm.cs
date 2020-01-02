/**
* CRL 快速开发框架 V5
* Copyright (c) 2019 Hubro All rights reserved.
* GitHub https://github.com/hubro-xx/CRL5
* 主页 http://www.cnblogs.com/hubro
* 在线文档 http://crl.changqidongli.com/
*/
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using SqlSugar;
using CRL;
using System.Drawing.Drawing2D;

namespace TestConsole
{
    public partial class MainForm : Form
    {
        Dictionary<string, Action<int>> methods = new Dictionary<string, Action<int>>();
        public MainForm()
        {
            InitializeComponent();

        
            methods.Add("DapperQueryTest(SQL)", MappingSpeedTest.DapperQueryTest);
            methods.Add("SqlSugarQueryTest", MappingSpeedTest.SugarQueryTest);
            //methods.Add("LoognQueryTest(SQL)", MappingSpeedTest.LoognQueryTest);
            methods.Add("CRLQueryTest", MappingSpeedTest.CRLQueryTest);
            methods.Add("CRLSQLQueryTest(SQL)", MappingSpeedTest.CRLSQLQueryTest);
            methods.Add("ChloeQueryTest", MappingSpeedTest.ChloeQueryTest);
            methods.Add("EFLinqQueryTest", MappingSpeedTest.EFLinqQueryTest);
            methods.Add("EFSqlQueryTest(SQL)", MappingSpeedTest.EFSqlQueryTest);
            methods.Add("LinqToDBQueryTest", MappingSpeedTest.LinqToDBQueryTest);
            methods.Add("FreeSqlTest", MappingSpeedTest.FreeSqlTest);

        }

        private async void MainForm_Load(object sender, EventArgs e)
        {
            TestConsole.CRLManage.Instance.QueryItem(b => b.Id > 0);
            //var obj = CRL.Base.CreateObjectTest<TestEntity>();
            //MappingSpeedTest.LinqToDBQueryTest(1);

            button1.Enabled = false;
            button2.Enabled = false;
            ///调试时会线程错误
            await Task.Run(() =>
            {
                TestConsole.CRLManage.Instance.QueryItem(b => b.Id > 0);
          
                this.BeginInvoke(new Action(()=>
                {
                    labTip.Visible = false;
                    button1.Enabled = true;
                    button2.Enabled = true;
                }));
        
            });
        }
        private async void button1_Click(object sender, EventArgs e)
        {
            button1.Enabled = false;
            button2.Enabled = false;
            var n = Convert.ToInt32(textBox1.Text);
            string txt = "查询 top " + n + "行数据\r\n";
            txtResult.Clear();
            txtResult.AppendText(txt);
            long useTime;
            await Task.Run(()=>
            {
                foreach (var kv in methods)
                {
                    System.Threading.Thread.Sleep(300);
                    var method = kv.Value;
                    var counter = new CounterWatch();
                    counter.Start(kv.Key, () =>
                    {
                        method(n);
                    }, 3);
                    txt = counter.ToString() + "\r\n";
                    if (count1.ContainsKey(kv.Key))
                    {
                        count1[kv.Key] += counter.ElapsedMilliseconds;
                    }
                    else
                    {
                        count1.Add(kv.Key, counter.ElapsedMilliseconds);
                    }
                    this.BeginInvoke(new Action(() =>
                    {
                        txtResult.AppendText(txt);
                    }));
                }
                this.BeginInvoke(new Action(() =>
                {
                    button1.Enabled = true;
                    button2.Enabled = true;
                    showTotal(dataGridView1, count1);
                }));
            });
            
        }
        Dictionary<string, long> count1 = new Dictionary<string, long>();
        Dictionary<string, long> count2 = new Dictionary<string, long>();
        private async void button2_Click(object sender, EventArgs e)
        {
            button1.Enabled = false;
            button2.Enabled = false;
            var n = Convert.ToInt32(textBox2.Text);
            string txt = "top 1 轮循" + n + "次\r\n";
            txtResult.Clear();
            txtResult.AppendText(txt);
            await Task.Run(() =>
            {
                long useTime;

                foreach (var kv in methods)
                {
                    var item = kv.Value;
                    var counter = new CounterWatch();
                    counter.Start(kv.Key, () =>
                    {
                        item(1);
                    }, n);
                    txt = counter.ToString() + "\r\n";
                    if (count2.ContainsKey(kv.Key))
                    {
                        count2[kv.Key] += counter.ElapsedMilliseconds;
                    }
                    else
                    {
                        count2.Add(kv.Key, counter.ElapsedMilliseconds);
                    }
                    this.BeginInvoke(new Action(() =>
                    {
                        txtResult.AppendText(txt);
                    }));
                }
        
                this.BeginInvoke(new Action(() =>
                {
                    button1.Enabled = true;
                    button2.Enabled = true;
                    showTotal(dataGridView2, count2);
                }));
            });
        }

        void showTotal(DataGridView dv, Dictionary<string, long> dic)
        {
            var data = new List<dynamic>();
            foreach (var kv in dic)
            {
                data.Add(new { name = kv.Key, value = kv.Value });
            }
            dv.DataSource = data;
        }
    }
}
