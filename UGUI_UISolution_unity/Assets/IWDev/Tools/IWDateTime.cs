using System;
using UnityEngine;

/// <summary>
/// This class is something like DateTime and TimeSpan tweaker.
/// Better not use it if you need to calculate leap year and how much days every month have.
/// It it usefull for such cases as:
/// 1) when you need to store date in string format, use GetSpecificIWDT_String method. This is needed because DateTime.ToString returns different values on Android and IOS platforms;
/// 2) clear representativeness in editor for timers using method AddSeconds which automatically calculates hours and minutes
/// 3) possibility to use DateTime and TimeSpan at the same time
/// </summary>
[Serializable]
public class IWDateTime
{
    public int Year = 0;

    public int Month = 0;
    public int Day = 0;
    public int Hour = 0;
    public int Minute = 0;
    public int Second = 0;

    public double TotalSeconds = 0;


    public DateTime ThisDateTime;

    private static string _string_separator = ".";

    public IWDateTime(int year=0, int month=0, int day=0, int hour=0, int minute=0, int second=0)
    {
        Year = year;
        Month = month;
        Day = day;
        Hour = hour;
        Minute = minute;
        Second = second;

        ThisDateTime = ParseFromIWDT(this);

        if (Year == 0 && Month == 0 && Day == 0)
        {
            TotalSeconds = Hour*3600f + Minute*60f + Second;
        }
        else
        {
            TotalSeconds = (ThisDateTime - new DateTime(1,1,1)).TotalSeconds;
        }
    }

    public string GetString()
    {
        return Year + _string_separator
                + Month + _string_separator
                + Day + _string_separator
                + Hour + _string_separator
                + Minute + _string_separator
                + Second;
    }

    public string GetStringCuteFull()
    {
        string res = "";
        if (Year > 0) res += Year.ToString() + "y" + " ";
        if (Month > 0) res += Month.ToString() + "m" + " ";
        if (Day > 0) res += Day.ToString() + "d" + " ";
        if (Hour > 0) res += Hour.ToString() + "h" + " ";
        if (Minute > 0) res += Minute.ToString() + "m" + " ";
        if (Second > 0) res += Second.ToString() + "s" + " ";

        return res;
    }

    public static string GetSpecificIWDT_String(DateTime dt)
    {
        return dt.Year + _string_separator
            + dt.Month + _string_separator
            + dt.Day + _string_separator
            + dt.Hour + _string_separator
            + dt.Minute + _string_separator
            + dt.Second;
    }

    public static IWDateTime GetSpecificIWDT_IWDT(DateTime dt)
    {
        return new IWDateTime(
            dt.Year,
            dt.Month,
            dt.Day,
            dt.Hour,
            dt.Minute,
            dt.Second
            );

    }

    public static IWDateTime ParseFromFullString_toIWDT(string value)
    {
        string[] strs = value.Split(_string_separator[0]);

        return new IWDateTime(
            int.Parse(strs[0]),
            int.Parse(strs[1]),
            int.Parse(strs[2]),
            int.Parse(strs[3]),
            int.Parse(strs[4]),
            int.Parse(strs[5])
            );
    }

    public static DateTime ParseFromFullString_toDateTime(string value)
    {
        string[] strs = value.Split(_string_separator[0]);

        return new DateTime(
             int.Parse(strs[0]), //year
             int.Parse(strs[1]), //month
             int.Parse(strs[2]), //day

             int.Parse(strs[3]), //hours
             int.Parse(strs[4]), //minutes
             int.Parse(strs[5])  //seconds
             );
    }

    public static DateTime ParseFromIWDT(IWDateTime value)
    {
        int fixedY = value.Year;
        if (fixedY < 1) fixedY = 1;
        int fixedM = value.Month;
        if (fixedM < 1) fixedM = 1;
        if (fixedM > 12) fixedM = 12;

        int fixedD = value.Day;
        if (fixedD < 1) fixedD = 1;
        if (fixedD > 31) fixedD = 31;


        return new DateTime(
             fixedY, //year
             fixedM, //month
             fixedD, //day

             value.Hour, //hours
             value.Minute, //minutes
             value.Second  //seconds
             );
    }


    public static IWDateTime GetSpecificIWDT_TimeSpan(TimeSpan timeSpan)
    {
        int hours = (int)Math.Floor(timeSpan.TotalSeconds / 3600f);
        int minutes = (int)Math.Floor((timeSpan.TotalSeconds - hours * 3600) / 60f);
        int seconds = (int)(timeSpan.TotalSeconds - hours * 3600 - minutes*60);

        return new IWDateTime(
            0,
            0,
            0,
            hours,
            minutes,
            seconds
            );

    }

    /// <summary>
    /// Simple subtraction operation
    /// </summary>
    /// <param name="val1">from what we substract</param>
    /// <param name="val2">what we subtract</param>
    /// <returns></returns>
    public static IWDateTime Subtract(IWDateTime val1, IWDateTime val2)
    {
        TimeSpan interval = val1.ThisDateTime - val2.ThisDateTime;

        return GetSpecificIWDT_TimeSpan(interval);
    }

    public IWDateTime Subtract(IWDateTime val)
    {
        TimeSpan interval = ThisDateTime - val.ThisDateTime;

        return GetSpecificIWDT_TimeSpan(interval);
    }


    public void AddSeconds(float value)
    {
        TotalSeconds += value;

        Hour = (int)Math.Floor(TotalSeconds / 3600f);
        Minute = (int)Math.Floor((TotalSeconds - Hour * 3600) / 60f);
        Second = (int)(TotalSeconds - Hour * 3600 - Minute * 60);

    }

}
