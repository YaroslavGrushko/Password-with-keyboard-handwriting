using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;

namespace Password
{
    public partial class Form1 : Form
    {
        public static List<long> elapsedList = new List<long>();
        public static List<long> elapsedListSample = new List<long>();
        long Length_elapsed = 0;              //Length of user's vector
        long Length_elapsed_Sample = 0;       //Length of sample's vector:
        long Manhattan_distance = 0;
        double Difference_with_the_sample = 0;
               
        public Form1()
        {           
            InitializeComponent();
            KeyPreview = true;
            
            var watch = new Stopwatch();
            KeyDown += (s, e) => watch.Start();
            KeyUp += (s, e) =>
            {                
                if (radioButton1.Checked)   //enter of user is selected
                {
                    watch.Stop();
                    if (password_textBox.Focused)
                    {
                        var elapsed = watch.ElapsedMilliseconds;    // Duration
                        elapsedListSample.Add(elapsed);
                        Vector_of_durations.Text += " " + elapsed;
                    }
                    watch.Reset();                  
                }
                else   //check of user is selected
                {
                    watch.Stop();
                    if (password_textBox.Focused)
                    {
                        var elapsed = watch.ElapsedMilliseconds;    // Duration
                        elapsedList.Add(elapsed);
                        Vector_of_durations.Text += " " + elapsed;
                    }
                    watch.Reset();
                }           
            };                                            
        }

        private void button1_Click(object sender, EventArgs e)  //  Apply button
        {
            Vector_of_durations.Text = "";
            Length_elapsed_Sample = 0;
            
            if (radioButton1.Checked)   //enter of user is selected
            {
                var vect = textBox1.Text; //Login
                var pass = password_textBox.Text;

                password_textBox.Text = "";

                    for (int i = 0; i < elapsedListSample.Count; i++)
                    {
                        Length_elapsed_Sample += elapsedListSample[i];    //Length of user's vector
                        vect =vect+ '|' + elapsedListSample[i].ToString();
                    }
                        LengthElapsedSample.Text = Length_elapsed_Sample.ToString();
                        vect += "|password=" + pass;
                //"UsersHandwriting" variable in config file for saving data:
                ((System.Collections.Specialized.StringCollection)Password.Properties.Settings.Default["UsersHandwriting"]).Add(vect);
                Password.Properties.Settings.Default.Save();

                textBox1.Text = ""; //Clear Login
                elapsedListSample.Clear();
            }
            else {
                Length_elapsed = 0;
               for (int i = 0; i < elapsedList.Count; i++)
               {
                    Length_elapsed += elapsedList[i];   //Length of sample's vector
               }
                    LengthElapsed.Text = Length_elapsed.ToString();
        
                elapsedListSample.Clear();

                int c = ((System.Collections.Specialized.StringCollection)Password.Properties.Settings.Default["UsersHandwriting"]).Count;
                String[,] myUsersDiff = new String[c,3];
                
                int index = 0;   //myUsersDiff index

            foreach (var user in ((System.Collections.Specialized.StringCollection)Password.Properties.Settings.Default["UsersHandwriting"]))
            {
                string[] items = user.Split('|');
                myUsersDiff[index,0] = items[0];  //user's Name
                int itemsIndex = 0;               //items index

                elapsedListSample.Clear();

                foreach (string item in items)
                {
                    if (itemsIndex != 0) {
                            if (!item.Contains("password=")) { 
                                elapsedListSample.Add(Convert.ToInt64(item));
                            }else {

                                var pass = item.Replace("password=", "");
                                if (pass == password_textBox.Text) { myUsersDiff[index, 2] = "1"; }  //verify password
                                else
                                {
                                    myUsersDiff[index, 2] = "0";
                                };
                                  }
                            }
                   itemsIndex++;
                }
                    Length_elapsed_Sample = 0;
                    for (int i = 0; i < elapsedListSample.Count; i++)
                    {
                        Length_elapsed_Sample += elapsedListSample[i];  //Length of user's vector
                    }

                      if ((Length_elapsed > 0) && (Length_elapsed_Sample > 0))  //check of user and Sample are exist
                        {
                      if(  elapsedListSample.Count== elapsedList.Count)  //verify vectors length
                        { 

                    for (int i = 0; i < elapsedListSample.Count; i++)
                    {
                        Manhattan_distance += Math.Abs(elapsedListSample[i] - elapsedList[i]);  //Manhattan distance                                
                    }
                        }else { myUsersDiff[index, 2] = "0"; }

                        ManhattanDistance.Text = Manhattan_distance.ToString();
                        Difference_with_the_sample = ((float)Manhattan_distance / (float)(Length_elapsed + Length_elapsed_Sample)) * 100;
                        myUsersDiff[index,1] = Difference_with_the_sample.ToString();
                       
                        }
                index++;
            }

            double minDiff = 1000;
            String ResultUser = "";
            for(int ind=0;ind< myUsersDiff.GetLength(0); ind++)
            {
                if ((minDiff > Convert.ToDouble(myUsersDiff[ind,1]))&&(textBox1.Text==myUsersDiff[ind, 0])&& myUsersDiff[ind,2]=="1")
                    { minDiff = Convert.ToDouble(myUsersDiff[ind, 1]);
                      ResultUser = myUsersDiff[ind, 0];
                    }
            }
            Diff.Text = minDiff.ToString("F2");

                if (ResultUser != "") { label6.Text = ResultUser; }
                else { label6.Text = "UserName or password is incorrect"; }
                
            LengthElapsedSample.Text = Length_elapsed_Sample.ToString();

            password_textBox.Clear();
            elapsedListSample.Clear();
            elapsedList.Clear();
            Length_elapsed = 0;
            Length_elapsed_Sample = 0;
            Manhattan_distance = 0;
            }
        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            Vector_of_durations.Text = "";
            if (radioButton1.Checked)
            {
                elapsedList.Clear();
                elapsedListSample.Clear();
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            radioButton1.Checked = true;
        }
     
        private void radioButton2_CheckedChanged(object sender, EventArgs e)  //move to check user
        {
            if (password_textBox.Text != "" && textBox1.Text != "")
            { 
            var vect = textBox1.Text; //Login
            if (radioButton2.Checked)
                {
                 var pass = password_textBox.Text;
                 password_textBox.Text = "";
                
                for (int i = 0; i < elapsedListSample.Count; i++)
                {
                Length_elapsed_Sample += elapsedListSample[i];    //Length of user's vector
                    vect+= '|' + elapsedListSample[i];
                }
                vect += "password=" + pass;
                LengthElapsedSample.Text =  Length_elapsed_Sample.ToString();
                }

            ((System.Collections.Specialized.StringCollection)Password.Properties.Settings.Default["UsersHandwriting"]).Add(vect);
            Password.Properties.Settings.Default.Save();
            textBox1.Text = "";
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            ((System.Collections.Specialized.StringCollection)Password.Properties.Settings.Default["UsersHandwriting"]).Clear();
        }
    }
}
