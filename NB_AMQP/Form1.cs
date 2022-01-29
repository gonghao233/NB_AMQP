using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Amqp;
using Amqp.Framing;
using Amqp.Sasl;
using Newtonsoft.Json;
using System.Windows.Forms;
using System.Threading;
using DAL;
using GMap.NET;
using GMap.NET.MapProviders;
using GMap.NET.WindowsForms;
using System.Net;
using GMap.NET.WindowsForms.Markers;
using System.Windows.Forms.DataVisualization.Charting;
using System.Data.SqlClient;
namespace NB_AMQP
{
    public partial class Form1 : Form
    {

        public Form1()
        {
            InitializeComponent();
        }
        static string Host = "a161726b49.iot-amqps.cn-north-4.myhuaweicloud.com";
        /// <summary>
        /// 端口
        /// </summary>
        static int Port = 5671;

        /// <summary>
        /// 接入凭证键值
        /// </summary>
        static string AccessKey = "6FjAWeqx";

        /// <summary>
        /// 接入凭证密钥
        /// </summary>
        static string AccessCode = "zDn7zUHOgMEfoWtBvTo9U95iyt9J9XNE";

        /// <summary>
        /// 队列名
        /// </summary>
        //static string QueueName = "${yourQueue}";
        static string QueueName = "DefaultQueue";
        static Connection connection;

        static Session session;

        static ReceiverLink receiverLink;

        static DateTime lastConnectTime = DateTime.Now;

