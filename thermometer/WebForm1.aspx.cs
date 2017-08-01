using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Net;
using System.Web.Script.Serialization;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;

namespace thermometer
{
    public partial class WebForm1 : System.Web.UI.Page{
        string degrees;
        string prevTemp;
        
        SqlConnection con = new SqlConnection(@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\ceng2\documents\visual studio 2015\Projects\thermometer\thermometer\App_Data\threshold.mdf;Integrated Security = True");

        //**************************************************************************************
        //* Page_Load Event
        //**************************************************************************************
        protected void Page_Load(object sender, EventArgs e){
            if (!IsPostBack)
            {
                //Initialize global variables
                degrees = "C";
                hfTemp.Value = "0";
                prevTemp = "0";

                //testing value
                //testTemperature.Value = "22";

                //Get the first reading for the thermometer
                thermometer();

                //Build the GridView1 table
                DefaultThresholdRecord();
            }
            else
            {
                //Do something
            }
        }
        //**************************************************************************************
        //*
        //**************************************************************************************
        protected void Timer1_Tick(object sender, EventArgs e){
            //get the value of hidden fields
            degrees = hfDegrees.Value;
            try {
                prevTemp = hfTemp.Value;
            } catch {

            }
            //update the thermometer
            thermometer();
        }
        //**************************************************************************************
        //*
        //**************************************************************************************
        protected void thermometer(){
            string appId = "1aef22a6bbff9222935c340baa7ccca3";
            string url = "";

            if (degrees == "C"){
                url = string.Format("http://api.openweathermap.org/data/2.5/weather?q={0}&units=metric&cnt=1&APPID={1}", "Vancouver, Canada", appId);
            }else{
                url = string.Format("http://api.openweathermap.org/data/2.5/weather?q={0}&units=imperial&cnt=1&APPID={1}", "Vancouver, Canada", appId);
            }

            using (WebClient client = new WebClient())
            {
                string json = client.DownloadString(url);
                WeatherInfo weatherInfo = (new JavaScriptSerializer()).Deserialize<WeatherInfo>(json);
                if (degrees == "C")
                {
                    lblTemp.Text = string.Format("{0}°С", Math.Round(weatherInfo.main.temp, 1));
                    hfDegrees.Value = degrees;
                    hfTemp.Value = string.Format("{0}", Math.Round(weatherInfo.main.temp, 1));
                }
                else
                {
                    lblTemp.Text = string.Format("{0}°F", Math.Round(weatherInfo.main.temp, 1));
                    hfDegrees.Value = degrees;
                    hfTemp.Value = string.Format("{0}", Math.Round(weatherInfo.main.temp, 1));
                }
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "tmp", "<script type='text/javascript'>initialize();</script>", false);
                soundAlert();

            }
        }
        //**************************************************************************************
        //*
        //**************************************************************************************
        public class WeatherInfo
        {
            public Main main { get; set; }
        }
        //**************************************************************************************
        //*
        //**************************************************************************************
        public class Main
        {
            public double temp { get; set; }
        }
        //**************************************************************************************
        //*
        //**************************************************************************************
        protected void Button1_Click(object sender, EventArgs e)
        {
            degrees = "C";
            Button1.Style.Add("background-color", "#73d5ef");
            Button2.Style.Add("background-color", "#CCC");

            TextBox i_C = (TextBox)GridView1.FooterRow.FindControl("i_C");
            TextBox i_F = (TextBox)GridView1.FooterRow.FindControl("i_F");

            i_C.Enabled = true;
            i_F.Enabled = false;

            thermometer();
        }
        //**************************************************************************************
        //*
        //**************************************************************************************
        protected void Button2_Click(object sender, EventArgs e)
        {
            degrees = "F";
            Button1.Style.Add("background-color", "#CCC");
            Button2.Style.Add("background-color", "#73d5ef");

            TextBox i_C = (TextBox)GridView1.FooterRow.FindControl("i_C");
            TextBox i_F = (TextBox)GridView1.FooterRow.FindControl("i_F");

            i_C.Enabled = false;
            i_F.Enabled = true;

            thermometer();
        }
        //**************************************************************************************
        //*
        //**************************************************************************************
        public void DefaultThresholdRecord()
        {

            SqlCommand cmd = new SqlCommand("select * from THRESHOLD", con);
            SqlDataAdapter da = new SqlDataAdapter(cmd);

            DataSet ds = new DataSet();
            da.Fill(ds);
            int count = ds.Tables[0].Rows.Count;
            if (ds.Tables[0].Rows.Count > 0)
            {
                GridView1.DataSource = ds;
                GridView1.DataBind();
            }else {
                ds.Tables[0].Rows.Add(ds.Tables[0].NewRow());
                GridView1.DataSource = ds;
                GridView1.DataBind();
                int columncount = GridView1.Rows[0].Cells.Count;
                lblmsg.Text = " No data found !!!";
            }
        }
        //**************************************************************************************
        //*
        //**************************************************************************************
        protected void GridView1_RowEditing(object sender, GridViewEditEventArgs e)
        {
            GridView1.EditIndex = e.NewEditIndex;
            DefaultThresholdRecord();
        }
        //**************************************************************************************
        //*
        //**************************************************************************************
        protected void GridView1_RowUpdating(object sender, GridViewUpdateEventArgs e)
        {
            int id = Convert.ToInt16(GridView1.DataKeys[e.RowIndex].Values["id"].ToString());
            TextBox txtCel = GridView1.Rows[e.RowIndex].FindControl("TextBox2") as TextBox;
            TextBox txtFar = GridView1.Rows[e.RowIndex].FindControl("TextBox3") as TextBox;

            DropDownList drp_D = GridView1.Rows[e.RowIndex].FindControl("DropDownList2") as DropDownList;
            DropDownList drp_NL = GridView1.Rows[e.RowIndex].FindControl("DropDownList3") as DropDownList;
            DropDownList drp_A = GridView1.Rows[e.RowIndex].FindControl("DropDownList4") as DropDownList;

            if (hfDegrees.Value == "C")
            {
                txtCel.Enabled = true;
                txtFar.Enabled = false;
            }
            else {
                txtCel.Enabled = false;
                txtFar.Enabled = true;
            }

            SqlCommand cmd = new SqlCommand("update THRESHOLD set threshold_C=@threshold_C, threshold_F=@threshold_F,threshold_D=@threshold_D,threshold_NL=@threshold_NL,threshold_A=@threshold_A where id =@id", con);
            cmd.Parameters.AddWithValue("@threshold_C", txtCel.Text);
            cmd.Parameters.AddWithValue("@threshold_F", txtCel.Text);

            cmd.Parameters.AddWithValue("@threshold_D", drp_D.SelectedItem.Text);
            cmd.Parameters.AddWithValue("@threshold_NL", drp_NL.SelectedItem.Text);
            cmd.Parameters.AddWithValue("@threshold_A", drp_A.SelectedItem.Text);

            cmd.Parameters.AddWithValue("@id", id);
            con.Open();
            cmd.ExecuteNonQuery();
            con.Close();

            lblmsg.BackColor = Color.Blue;
            lblmsg.ForeColor = Color.White;
            lblmsg.Text = id + "        Updated successfully........    ";
            GridView1.EditIndex = -1;

            DefaultThresholdRecord();
        }
        //**************************************************************************************
        //*
        //**************************************************************************************
        protected void GridView1_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
        {
            GridView1.EditIndex = -1;
            DefaultThresholdRecord();
        }
        //**************************************************************************************
        //*
        //**************************************************************************************
        protected void GridView1_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            int id = Convert.ToInt16(GridView1.DataKeys[e.RowIndex].Values["id"].ToString());
            con.Open();
            SqlCommand cmd = new SqlCommand("delete from THRESHOLD where id = @id", con);
            cmd.Parameters.AddWithValue("@id", id);            
            int result = cmd.ExecuteNonQuery();
            con.Close();
            if (result == 1)
            {
                DefaultThresholdRecord();
                lblmsg.BackColor = Color.Red;
                lblmsg.ForeColor = Color.White;
                lblmsg.Text = id + "      Deleted successfully.......    ";
           }         
        }
        //**************************************************************************************
        //*
        //**************************************************************************************
        protected void GridView1_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                string id = Convert.ToString(DataBinder.Eval(e.Row.DataItem, "id"));
                Button lnkbtnresult = (Button)e.Row.FindControl("ButtonDelete");
                if (lnkbtnresult != null)
                {
                    lnkbtnresult.Attributes.Add("onclick", "javascript:return deleteConfirm('" + id + "')");
                }
            }
        }
        //**************************************************************************************
        //*
        //**************************************************************************************
        protected void GridView1_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            
            if (e.CommandName.Equals("AddNew"))
            {

                TextBox i_id = (TextBox)GridView1.FooterRow.FindControl("i_id");
                TextBox i_C = (TextBox)GridView1.FooterRow.FindControl("i_C");
                TextBox i_F = (TextBox)GridView1.FooterRow.FindControl("i_F");

                DropDownList i_D = (DropDownList)GridView1.FooterRow.FindControl("i_D");
                DropDownList i_NL = (DropDownList)GridView1.FooterRow.FindControl("i_NL");
                DropDownList i_A = (DropDownList)GridView1.FooterRow.FindControl("i_A");

                con.Open();
                SqlCommand cmd = new SqlCommand("insert into threshold(threshold_C,threshold_F,threshold_D,threshold_NL,threshold_A) values('" + i_C.Text + "','" + i_F.Text + "','" + i_D.Text + "','" + i_NL.Text + "','" + i_A.Text + "')", con);

                int result = cmd.ExecuteNonQuery();
                con.Close();
                if (result == 1)
                {
                    DefaultThresholdRecord();
                    lblmsg.BackColor = Color.Green;
                    lblmsg.ForeColor = Color.White;
                    lblmsg.Text = "      Row added successfully......    ";
                }
                else
                {
                    lblmsg.BackColor = Color.Red;
                    lblmsg.ForeColor = Color.White;
                    lblmsg.Text = i_id.Text + " Error while adding row.....";
                }
            }
        }

        //**************************************************************************************
        //*
        //**************************************************************************************
        private void soundAlert()
        {
            bool test1 = false;
            bool test2 = false;
            bool test3 = false;
            bool test4 = false;
            bool test5 = false;

            string id;
            string testTemp = hfTemp.Value;

            //Test value
            //string testTemp = testTemperature.Value;
            
            SqlCommand cmd = new SqlCommand("select * from THRESHOLD", con);
            SqlDataAdapter da = new SqlDataAdapter(cmd);

            DataTable dtCurrentTable = new DataTable();
            da.Fill(dtCurrentTable);
            
            if (dtCurrentTable.Rows.Count > 0){

                foreach (DataRow dr in dtCurrentTable.Rows){
                    ///get the id of the row
                    id = dr["id"].ToString();

                    //Degrees Celcius
                    if (dr["threshold_C"].ToString() == testTemp){ test1 = true; }
                    //Degress Fahrenheit
                    if (dr["threshold_F"].ToString() == testTemp) { test2 = true; }
                    //Direction UD, UP, DOWN
                    if (dr["threshold_D"].ToString() == "UP/DOWN") {
                          test3 = true;
                    } else if ((dr["threshold_D"].ToString() == "UP") && (Convert.ToDouble(prevTemp) < Convert.ToDouble(testTemp))){
                        test3 = true;
                    }
                    else if ((dr["threshold_D"].ToString() == "DOWN") && (Convert.ToDouble(prevTemp) > Convert.ToDouble(testTemp))){
                        test3 = true;
                    }
                    //Direction
                    if ((dr["threshold_NL"].ToString() == "ALL NOTIFICATIONS") && (Convert.ToDouble(prevTemp) != Convert.ToDouble(testTemp))) {
                        test4 = true;
                    } else if ((dr["threshold_NL"].ToString() == "SINGLE") && (Math.Abs(Convert.ToDouble(prevTemp) - Convert.ToDouble(testTemp)) > 0.5)){
                        test4 = true;                        
                    }
                    //Alarm NOT ACTIVATED, ACTIVATED
                    if (dr["threshold_A"].ToString() == "ACTIVE")
                    {
                        test5 = true;
                    }
                    if (((test1 == true) || (test2 == true)) && (test3 == true) && (test4 == true) && (test5 == true))
                    {
                        //update the database for the row.  set table value ACTiVE to SOUNDED
                        SqlCommand cmda = new SqlCommand("update THRESHOLD set threshold_A='SOUNDED' where id ='" + id + "'", con);
                        con.Open();
                        cmda.ExecuteNonQuery();
                        con.Close();

                        test1 = false;
                        test2 = false;
                        test3 = false;
                        test4 = false;
                        test5 = false;

                        DefaultThresholdRecord();

                        //Activate popup
                        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "alert", "<script type='text/javascript'>giveNotification(" + testTemp + ");</script>", false);
                    }
                }
            }
            
        }

        //**************************************************************************************
        //*
        //**************************************************************************************
        //protected void test1_Click(object sender, EventArgs e)
        //{
        //    hfDegrees.Value = "C";
        //    testTemperature.Value = "5.73";
        //    thermometer();
        //}
    }
}