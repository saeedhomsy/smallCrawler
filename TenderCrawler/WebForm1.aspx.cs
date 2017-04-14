using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;

namespace TenderCrawler
{
    public partial class WebForm1 : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            getDat();
        }


        private void getDat()
        {
            DataTable DT = new DataTable();
            int OrderCounter = 1;
            DT.Columns.Add("Order");
            DT.Columns.Add("Classify");
            DT.Columns.Add("Link");
            DataRow[] drArr = new DataRow[10];
            int mcCollCount = 10;
            int pageNum = 10;
            string pageChar = "n";
            while (mcCollCount != 0)
            {
                string link = "http://www.medicaltenders.com/search.php?total=141&off=" + pageNum.ToString() + "&inc=" + pageChar + "n&global=1&region_name[]=EG&notice_type_new[]=1,2,3,7,10,11,16,9,4,8,5&sector=18&deadline=";
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(link);
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                StreamReader sr = new StreamReader(response.GetResponseStream());
                string resText = sr.ReadToEnd();
                Regex r = new Regex(@"<td class=""style5"" style=""width:70%; vertical-align:top; text-align:left;"" >.*?</td>");
                Regex tendLinkReg = new Regex(@"href=""auth.*?""");
                MatchCollection ms = r.Matches(resText);
                MatchCollection mas = tendLinkReg.Matches(resText);
                mcCollCount = ms.Count;
                var list = ms.Cast<Match>().Select(match => match.Value).ToList();
                var newlist = mas.Cast<Match>().Select(match => match.Value).ToList();
                for (int i = 0; i < list.Count; i++)
                {
                    DataRow row = DT.NewRow();
                    row["Order"] = OrderCounter;

                    //####################################
                    //here can get all key words for our claaify and get it from site category
                    //####################################
                    row["Classify"] = (list[i].IndexOf("Pharmaceuticals") > -1 ? "Pharmaceutical" : list[i].IndexOf("Healthcare Equipment and Services") > -1 ? "Medical Equipment" : list[i].IndexOf("Laboratory Equipment and Services") > -1 ? "Laboratory Product" : "Other");
                    row["Link"] = "http://www.medicaltenders.com/" + newlist[i].Substring(6, newlist[i].Length - 1 - 6);
                    DT.Rows.Add(row);
                    OrderCounter++;
                }
                pageNum += 10;
                pageChar = "y";
            }
            GridView1.DataSource = DT;
            GridView1.DataBind();
        }
    }
}