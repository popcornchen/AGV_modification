using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data;
using MySql.Data.MySqlClient;
using System.Data.SqlClient;

namespace demo
{
    class test
    {
        public static string constr = "server=192.168.3.6; user=root; database=test; port=3306; pwd=angel070711";
        public static Thread Newseq;
        
        /*-----------------测试数据库连接函数--------------------*/
        public void conDB()
        {
            MySqlConnection con = new MySqlConnection(constr);
            try
            {
                con.Open();
                Console.WriteLine("connection successful");
                con.Close();
            }
            catch (Exception)
            {
                Console.WriteLine("connection error");
            }
        }

        /*-----------------随机更新数据库信息--------------------*/
        public void update()
        {
            Newseq = new Thread(delegate()
            {
                MySqlConnection con = new MySqlConnection(constr);
                con.Open();
                string command;
                string setflag;
                //随机数形式上传station，实际读网页输入sequence
                for (int i = 1; i<10; i++)
                {
                    Random rd = new Random();
                    int j = rd.Next(1,9);
                    command = "Update agv_sequence set station =" + j + " where sequence=" + i;
                    MySqlCommand cmd = new MySqlCommand(command, con);
                    cmd.ExecuteNonQuery();
                }
                setflag = "Update upload_monitor set processed=0 where line=0";
                MySqlCommand changeflag = new MySqlCommand(setflag, con);
                changeflag.ExecuteNonQuery();
                Console.WriteLine("修改完成");
                con.Close();
            });
            Newseq.Start();
            System.Threading.Thread.Sleep(500);
            Newseq.Abort();
        }
    }
}




namespace AGVmodification
{
    class Program
    {
        static void Main(string[] args)
        {
            demo.test modify = new demo.test();
            modify.conDB();
            DateTime startTime = DateTime.Now;
            while (true)
            {
                modify.update();
                System.Threading.Thread.Sleep(5000);
                Application.DoEvents();
                if (DateTime.Now - startTime > TimeSpan.FromMinutes(3)) break;
            }
            Console.ReadLine();
        }
    }
}
