using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace TimeTrackerProgrammClient
{
    public partial class Form1 : Form
    {
        public static int myListCount = 0;


        public static Dictionary<string, string> Periods = new Dictionary<string, string> {
            { "Сейчас", "Now" },
            { "Сегодня", "Day" },
            { "Последние 7 дней", "Period" },
            { "В этом месяце", "Month" },
            { "В этом году", "Year" },
            { "За все время", "AllOfTime" }
        };

        public static Dictionary<string, string> Chastota = new Dictionary<string, string> {
            { "Сейчас", "ForNow" },
            { "По часам", "ForHour" },
            { "По дням недели", "ForDay"},
            { "По дням месяца", "ForDate"},
            { "По месяцам", "ForMonth" },
            { "По годам", "ForYear"}
        };

        public static string token;

        public static MyTimeSpan[] myTimes = null;
        public Form1()
        {
            InitializeComponent();
            button4.BackgroundImage = Image.FromFile(Directory.GetCurrentDirectory() + "\\Resourses\\Down.png");
            Form1_SizeChanged(this, new EventArgs());
            FileStream file = new FileStream(Directory.GetCurrentDirectory() + "\\Userdata.txt", FileMode.OpenOrCreate);
            StreamReader reader = new StreamReader(file);
            if (!reader.EndOfStream)
            {
                try
                {
                    WebRequest req = WebRequest.CreateHttp("https://localhost:5001/api/Account/login");
                    req.ContentType = "application/json";
                    req.Method = "POST";
                    using (var streamWriter = new StreamWriter(req.GetRequestStream()))
                    {
                        streamWriter.Write(JsonSerializer.Serialize(new User { Email = reader.ReadLine(), Password = reader.ReadLine() }));
                    }
                    WebResponse resp = req.GetResponse();
                    using (var streamWriter = new StreamReader(resp.GetResponseStream()))
                    {
                        token = streamWriter.ReadToEnd().Split('"')[3];
                        panel1.Visible = false;
                        panel2.Visible = true;
                    }
                }
                catch
                {
                    MessageBox.Show("Данные о пользователе повреждены, выполните ручной вход", "Не удалось выполнить автоматический вход", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    panel1.Visible = true;
                }
            }
            else
            {
                panel1.Visible = true;
            }
            reader.Close();
            file.Close();
        }

        private void Form1_SizeChanged(object sender, EventArgs e)
        {
            panel1.Size = new Size(Width, Height);
            panel3_SizeChanged(panel3, new EventArgs());
            panel4_SizeChanged(panel4, new EventArgs());
        }

        private void panel1_SizeChanged(object sender, EventArgs e)
        {
            textBox1.Size = new Size(panel1.Width / 3, panel1.Height / 3);
            textBox2.Size = new Size(panel1.Width / 3, panel1.Height / 3);
            button1.Size = new Size(panel1.Width / 6 - 2, panel1.Height / 13);
            button2.Size = new Size(panel1.Width / 6 - 2, panel1.Height / 13);
            textBox1.Location = new Point(panel1.Width / 3, panel1.Height / 5);
            label1.Location = new Point(panel1.Width / 3, panel1.Height / 5 - 15);
            textBox2.Location = new Point(panel1.Width / 3, 2 * panel1.Height / 5);
            label2.Location = new Point(panel1.Width / 3, 2 * panel1.Height / 5 - 15);
            button1.Location = new Point(panel1.Width / 3, 3 * panel1.Height / 5);
            button2.Location = new Point(panel1.Width / 2 + 2, 3 * panel1.Height / 5);
            label3.Location = new Point(panel1.Width / 3, panel1.Height / 3 - 30);
            checkBox1.Location = new Point(panel1.Width / 3, panel1.Height * 3 / 5 - 25);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            WebRequest req = WebRequest.CreateHttp("https://localhost:5001/api/Account/login");
            req.ContentType = "application/json";
            req.Method = "POST";
            using (var streamWriter = new StreamWriter(req.GetRequestStream()))
            {
                streamWriter.Write(JsonSerializer.Serialize(new User { Email = textBox1.Text, Password = textBox2.Text }));
            }
            WebResponse resp = req.GetResponse();
            using (var streamWriter = new StreamReader(resp.GetResponseStream()))
            {
                token = streamWriter.ReadToEnd().Split('"')[3];
                if (checkBox1.Checked)
                {
                    FileStream file = new FileStream(Directory.GetCurrentDirectory() + "\\Userdata.txt", FileMode.OpenOrCreate);
                    StreamWriter writer = new StreamWriter(file);
                    writer.WriteLine(textBox1.Text);
                    writer.WriteLine(textBox2.Text);
                    writer.Close();
                    file.Close();
                }
                panel1.Visible = false;
                panel2.Visible = true;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            WebRequest req = WebRequest.CreateHttp("https://localhost:5001/api/Account/Register");
            req.ContentType = "application/json";
            req.Method = "POST";
            using (var streamWriter = new StreamWriter(req.GetRequestStream()))
            {
                streamWriter.Write(JsonSerializer.Serialize(new User { Email = textBox1.Text, Password = textBox2.Text }));
            }
            WebResponse resp = req.GetResponse();
            using (var streamWriter = new StreamReader(resp.GetResponseStream()))
            {
                token = streamWriter.ReadToEnd().Split('"')[3];
                if (checkBox1.Checked)
                {
                    FileStream file = new FileStream(Directory.GetCurrentDirectory() + "\\Userdata.txt", FileMode.OpenOrCreate);
                    StreamWriter writer = new StreamWriter(file);
                    writer.WriteLine(textBox1.Text);
                    writer.WriteLine(textBox2.Text);
                    writer.Close();
                    file.Close();
                }
                panel1.Visible = false;
                panel2.Visible = true;
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            panel4.Visible = false;
            panel3.Visible = true;
            button4.Visible = true;
        }

        private void panel3_SizeChanged(object sender, EventArgs e)
        {
            button4.Location = new Point(panel3.Width / 2 - 25, label4.Location.Y + label4.Height + 50);
        }

        private void button4_Click(object sender, EventArgs e)
        {
            if (comboBox1.SelectedIndex != -1)
            {
                WebRequest req = WebRequest.CreateHttp("https://localhost:5001/api/Activity/Statistics/List/" + Periods[comboBox1.Text] + "/" + myListCount++ + "/" + 2);
                req.ContentType = "application/json";
                req.Method = "GET";
                req.Headers.Add("Authorization:Bearer " + token);
                WebResponse resp = req.GetResponse();
                using (var streamWriter = new StreamReader(resp.GetResponseStream()))
                {
                    string res = Regex.Unescape(streamWriter.ReadToEnd().Replace("}", "\r\n"));
                    label4.Text += res;
                    if (res == "\"Активностей больше нет\"") button4.Visible = false;
                }
                button4.Location = new Point(panel3.Width / 2 - 25, label4.Location.Y + label4.Height + 50);
            }
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            label4.Text = "";
            myListCount = 0;
            button4.Visible = true;
            button4_Click(comboBox1, new EventArgs());
        }

        private void button5_Click(object sender, EventArgs e)
        {
            panel3.Visible = false;
            panel4.Visible = true;
            button6.Visible = true;
            comboBox2.Items.Clear();
            WebRequest req = WebRequest.CreateHttp("https://localhost:5001/api/Activity/ActivityInfo/Activities");
            req.ContentType = "application/json";
            req.Method = "GET";
            req.Headers.Add("Authorization:Bearer " + token);
            WebResponse resp = req.GetResponse();
            using (var streamWriter = new StreamReader(resp.GetResponseStream()))
            {
                string[] res = JsonSerializer.Deserialize(streamWriter.ReadToEnd(), typeof(string[])) as string[];
                comboBox2.Items.AddRange(res);
            }
            comboBox2.SelectedIndex = 0;
            comboBox3.SelectedIndex = 0;
        }

        private void button6_Click(object sender, EventArgs e)
        {
            WebRequest req = WebRequest.CreateHttp("https://localhost:5001/api/Activity/ActivityInfo/Activities");
            req.ContentType = "application/json";
            req.Method = "GET";
            req.Headers.Add("Authorization:Bearer " + token);
            WebResponse resp = req.GetResponse();
            using (var streamWriter = new StreamReader(resp.GetResponseStream()))
            {
                string[] res = JsonSerializer.Deserialize(streamWriter.ReadToEnd(), typeof(string[])) as string[];
                comboBox2.Items.AddRange(res);
            }
            button6.Location = new Point(panel4.Width / 2 - 25, label5.Location.Y + label5.Height + 50);
        }

        private void panel4_SizeChanged(object sender, EventArgs e)
        {
            button6.Location = new Point(panel4.Width / 2 - 25, label5.Location.Y + label5.Height + 50);
        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            button6.Location = new Point(panel4.Width / 2 - 25, label5.Location.Y + label5.Height + 50);
        }

        private void comboBox3_SelectedIndexChanged(object sender, EventArgs e)
        {
            comboBox4.Items.Clear();
            if (comboBox3.SelectedIndex > 0)
            {
                comboBox4.Items.AddRange(Chastota.Keys.ToList().GetRange(1, comboBox3.SelectedIndex).ToArray());
            }
            else
            {
                comboBox4.Items.AddRange(Chastota.Keys.ToList().GetRange(0, comboBox3.SelectedIndex + 1).ToArray());
            }
            comboBox4.SelectedIndex = 0;
            ActivityGrafDraw();
            button6.Location = new Point(panel4.Width / 2 - 25, label5.Location.Y + label5.Height + 50);
        }

        private void comboBox4_SelectedIndexChanged(object sender, EventArgs e)
        {
            ActivityGrafDraw();
        }

        private void ActivityGrafDraw()
        {
            label5.Text = "";
            WebRequest req = WebRequest.CreateHttp("https://localhost:5001/api/Activity/Statistics/Activity/" + Periods[comboBox3.Text] + "/" + Chastota[comboBox4.Text] + "/" + comboBox2.Text);
            req.ContentType = "application/json";
            req.Method = "GET";
            req.Headers.Add("Authorization:Bearer " + token);
            WebResponse resp = req.GetResponse();
            MyTimeSpan[] times;
            using (var streamWriter = new StreamReader(resp.GetResponseStream()))
            {
                string s = streamWriter.ReadToEnd();
                times = MyTimeSpan.DesirialazeArray(s);
                for (int i = 0; i < times.Length; i++)
                {
                    label5.Text += i + ":    " + times[i].Day + " Дней, " + times[i].Hour + " Часов, " + times[i].Minute + " Минут, " + times[i].Second + " Секунд" + "\r\n";
                }
            }
            myTimes = times;
            panel4.Refresh();
        }

        private void panel4_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = panel5.CreateGraphics();
            Pen penOXY = new Pen(Color.Black, 4);
            penOXY.EndCap = System.Drawing.Drawing2D.LineCap.ArrowAnchor;
            g.DrawLine(penOXY, 0, 202, 404, 202);
            g.DrawLine(penOXY, 2, 204, 2, 0);
            if(myTimes != null)
            {
                Pen lines = new Pen(Color.Red, 2);
                TimeSpan[] times = myTimes.ToList().ConvertAll(t => new TimeSpan(t.Day, t.Hour, t.Minute, t.Second, t.Milisecond)).ToArray();
                int max = (int)times.Max().TotalSeconds;
                Point Prev = new Point();
                if (times.Length > 0) Prev = new Point(4, 200 - (int)(200*times[0].TotalSeconds/max));
                for(int i = 1; i < times.Length; i++)
                {
                    g.DrawLine(lines, Prev, new Point(4 + (i * (400/times.Length)), 200 - (int)(200 * times[i].TotalSeconds / max)));
                    Prev = new Point(4 + (i * (400 / times.Length)), 200 - (int)(200 * times[i].TotalSeconds / max));
                }
            }
        }
    }
}
