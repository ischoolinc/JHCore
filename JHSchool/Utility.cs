using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using FISCA.DSAClient;
using System.Net;
using System.IO;

namespace StudentChangeStatus_KH
{
    public class Utility
    {
        /// <summary>
        /// 傳送資料到局端
        /// </summary>
        /// <param name="ClassID"></param>
        /// <param name="Action"></param>
        /// <returns></returns>
        public static string SendData(string Action,string ClassName,string StudentName,string StudentNumber,string IDNumber, string StudentStatus, string NewStudentStatus,string Msg)
        {
            string DSNS = FISCA.Authentication.DSAServices.AccessPoint;
            string AccessPoint = @"j.kh.edu.tw";

            if (FISCA.RTContext.IsDiagMode)
            {
                string accPoint = FISCA.RTContext.GetConstant("KH_AccessPoint");
                if (!string.IsNullOrEmpty(accPoint))
                    AccessPoint = accPoint;
            }

            string Contract = "log";
            string ServiceName = "_.InsertLog";

            string errMsg = "";
            try
            {
                {
                    XElement xmlRoot = new XElement("Request");
                    XElement s1 = new XElement("SchoolLog");
                    XElement s2 = new XElement("Field");
                    s2.SetElementValue("DSNS", DSNS);
                    s2.SetElementValue("Action", Action);
                    XElement s3 = new XElement("Content");
                    s3.SetElementValue("ClassName", ClassName);
                    s3.SetElementValue("StudentName", StudentName);
                    s3.SetElementValue("StudentNumber", StudentNumber);
                    s3.SetElementValue("IDNumber", IDNumber);
                    s3.SetElementValue("StudentStatus", StudentStatus);
                    s3.SetElementValue("NewStudentStatus", NewStudentStatus);
                    s3.SetElementValue("Reason", Msg); // 備註
                    s2.Add(s3);
                    s1.Add(s2);
                    xmlRoot.Add(s1);

                    XmlHelper reqXML = new XmlHelper(xmlRoot.ToString());
                    FISCA.DSAClient.Connection cn = new FISCA.DSAClient.Connection();
                    cn.Connect(AccessPoint, Contract, DSNS, DSNS);
                    Envelope rsp = cn.SendRequest(ServiceName, new Envelope(reqXML));
                    XElement rspXML = XElement.Parse(rsp.XmlString);                
                }
                
                //2017/6/7 穎驊新增 高雄項目 [03-01][03] 巨耀局端介接學生資料欄位 巨耀自動編班 更新Service                
                try
                {
                    string urlString = "http://163.32.129.9/khdc/ito";
                    // 準備 Http request
                    HttpWebRequest req = (HttpWebRequest)HttpWebRequest.Create(urlString);
                    req.Method = "POST";

                    // 呼叫並取得結果
                    HttpWebResponse rsp;
                    rsp = (HttpWebResponse)req.GetResponse();

                    Stream dataStream = rsp.GetResponseStream();
                    StreamReader reader = new StreamReader(dataStream);

                    string response = reader.ReadToEnd(); //檢查使用，若成功回傳，response 值為 "00"

                    reader.Close();
                    dataStream.Close();
                    rsp.Close();
                }
                catch (Exception e)
                {

                }

            }
            catch (Exception ex)
            {

                errMsg = ex.Message;
            }
            return errMsg;
        }

    }
}
