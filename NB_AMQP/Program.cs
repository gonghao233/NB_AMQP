using DAL;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NB_AMQP
{
    static class Program
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main()
        {
            CommstaticClass.ConnectionStrings = ConfigurationManager.ConnectionStrings["NB_AMQP"].ConnectionString;
            CommstaticClass.ConnectionString_CreatDB = ConfigurationManager.ConnectionStrings["Creat_DB"].ConnectionString;
            SQLhelper sqlhelper = new SQLhelper();
            string DATABASE_name="MY_DB",Table_name = "WENANDSHI";
            sqlhelper.Create_DB(DATABASE_name, Table_name);
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
        }
    }
}
