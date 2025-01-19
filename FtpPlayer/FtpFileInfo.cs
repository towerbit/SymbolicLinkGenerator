using System;
using System.Globalization;
using System.Linq;

namespace FtpPlayer
{
    internal class FtpFileInfo
    {
        public string Name { get; set; }
        public ulong Length { get; set; }
        public DateTime Date { get; set; }
        public bool IsFile { get; set; }

        public FtpFileInfo() { }
        public FtpFileInfo(string ftpLine)
        {
            //drwxrwxrwx 1 ftp ftp               0 Jul 08  2024 iptv
            //-rw-rw-rw- 1 ftp ftp              22 Nov 06  2022 readme.txt
            //drwxrwxrwx 1 ftp ftp               0 Dec 07 14:21 movie
            string[] parts = ftpLine.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            IsFile = parts.First<string>().StartsWith("-");
            // 兼容文件名中包含空格的情况
            Name = parts.Skip(8).Take(parts.Length - 8).Aggregate((a, b) => a + " " + b).Trim();
            // 解析日期
            var strDate = parts.Skip(5).Take(3).Aggregate((a, b) => a + " " + b);
            try
            {
                Date = DateTime.Parse(strDate);
            }
            catch (FormatException)
            {
                // 兼容日期格式为"MMM dd HH:mm"的日期字符串
                if (TryParseDateString(strDate, out var dt))
                    Date = dt;
            }

            Length = IsFile ? Convert.ToUInt64(parts[4]) : 0;
        }

        static bool TryParseDateString(string dateString, out DateTime dateTime)
        {
            string format = "MMM dd HH:mm";
            int currentYear = DateTime.Now.Year; // 获取当前年份
            string fullDateString = dateString + " " + currentYear;
            string fullFormat = format + " yyyy";

            return DateTime.TryParseExact(fullDateString, fullFormat, CultureInfo.InvariantCulture, DateTimeStyles.None, out dateTime);
        }
    }
}
