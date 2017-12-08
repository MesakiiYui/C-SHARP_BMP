using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace parsertest
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            String mykey = textBox1.Text;
            myparser(mykey);

        }
        //解析函数的入口，将字符串分割，反转，分派
        public void myparser(String mykey)
        {
            //mykey 示例 555534008D7C6B5AD041CCCC
            //string a = ReverseString(mykey);
            //textBox2.Text = a;
            //示例 addr+data=crccheck= 3400 8D7C6B5A
            string crccheck = (mykey.Substring(4, 12));

            //起始位start=55 55 
            string strstart = mykey.Substring(0, 4);
            //地址位反转straddr= 0034
            string straddr = parsertest.Utils.Utils.ReverseString(mykey.Substring(4, 4));
            //数据位反转strdata= 5A6B7C8D
            string strdata = parsertest.Utils.Utils.ReverseString(mykey.Substring(8, 8));
            //crc校验未反转 crc= D041
            string strcrc = mykey.Substring(16, 4);
            //结束位 end=CCCC
            string strend =mykey.Substring(20, 4);

            //textBox2.Text += strstart;
            //textBox2.Text += straddr;
            //textBox2.Text += strdata;
            //textBox2.Text += strcrc;
            //textBox2.Text += strend;

            //接收到数据后进行crc校验
            //转化为byte数组
            byte[] databyte = parsertest.Utils.Utils.hexString2Bytes(crccheck);
            //Console.WriteLine("用于校验string   " + crccheck);
            //Console.WriteLine("用于校验byte    " + BitConverter.ToString(databyte));
            //交给crc16进行校验
            crc.CRC16Util myutil = new crc.CRC16Util();
            //通过crc16计算得到byte[]型的crcanswer
            byte[] crcanswer = myutil.CalculateCrc16(databyte);
            //将它转化为16进制的string
            string strcrcanswerrever = parsertest.Utils.Utils.byte2string(crcanswer);
            //最后将其反转（）为strcrcanswer = D041
            string strcrcanswer = parsertest.Utils.Utils.ReverseString(strcrcanswerrever);
            textBox3.Text += strcrcanswer;
            // crc对比
            Console.WriteLine("计算得到   " + strcrcanswer);
            if (strcrcanswer== strcrc)
            {
                textBox3.Text += "crc匹配成功";
                //交给addrpaser进行任务分流
                addrparser(straddr,strdata);
            }
            else
            {
               Console.WriteLine("匹配失败");
            }
        
            
            
         
           

        }

        //解析addr位，根据地址位内容的不同进行switch-case任务分流
        public void  addrparser(string straddr,string strdata)
        {
            Dictionary<string, string> feedback = new Dictionary<string, string>();
            switch (straddr)
            {
                
                //地址序号（只读01）
                case "0100":
                    feedback = FeedbackPaser0100(strdata);
                    break;
                case "0104":
                    feedback = FeedbackPaser0104(strdata);
                    break;

                default:
                    Console.WriteLine("格式错误");
                    break;
            }

            //开启数据库连接,插入数据库
            Utils.SQLiteDBHelper sqlconn = new Utils.SQLiteDBHelper();
            foreach (KeyValuePair<string, string> kv in feedback)
            {
                //将内容插入数据库
                Console.WriteLine("键为：{0}，值为：{1}", kv.Key, kv.Value);
                //生成时间戳
                string timestamp = parsertest.Utils.Utils.GenerateTimeStamp();
                string sql = "insert into tablename(key,value,timestamp) values (" + kv.Key + "," + kv.Value + "," + timestamp + ")";
                sqlconn.ExecuteNonQuery(sql);

            }

        }

        

        //解析data位，strdata已经经过了反转
        //产生键值对
              
        private Dictionary<string, string> FeedbackPaser0100(string strdata)
        {
            Dictionary<string, string> feedbackdic = new Dictionary<string, string>();
            //最大放电功率
            string maxdischargepower16 = strdata.Substring(0, 4);
            //功率控制方式
            string strpowercontrol = strdata.Substring(4, 2);
            //PCS工作模式
            string strpcsworkmodel = strdata.Substring(6, 2);
            //计算最大放电功率
            //将一个16进制的string转化为10进制的string
            string maxdischargepower = int.Parse(maxdischargepower16, System.Globalization.NumberStyles.AllowHexSpecifier).ToString();
            feedbackdic.Add("maxdischargepower", maxdischargepower);
            feedbackdic.Add("strpowercontrol",strpowercontrol);
            feedbackdic.Add("strpcsworkmodel", strpcsworkmodel);
            return feedbackdic;
        }
        private Dictionary<string, string> FeedbackPaser0104(string strdata)
        {
            Dictionary<string, string> feedbackdic = new Dictionary<string, string>();
            //无功功率
            string reactivepower16 = strdata.Substring(0, 4);
            string reactivepower = int.Parse(reactivepower16, System.Globalization.NumberStyles.AllowHexSpecifier).ToString();
            feedbackdic.Add("reactivepower", reactivepower);
            //最大放电功率
            string maxidischargepower16 = strdata.Substring(4, 4);
            string maxidischargepower = int.Parse(maxidischargepower16, System.Globalization.NumberStyles.AllowHexSpecifier).ToString();
            feedbackdic.Add("maxidischargepower", maxidischargepower);

            return feedbackdic;
        }
        private Dictionary<string, string> FeedbackPaser4(string strdata)
        {
            Dictionary<string, string> feedbackdic = new Dictionary<string, string>();
            return feedbackdic;
        }
       
        
       


    }
}
