using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using DAL;

namespace DAL
{
    public class SQLhelper : CommstaticClass
    {
        public SqlConnection creatconnection()
        //private SqlConnection creatconnection()  12.20改成public
        {
            return new SqlConnection(ConnectionStrings);
        }
        public SqlConnection creatconnection_creatdb()
        {
            return new SqlConnection(ConnectionString_CreatDB);
        }
        public  void UP_INFO(UP_info uP_Info)
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendLine("INSERT INTO CESHI");
            stringBuilder.AppendLine("VALUES(@device_number,@wendu,@shidu,@device_id,@event_time)");
            SqlParameter[] sqlParameters =
            {
                new SqlParameter("device_number",uP_Info.device_number),
                new SqlParameter("wendu",uP_Info.wendu),
                new SqlParameter("shidu",uP_Info.shidu),
                new SqlParameter("event_time",uP_Info.event_time),
                new SqlParameter("device_id",uP_Info.device_id)
            };
            int irows=ExecuteNonQuery(stringBuilder.ToString(), sqlParameters);

            //MessageBox.Show("受影响行数：" + irows);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="my_device_number"></param>
        /// <returns></returns>
        public List<int> Select_lastest_info(int my_device_number)
        {

            using (SqlConnection connection = creatconnection())
            {
                connection.Open();
                StringBuilder stringBuilder = new StringBuilder();
                stringBuilder.AppendLine("SELECT *FROM CESHI WHERE device_number=@number order by event_time DESC;");
                SqlParameter[] sqlParameters =
                {
                //new SqlParameter("device_number",uP_Info.device_number),
                //new SqlParameter("event_time",uP_Info.event_time),
                new SqlParameter("number",my_device_number)
                };
                SqlCommand cmd = new SqlCommand(stringBuilder.ToString(), connection);
                cmd.Parameters.AddRange(sqlParameters);
                //cmd.Parameters.Clear();
                SqlDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    int id = reader.GetInt32(0);
                    int temperature = reader.GetInt32(1);
                    int huminidity = reader.GetInt32(2);
                    //MessageBox.Show("温度：" + temperature);
                    List<int> my_list = new List<int>();
                    my_list.Add(temperature);
                    my_list.Add(huminidity);
                    cmd.Parameters.Clear();
                    return my_list;
                }
                else
                {
                    return null;
                }
            }   

        }
 /// <summary>
 /// 
 /// </summary>
 /// <returns></returns>
            public List<UP_info> select()
            {
                using (SqlConnection connection = creatconnection())
                {
                List<UP_info> list = null;
                connection.Open();
                SqlCommand cmd = connection.CreateCommand();
                cmd.CommandText = "SELECT *FROM CESHI WHERE device_number=1 order by event_time ASC;";
                SqlDataReader dr = cmd.ExecuteReader();
                if (dr.HasRows)
                {
                    list = new List<UP_info>();
                    while (dr.Read())
                    {
                        UP_info data = new UP_info();
                        data = new UP_info();
                        data.event_time = dr["event_time"].ToString();
                        data.shidu = dr.GetInt32(2);
                        data.wendu = dr.GetInt32(1);
                        list.Add(data);
                    }
                }
                     return list;
                 }
            }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="my_device_number"></param>
        /// <returns></returns>
        public List<Temp_Hum_Time> Select_one_device_info(int my_device_number)
        {
            List<Temp_Hum_Time> list = null;
            using (SqlConnection connection = creatconnection())
            {   
                connection.Open();
                StringBuilder stringBuilder = new StringBuilder();
                stringBuilder.AppendLine("SELECT *FROM CESHI WHERE device_number=@number order by event_time ASC;");
                SqlParameter[] sqlParameters =
                {
                //new SqlParameter("device_number",uP_Info.device_number),
                //new SqlParameter("event_time",uP_Info.event_time),
                new SqlParameter("number",my_device_number)
                };
                SqlCommand cmd = new SqlCommand(stringBuilder.ToString(), connection);
                cmd.Parameters.AddRange(sqlParameters);
                //cmd.Parameters.Clear();
                SqlDataReader reader = cmd.ExecuteReader();
                if (reader.HasRows)
                {
                    list = new List<Temp_Hum_Time>();
                    while(reader.Read())
                    {
                        Temp_Hum_Time temp_hum_time = new Temp_Hum_Time();
                        int id = reader.GetInt32(0);
                        temp_hum_time.temperature = reader.GetInt32(1);
                        temp_hum_time.humidity = reader.GetInt32(2);
                        temp_hum_time.event_time = reader.GetString(3);
                        list.Add(temp_hum_time);
                    }
                }

                cmd.Parameters.Clear();
                return list;
            }

        }

        public int ExecuteNonQuery(string cmdcxt, params SqlParameter[] sqlParameters)
        {
            using (SqlConnection connection = creatconnection())
            {
                connection.Open();
                SqlCommand cmd = new SqlCommand(cmdcxt, connection);
                cmd.Parameters.AddRange(sqlParameters);
                int iRows = cmd.ExecuteNonQuery();
                cmd.Parameters.Clear();
                return iRows;
            }
        }

        public void Create_DB(string database_name,string table_name)
        {
            using (SqlConnection connection = creatconnection_creatdb())
            {
                connection.Open();
                String str = "IF(NOT EXISTS(SELECT * FROM master.dbo.sysdatabases WHERE dbid=db_ID('"+ database_name + "'))) CREATE DATABASE "+ database_name;
                //SqlParameter[] sqlParameters =
                //{
                //new SqlParameter("@DB_NAME",database_name)
                //};
                SqlCommand cmd1 = new SqlCommand(str, connection);
                //cmd1.Parameters.AddRange(sqlParameters);
                cmd1.ExecuteNonQuery();

                str = "SELECT database_id from sys.databases WHERE Name ='" + database_name+"'";
                SqlCommand cmd3 = new SqlCommand(str, connection);
                while (true)
                {
                    object resultObj = cmd3.ExecuteScalar();
                    if (resultObj != null)
                    {
                        //MessageBox.Show("已创建数据库！", "创建数据库");
                        break;
                    }
                }


                str = "USE "+ database_name+" if not exists(select * from sysobjects where id = OBJECT_ID('" + database_name + ".dbo."+ table_name + "'))" +
                    "create table "+ table_name + "(device_number int not null,wendu int not null,shidu int not null,device_id char(40)  null,event_time char(40)   null)";
                //SqlParameter[] sqlParameters_ =
                //{
                //new SqlParameter("@DB_NAME",database_name),
                //new SqlParameter("@Table_NAME",table_name)
                //};
                SqlCommand cmd2 = new SqlCommand(str, connection);
                //cmd2.Parameters.AddRange(sqlParameters_);
                cmd2.ExecuteNonQuery();
            }
        }


    }
}
