using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NB_AMQP
{
    public partial class Form2 : Form
    {
        public Form2()
        {
            InitializeComponent();
        }
        /// <summary>
        /// 名称：IsNumberic
        /// 功能：判断输入的是否是数字
        /// 参数：string oText：源文本
        /// 返回值：　bool true:是　false:否
        /// </summary>
        public bool IsNumberic(string oText)
        {
            try
            {
                int var1 = Convert.ToInt32(oText);
                return true;
            }
            catch
            {
                return false;
            }
        }

        private void Form2_Load(object sender, EventArgs e)
        {
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
        }

        private void button_save_Click(object sender, EventArgs e)
        {
            if(!IsNumberic(text_temperature_threshold.Text)&& IsNumberic(text_humidity_threshold.Text))
            {
                MessageBox.Show("请输入温度和湿度！");
            }
            else
            {
                Mysetting.Default.humidity_threshold = Convert.ToInt32(text_humidity_threshold.Text);
                Mysetting.Default.temperature_threshold = Convert.ToInt32(text_temperature_threshold.Text);
                Mysetting.Default.Save();
                MessageBox.Show("温湿度阈值设置成功，请重启窗口使之生效！");
                this.Close();
            }
        }

        private void button_cancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
