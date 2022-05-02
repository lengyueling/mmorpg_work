using System;

public class TimeUtil
{
    public static int timestamp
    {
        get { return GetTimestamp(DateTime.Now); }
    }

    public static DateTime GetTime(long timeStamp)
    {
        DateTime dateTimeStart = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1));
        long lTime = timeStamp * 10000000;
        TimeSpan toNow = new TimeSpan(lTime);
        return dateTimeStart.Add(toNow);
    }

    public static int GetTimestamp(System.DateTime time)
    {
        System.DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1));
        return (int)(time - startTime).TotalSeconds;
    }
}