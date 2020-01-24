using System;

namespace AJP.SimpleScheduler.Intervals
{
    public class Interval : IInterval
    {
        public string Unit { get; }
        public int Number { get; }

        public const string YearsUnit = "y";
        public const string MonthsUnit = "m";
        public const string DaysUnit = "d";
        public const string HoursUnit = "hh";
        public const string MinutesUnit = "mm";
        public const string SecondsUnit = "ss";

        public Interval(string unit, int number)
        {
            Unit = unit;
            Number = number;
        }

        public override string ToString()
        {
            return $"{Number}{Unit}";
        }

        public static Interval Years(int number)
        {
            return new Interval(YearsUnit, number);
        }

        public static Interval Months(int number)
        {
            return new Interval(MonthsUnit, number);
        }

        public static Interval Days(int number) 
        {
            return new Interval(DaysUnit, number);
        }

        public static Interval Hours(int number)
        {
            return new Interval(HoursUnit, number);
        }

        public static Interval Minutes(int number)
        {
            return new Interval(MinutesUnit, number);
        }

        public static Interval Seconds(int number)
        {
            return new Interval(SecondsUnit, number);
        }

        public static Interval Parse(string interval)
        {
            // "1y" | "2m" | "3d" | "4hh" | "5mm" | "6ss"
            return interval switch
            {
                string i when i.EndsWith(YearsUnit) => Years(ParseAmount(RemoveUnit(i, YearsUnit))),
                string i when i.EndsWith(MonthsUnit) => Months(ParseAmount(RemoveUnit(i, MonthsUnit))),
                string i when i.EndsWith(DaysUnit) => Days(ParseAmount(RemoveUnit(i, DaysUnit))),
                string i when i.EndsWith(HoursUnit) => Hours(ParseAmount(RemoveUnit(i, HoursUnit))),
                string i when i.EndsWith(MinutesUnit) => Minutes(ParseAmount(RemoveUnit(i, MinutesUnit))),
                string i when i.EndsWith(SecondsUnit) => Seconds(ParseAmount(RemoveUnit(i, SecondsUnit))),
                _ => throw new ArgumentException("The interval part must be an integer and the letter y=years, m=months, d=days, hh=hours, mm=minutes or ss=seconds i.e. 5d = 5 days."),
            };
        }

        private static string RemoveUnit(string intervalPart, string unit) 
        {
            return intervalPart.Replace(unit, string.Empty);
        }

        private static int ParseAmount(string intervalPart) 
        {
            try
            {
                var amount = int.Parse(intervalPart);
                return amount;
            }
            catch (Exception)
            {
                throw new ArgumentException("The interval part must be an integer and the letter y=years, m=months, d=days, hh=hours, mm=minutes or ss=seconds i.e. 5d = 5 days.");
            }
             
        }

        public static int ParseRepeatTimes(string interval)
        {
            // "x5"
            if (!interval.StartsWith("x"))
                throw new ArgumentException("The repeat times part must be an 'x' followed by an integer i.e. x5 means repeat 5 times");

            interval = interval.Replace("x", "");

            var times = int.Parse(interval);

            if (times != -1)
                return times;

            throw new ArgumentException("repeat times part must be an 'x' followed by an integer i.e. x5 means repeat 5 times");
        }
    }
}
