using System;

namespace TimeTrackerProgrammClient
{
    class User
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }

    public struct MyTimeSpan
    {
        public int Day { get; set; }

        public int Hour { get; set; }

        public int Minute { get; set; }

        public int Second { get; set; }

        public int Milisecond { get; set; }


        public static MyTimeSpan[] DesirialazeArray(string s)
        {
            string[] ss = s.Split(new string[] { "},", "}" }, StringSplitOptions.RemoveEmptyEntries);
            MyTimeSpan[] myTimes = new MyTimeSpan[ss.Length-1];
            for (int i = 0; i < ss.Length-1; i++)
            {
                string[] sss = ss[i].Split(',');
                myTimes[i] = new MyTimeSpan();
                string[] ssss = sss[0].Split(':');
                myTimes[i].Day = Convert.ToInt32(ssss[ssss.Length - 1]);
                ssss = sss[1].Split(':');
                myTimes[i].Hour = Convert.ToInt32(ssss[ssss.Length - 1]);
                ssss = sss[2].Split(':');
                myTimes[i].Minute = Convert.ToInt32(ssss[ssss.Length - 1]);
                ssss = sss[3].Split(':');
                myTimes[i].Second = Convert.ToInt32(ssss[ssss.Length - 1]);
                ssss = sss[4].Split(':');
                myTimes[i].Milisecond = Convert.ToInt32(ssss[ssss.Length - 1]);
            }
            return myTimes;
        }
    }
}
