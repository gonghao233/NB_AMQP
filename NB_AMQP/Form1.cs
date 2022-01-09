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

                humidity_series.ChartType = SeriesChartType.Line;
                humidity_series.XValueMember = "event_time";
                humidity_series.YValueMembers = "shidu";

                chart1.Series.Add(temperature_series);
                chart1.Series.Add(humidity_series);
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
            List<int> My_list=sqlhelper.Select_lastest_info( number);
            if(My_list!=null)
            { 
            text_wendu.Text = My_list[0].ToString();
            text_shidu.Text = My_list[1].ToString();
                //text_wendu.Text= "1";
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

                humidity_series.ChartType = SeriesChartType.Line;
                humidity_series.XValueMember = "event_time";
                humidity_series.YValueMembers = "shidu";

                chart1.Series.Add(temperature_series);
                chart1.Series.Add(humidity_series);
            }
        }
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
