using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using uPLibrary.Networking.M2Mqtt;
using uPLibrary.Networking.M2Mqtt.Messages;


namespace DevicesubApp
{
    public partial class Frmmain : Form
    {
        MqttClient client;
        string connectionString; //DB연결문자열 || MQTT Broker address
        ulong lineCount;
        delegate void UpdateTextCallback(string message);//스레드상 윈폼 richtextbox텍스트 출력 필요

        Stopwatch sw = new Stopwatch();
        public Frmmain()
        {
            InitializeComponent();
            InitializeAllData();
        }

        //값을 초기화 
        private void InitializeAllData() 
        {
            connectionString = "Data Source=210.119.12.88;Initial Catalog=MRP;" +
                "User ID=sa; password=mssql_p@ssw0rd!";
            lineCount = 0;
            Btnconnect.Enabled = true;
            BtnDisconnect.Enabled = false;
            IPAddress brokerAddress;
            try
            {
                brokerAddress = IPAddress.Parse(TxtConnectionString.Text);//형변환 해줘야 값을 받을 수 있다 
                client = new MqttClient(brokerAddress);
                client.MqttMsgPublishReceived += Client_MqttMsgPublishReceived;
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.ToString());
            }
            Timer.Enabled = true;
            Timer.Interval = 1000; // 1000ms --> 1sec
            Timer.Tick += Timer_Tick;
            Timer.Start();
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            LblResult.Text = sw.Elapsed.Seconds.ToString(); //시간이 지난것을 출력해준다
            if(sw.Elapsed.Seconds >= 2) //데이터 들어오고 2초지나면 처리!!
            {
                sw.Stop();
                sw.Reset();
                //TODO 실제 처리 프로세스 실행
                // UpdateText("처리 !!");
                PrcCorrectDataToDB();
                
                
                //ClearData(); ->전역에 있는 것 다 지우는

            }
        }

        //여러 데이터중 최종 데이터만 Db에 입력처리 메서드
        private void PrcCorrectDataToDB()
        {
            if(iotData.Count > 0)
            {
                var correctData = iotData[iotData.Count - 1];
                //Db입력
                // UpdateText("DB처리");
                if (correctData["PRC_MSG"] == "OK" || correctData["PRC_MSG"] == "FAIL")
                {
                    using (var conn = new SqlConnection(connectionString))
                    {
                        var prcResult = correctData["PRC_MSG"] == "OK" ? 1 : 0;
                        string strUpQry = $"UPDATE Process " +
                                          $" SET PrcResult = '{prcResult}' " +
                                          $"     ,ModDate = '{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") }' " +
                                          $"     ,ModID = '{"SYS"}' " +
                                          $"WHERE PrcIdx = " +
                                          $"(SELECT TOP 1 PrcIdx FROM Process ORDER BY PrcIdx DESC)";

                        try
                        {
                            conn.Open();
                            SqlCommand cmd = new SqlCommand(strUpQry, conn);
                            if (cmd.ExecuteNonQuery() == 1)
                                UpdateText("[DB] 센싱값 Update 성공");
                            else
                                UpdateText("[DB] 센싱값 Update 실패");
                        }
                        catch (Exception ex)
                        {
                            UpdateText($">>>>> DB ERROR !! : { ex.Message}");
                        }
                    }

                }
                              

                //JObject result = new JObject();
                //result.Add("PRC_MSG", correctData["PRC_MSG"]);

                //client.Publish("factory1/monitor/data", )
            }

            iotData.Clear(); //데이터 모두 삭제
        }

        private void Client_MqttMsgPublishReceived(object sender, MqttMsgPublishEventArgs e)
        {
            try
            {
                var message = Encoding.UTF8.GetString(e.Message);
                UpdateText($">>>>>받은메세지 : {message}");
                //message(json) > C#
                var currentData = JsonConvert.DeserializeObject<Dictionary<string, string>>(message);

                
                PrcInputDataToList(currentData);

                //timer_tick에서 메세지를 출력한다
                sw.Stop();
                sw.Reset();
                sw.Start();
                
            }
            catch (Exception ex)
            {
               UpdateText($">>>>> ERROR!!: {ex.Message}");
            }
        }

        List<Dictionary<string, string>> iotData= new List<Dictionary<string, string>>();

        //라즈베리에서 들어온 메세지를 전역리스트에다가 입력하는 메서드
        private void PrcInputDataToList(Dictionary<string, string> currentData)
        {
            if(currentData["PRC_MSG"] != "OK" || currentData["PRC_MSG"] != "FAIL")
            iotData.Add(currentData);
        }

        private void Btnconnect_Click(object sender, EventArgs e)
        {
            client.Connect(TxtClientID.Text); //SUBSCR01 이름으로 connect
            RtbSubscr.AppendText(" >>>>> Client Connected \n");
            client.Subscribe(new string[] { TxtSubscriptionTopic.Text },
                new byte[] { MqttMsgBase.QOS_LEVEL_AT_MOST_ONCE });
            RtbSubscr.AppendText(" >>>>> Subscribing to : " + TxtSubscriptionTopic.Text);

            Btnconnect.Enabled = false;
            BtnDisconnect.Enabled = true;


                                              
        }

        private void BtnDisconnect_Click(object sender, EventArgs e)
        {
            client.Disconnect();
            RtbSubscr.AppendText(">>>>> Client disconnected !!");

            Btnconnect.Enabled = true;
            BtnDisconnect.Enabled = false;
        }

        private void UpdateText(string message)
        {
            if(RtbSubscr.InvokeRequired)
            {
                UpdateTextCallback callback = new UpdateTextCallback(UpdateText);
                this.Invoke(callback, new object[] { message });
            }
            else
            {
                lineCount++;
                RtbSubscr.AppendText($"{lineCount} : {message}\n");
                RtbSubscr.ScrollToCaret();//화면 찼을때 스크롤 생성후 내려가기
            }
        }
    }
}