        private void Form1_Load(object sender, EventArgs e)
        {
            //if (string.isnullorempty(text_temperature_threshold.text))
            //{
            //    text_temperature_threshold.text = "5";
            //}

            //if (string.isnullorempty(text_humidity_threshold.text))
            //{
            //    text_humidity_threshold.text = "45";
            //}

            /*数据表格初始化*/
            chart1.Series.Clear();
            List<UP_info> list = new SQLhelper().select();
            if (list != null)
            {
                Series temperature_series = new Series("温度");
                Series humidity_series = new Series("湿度");
                chart1.DataSource = list;

                temperature_series.ChartType = SeriesChartType.Line;
                temperature_series.XValueMember = "event_time";
                temperature_series.YValueMembers = "wendu";
                temperature_series.ToolTip = "温度：#VALY\n时间：#VALX";
                temperature_series.MarkerColor = Color.Red;
                temperature_series.MarkerSize = 4;
                temperature_series.MarkerStyle = MarkerStyle.Circle;

                humidity_series.ChartType = SeriesChartType.Line;
                humidity_series.XValueMember = "event_time";
                humidity_series.YValueMembers = "shidu";


                int temperature_threshold = Mysetting.Default.temperature_threshold;
                int humidity_threshold = Mysetting.Default.humidity_threshold;

                StripLine stripMax_temperature = new StripLine();
                StripLine stripMax_humidity = new StripLine();

                stripMax_temperature.Text = string.Format("温度阈值：{0:F}", temperature_threshold);//展示文本
                //stripMax.BackColor = Color.FromArgb(208, 109, 106);//背景色
                stripMax_temperature.BackColor = Color.Black;//背景色
                stripMax_temperature.Interval = 0;//间隔
                stripMax_temperature.IntervalOffset = temperature_threshold;//偏移量
                stripMax_temperature.StripWidth = 0.1;//线宽
                stripMax_temperature.ForeColor = Color.Black;//前景色
                stripMax_temperature.TextAlignment = StringAlignment.Near;//文本对齐方式

                stripMax_humidity.Text = string.Format("湿度阈值：{0:F}",humidity_threshold);//展示文本
                //stripMax.BackColor = Color.FromArgb(208, 109, 106);//背景色
                stripMax_humidity.BackColor = Color.Black;//背景色
                stripMax_humidity.Interval = 0;//间隔
                stripMax_humidity.IntervalOffset = humidity_threshold;//偏移量
                stripMax_humidity.StripWidth = 0.1;//线宽
                stripMax_humidity.ForeColor = Color.Black;//前景色
                stripMax_humidity.TextAlignment = StringAlignment.Near;//文本对齐方式

                chart1.ChartAreas[0].AxisY.StripLines.Add(stripMax_temperature);//添加到ChartAreas中
                chart1.ChartAreas[0].AxisY.StripLines.Add(stripMax_humidity);

                chart1.ChartAreas[0].AxisY.MajorGrid.Enabled = false;
                chart1.ChartAreas[0].AxisX.MajorGrid.Enabled = false;

                chart1.Series.Add(temperature_series);
                chart1.Series.Add(humidity_series);
            }

            chart2.Series.Clear();
            if (list != null)
            {
                Series temperature_series = new Series("温度");
                Series humidity_series = new Series("湿度");
                chart2.DataSource = list;
                //chart2.DataSource = list;

                temperature_series.ChartType = SeriesChartType.Line;
                temperature_series.XValueMember = "event_time";
                temperature_series.YValueMembers = "wendu";

                temperature_series.MarkerColor = Color.Red;
                temperature_series.MarkerSize = 4;
                temperature_series.MarkerStyle = MarkerStyle.Circle;
                //temperature_series.MarkerBorderColor = Color.Black;
                //temperature_series.MarkerBorderWidth = 1;
                temperature_series.ToolTip = "温度：#VALY\n 时间：#VALX";


                humidity_series.ChartType = SeriesChartType.Line;
                humidity_series.XValueMember = "event_time";
                humidity_series.YValueMembers = "shidu";



                //最大线条
                int max = Mysetting.Default.humidity_threshold;
                StripLine stripMax = new StripLine();
                stripMax.Text = string.Format("最大：{0:F}", max);//展示文本
                //stripMax.BackColor = Color.FromArgb(208, 109, 106);//背景色
                stripMax.BackColor = Color.Black;//背景色
                stripMax.Interval = 0;//间隔
                stripMax.IntervalOffset = max;//偏移量
                stripMax.StripWidth = 0.01;//线宽
                stripMax.ForeColor = Color.Black;//前景色
                stripMax.TextAlignment = StringAlignment.Near;//文本对齐方式
                chart2.ChartAreas[0].AxisY.StripLines.Add(stripMax);//添加到ChartAreas中
                chart2.ChartAreas[0].AxisY.MajorGrid.Enabled = false;

                chart2.Series.Add(temperature_series);
                chart2.Series.Add(humidity_series);
                chart2.ChartAreas[0].AxisX.ScrollBar.Enabled = Enabled;
                chart2.ChartAreas[0].AxisX.ScrollBar.ButtonColor = Color.LightBlue;
                chart2.ChartAreas[0].AxisX.ScrollBar.IsPositionedInside = Enabled;
                chart2.ChartAreas[0].AxisX.ScaleView.Size = 10;
                chart2.ChartAreas[0].AxisX.IntervalAutoMode = IntervalAutoMode.VariableCount;

                
            }
            this.BackgroundImage = Image.FromFile("E:/NB_AMQP_CODE 测试 2022.1.22/NB_AMQP/NB_AMQP/obj/Debug/背景图片.jpg");
            this.WindowState = FormWindowState.Maximized;

            DataGridViewTextBoxColumn col_deviceid = new DataGridViewTextBoxColumn();
            DataGridViewTextBoxColumn col_temperature = new DataGridViewTextBoxColumn();
            DataGridViewTextBoxColumn col_humidity = new DataGridViewTextBoxColumn();
            DataGridViewTextBoxColumn col_datatime = new DataGridViewTextBoxColumn();

            //DataGridViewTextBoxCell celltext = new DataGridViewTextBoxCell();

            col_deviceid.HeaderText = "设备序号";
            col_temperature.HeaderText = "温度";
            col_humidity.HeaderText = "湿度";
            col_datatime.HeaderText = "数据上报时间";

            dataGridView1.Columns.Add(col_deviceid);
            dataGridView1.Columns.Add(col_temperature);
            dataGridView1.Columns.Add(col_humidity);
            dataGridView1.Columns.Add(col_datatime);
            foreach (DataGridViewColumn column in dataGridView1.Columns)
            {
                column.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            }
        }
        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                var connection = CreateConnection();
                // 添加Connection Exception回调
                connection.AddClosedCallback(ConnectionClosed);

