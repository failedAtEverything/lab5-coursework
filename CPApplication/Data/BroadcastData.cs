using CPApplication.Core.Models;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace CPApplication.Data
{
    public class BroadcastData
    {
        public string Program { get; set; }
        public int Week { get; set; }
        public int Month { get; set; }
        public int Year { get; set; }

        public override string ToString()
        {
            return Program + " " + Week + "-" + Month + "-" + Year;
        }

        public static BroadcastData FromString(string str)
        {
            var spacePos = 0;
            for(int i = str.Length - 1; i >= 0; i--)
            {
                if (str[i] == ' ')
                {
                    spacePos = i;
                    break;
                }
            }

            var program = str[0..(spacePos + 1)];
            var date = str[(spacePos + 1)..];

            var week = int.Parse(date.Split('-')[0]);
            var month = int.Parse(date.Split('-')[1]);
            var year = int.Parse(date.Split('-')[2]);

            return new BroadcastData()
            {
                Program = program,
                Week = week,
                Month = month,
                Year = year
            };
        }

        public static bool Validate(string str)
        {
            if (!str.Contains(' ') | !str.Contains('-'))
            {
                return false;
            }

            var spacePos = 0;
            for (int i = str.Length - 1; i >= 0; i--)
            {
                if (str[i] == ' ')
                {
                    spacePos = i;
                    break;
                }
            }

            var program = str[0..spacePos];
            var date = str[spacePos..];

            if (!date.Contains('-') | date.Split('-').Length != 3)
            {
                return false;
            }

            int week, month, year;

            try
            {
                week = int.Parse(date.Split('-')[0]);
                month = int.Parse(date.Split('-')[1]);
                year = int.Parse(date.Split('-')[2]);
            }
            catch
            {
                return false;
            }

            return true;
        }
    }
}