                // 创建Session。
                var session = new Session(connection);

                // 创建ReceiverLink
                var receiver = new ReceiverLink(session, "receiverName", QueueName);

                //接收消息。
                ReceiveMessage(receiver);
            }
            catch (Exception err)
            {
                Console.WriteLine(err);
            }
        }

        static Connection CreateConnection()
        {
            lastConnectTime = DateTime.Now;
            long timestamp = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeMilliseconds();
            string userName = "accessKey=" + AccessKey + "|timestamp=" + timestamp;
            Address address = new Address(Host, Port, userName, AccessCode);
            ConnectionFactory factory = new ConnectionFactory();
            factory.SASL.Profile = SaslProfile.External;
            // 信任服务端,跳过证书校验
            factory.SSL.RemoteCertificateValidationCallback = (sender, certificate, chain, sslPolicyError) => { return true; };
            factory.AMQP.IdleTimeout = 8000;
            factory.AMQP.MaxFrameSize = 8 * 1024;
            factory.AMQP.HostName = "default";
            var connection = factory.CreateAsync(address).Result;
            return connection;
        }

        static void ReceiveMessage(ReceiverLink receiver)
        {
            receiver.Start(20, (link, message) =>
            {
                // 在线程池中处理消息，防止阻塞拉取消息的线程
                ThreadPool.QueueUserWorkItem((obj) => ProcessMessage(obj), message);
                // 回ACK
                link.Accept(message);
            });
        }

        //public string str;
        public static void textbox_meassage(TextBox textBox,int text)
        {
            textBox.Text = text.ToString();
        }
        static void ProcessMessage(Object obj)
        {
            if (obj is Amqp.Message message)
            {
                string body = message.Body.ToString();
                Rootobject ra = JsonConvert.DeserializeObject<Rootobject>(body);

                UP_info info = new UP_info();
                info.device_number = ra.notify_data.body.services[0].properties.device_number;
                info.wendu = ra.notify_data.body.services[0].properties.wendu;
                info.shidu = ra.notify_data.body.services[0].properties.shidu;
                //string m_time= ra.notify_data.body.services[0].event_time;
                DateTime dt = DateTime.ParseExact(ra.notify_data.body.services[0].event_time, "yyyyMMddTHHmmssZ", System.Globalization.CultureInfo.CurrentCulture);
                //DateTime convertdate = DateTime.Parse(m_time);
                //info.event_time = ra.notify_data.body.services[0].event_time;
                info.event_time = dt.ToString();
                //MessageBox.Show()
                info.device_id = ra.notify_data.header.device_id;
                SQLhelper sqlhelper = new SQLhelper();
                sqlhelper.UP_INFO(info);


                //public_class.info_map.shidu= ra.notify_data.body.services[0].properties.shidu; 
                //public_class.info_map.wendu= ra.notify_data.body.services[0].properties.wendu; 


            }

            var connection = CreateConnection();
            // 添加Connection Exception回调
            connection.AddClosedCallback(ConnectionClosed);

            // 创建Session。
            var session = new Session(connection);

            // 创建ReceiverLink
            var receiver = new ReceiverLink(session, "receiverName", QueueName);

            //接收消息。
            ReceiveMessage(receiver);
        }


        static void ConnectionClosed(IAmqpObject amqpObject, Amqp.Framing.Error e)
        {
            // 断线重连,15s重连一次
            while (DateTime.Now.CompareTo(lastConnectTime.AddSeconds(15)) < 0)
            {
                Thread.Sleep(1000);
            }
            ShutDown();

            var connection = CreateConnection();
            // 添加Connection Exception回调
            connection.AddClosedCallback(ConnectionClosed);

            // 创建Session。
            var session = new Session(connection);

            // 创建ReceiverLink
            var receiver = new ReceiverLink(session, "receiverName", QueueName);

            //接收消息。
            ReceiveMessage(receiver);
        }

        public static void ShutDown()
        {
            if (receiverLink != null)
            {
                try
                {
                    MessageBox.Show("关闭！");
                    receiverLink.Close();
                }
                catch (Exception e)
                {
                    MessageBox.Show("close receiverLink error, exception =" + e);
                }

            }
            if (session != null)
            {
                try
                {
                    session.Close();
                }
                catch (Exception e)
                {
                    MessageBox.Show("close session error, exception =" + e);
                }

            }

            if (connection != null)
            {
                try
                {
                    connection.Close();
                }
                catch (Exception e)
                {
                    MessageBox.Show("close connection error, exception =" + e);
                }

            }
        }

        private void gMapControl_Load(object sender, EventArgs e)
        {
            gMapControl.MapProvider = GMapProviders.BingMap;
           // gMapControl.MapProvider = GMapProviders.GoogleChinaHybridMap;

            gMapControl.Manager.Mode = AccessMode.ServerOnly;
            //String mapPath = Application.StartupPath + "\\bingmap.gmdb";
            //GMap.NET.GMaps.Instance.ImportFromGMDB(mapPath);  
            //gMapControl.SetPositionByKeywords("china,harbin");//设置初始中心为china harbin
            gMapControl.Position = new PointLatLng(45.75, 126.63);


            //不显示中心十字点
            gMapControl.ShowCenter = false;
            //左键拖拽地图
            gMapControl.DragButton = MouseButtons.Left;
            gMapControl.MinZoom = 2;   //最小缩放
            gMapControl.MaxZoom = 24;  //最大缩放
            gMapControl.Zoom = 10;      //当前缩放

            GMapOverlay markersOverlay = new GMapOverlay("markers");//创建新的图层markersOverlay
            GMarkerGoogle marker = new GMarkerGoogle(new PointLatLng(45.75, 126.63), GMarkerGoogleType.green);//定点创建新的标记marker
            //StringBuilder tooltiptext = new StringBuilder("温度：");
            //tooltiptext.AppendFormat("{0:D}", public_class.info_map.wendu);
            //tooltiptext.Append("湿度：");
            //tooltiptext.AppendFormat("{0:D}", public_class.info_map.shidu);
            //marker.ToolTipText = tooltiptext.ToString();
            marker.ToolTipText = "节点1";
            marker.ToolTipMode = MarkerTooltipMode.Always;
            markersOverlay.Markers.Add(marker);//新的图层markersOverlay 上增加marker这个标记
            
            //var labelMarker = new GmapMarkerWithLabel(new PointLatLng(46.75, 127.63), "caption text", GMarkerGoogleType.blue);
            //markersOverlay.Markers.Add(labelMarker);
            gMapControl.Overlays.Add(markersOverlay);//地图上增加markersOverlay 这个图层
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            
        }

        //public void data_bingding()
        //{
        //    text_wendu.DataBindings.Add(new Binding("Text", UP_info, "wendu"))
        //}

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void timer_text_Tick(object sender, EventArgs e)
        {   
            
            int number = 1;
            SQLhelper sqlhelper = new SQLhelper();
            List<UP_info> My_list=sqlhelper.Select_lastest_info( number);
            if(My_list!=null)
            { 
                text_wendu.Text = My_list[0].wendu.ToString();
                text_shidu.Text = My_list[0].shidu.ToString();
                int device_number = My_list[0].device_number;
                int temperature = My_list[0].wendu;
                int humidity = My_list[0].shidu;
                string device_id = My_list[0].device_id;
                string data_time = My_list[0].event_time;
                //text_wendu.Text= "1";
                if(dataGridView1.Rows.Count<My_list[0].device_number)
                {
                    DataGridViewRow row = new DataGridViewRow();
                    dataGridView1.Rows.Add(row);
                }
                int row_index = device_number - 1;
                dataGridView1.Rows[row_index].Cells[0].Value = device_number;
                dataGridView1.Rows[row_index].Cells[1].Value = temperature;
                dataGridView1.Rows[row_index].Cells[2].Value = humidity;
                dataGridView1.Rows[row_index].Cells[3].Value = data_time;
            }


        }

        private void chart1_Click(object sender, EventArgs e)
        {
        }

        private void text_wendu_TextChanged(object sender, EventArgs e)
        {

        }

        private void chart1_Click_1(object sender, EventArgs e)
        {
            
        }

        private void timer_chart_Tick(object sender, EventArgs e)
        {
            chart1.Series[0].Points.Clear();
            Series temperature_series = new Series("温度");
            Series humidity_series = new Series("湿度");
            /*数据表格初始化*/
            chart1.Series.Clear();
            List<UP_info> list = new SQLhelper().select();
            if (list != null)
            {
                chart1.DataSource = list;
                chart1.DataSource = list;
                //chart1.Series["temperature"].ChartType = SeriesChartType.Line;
                //chart1.Series["temperature"].XValueMember = "event_time";
                //chart1.Series["temperature"].YValueMembers = "wendu";
                temperature_series.ChartType = SeriesChartType.Line;
                temperature_series.XValueMember = "event_time";
                temperature_series.YValueMembers = "wendu";
                temperature_series.ToolTip = "温度：#VALY\n时间：#VALX";
                temperature_series.MarkerColor = Color.Red;
                temperature_series.MarkerSize = 4;
                temperature_series.MarkerStyle = MarkerStyle.Circle;

                humidity_series.ChartType = SeriesChartType.Line;
                humidity_series.XValueMember = "event_time";
                humidity_series.YValueMembers = "shidu";

                chart2.ChartAreas[0].AxisY.MajorGrid.Enabled = false;
                chart2.ChartAreas[0].AxisX.MajorGrid.Enabled = false;

                chart1.Series.Add(temperature_series);
                chart1.Series.Add(humidity_series);
                chart1.ChartAreas[0].AxisX.ScrollBar.ButtonColor = Color.LightBlue;
            }
        }

        private void yuzhishezhi_Click(object sender, EventArgs e)
        {
            Form2 form = new Form2();
            form.Show();
        }



        //public class ObjectUtil
        //{
        //    /// <summary>
        //    /// 获取某个对象中的属性值
        //    /// </summary>
        //    /// <param name="info"></param>
        //    /// <param name="field"></param>
        //    /// <returns></returns>
        //    public static object GetPropertyValue(object info, string field)
        //    {
        //        if (info == null) return null;
        //        Type t = info.GetType();
        //        IEnumerable<System.Reflection.PropertyInfo> property = from pi in t.GetProperties() where pi.Name.ToLower() == field.ToLower() select pi;
        //        return property.First().GetValue(info, null);
        //    }
        //}

    }

    //class timer_init: Form1
    //{
    //    public timer_init()
    //    {
    //        System.Timers.Timer t = new System.Timers.Timer(10000);//实例化Timer类，设置间隔时间为10000毫秒；
    //        t.Elapsed += new System.Timers.ElapsedEventHandler(Execute);//到达时间的时候执行事件；
    //        t.AutoReset = false;//设置是执行一次（false）还是一直执行(true)；
    //        t.Enabled = true;//是否执行System.Timers.Timer.Elapsed事件；
    //        t.Start(); //启动定时器
    //                   //上面初始化代码可以写到构造函数中
    //        void Execute(object source, System.Timers.ElapsedEventArgs e)
    //        {
    //            //t.Stop(); //先关闭定时器
    //            //Form1.ShutDown();
    //            //t.Start(); //执行完毕后再开启器
    //            //text_wendu.Text
    //        }
    //    }
    //}
    public class string_name
    {
        public string string_my;
    }



    public class Rootobject
    {
        public string resource { get; set; }
        public string _event { get; set; }
        public string event_time { get; set; }
        public Notify_Data notify_data { get; set; }
    }


    public class Notify_Data
    {
        public Header header { get; set; }
        public Body body { get; set; }
    }

    public class Header
    {
        public string app_id { get; set; }
        public string device_id { get; set; }
        public string node_id { get; set; }
        public string product_id { get; set; }
        public string gateway_id { get; set; }
    }

    public class Body
    {
        public Service[] services { get; set; }
    }

    public class Service
    {
        public string service_id { get; set; }
        public Properties1 properties { get; set; }
        public string event_time { get; set; }
    }

    public class Properties1
    {
        public int wendu { get; set; }
        public int shidu { get; set; }
        public int device_number { get; set; }
        public Properties1(int Device_number,int Wendu, int Shidu)
        {
            this.shidu = Shidu;
            this.wendu = Wendu;
            this.device_number = Device_number;
        }
    }
}
