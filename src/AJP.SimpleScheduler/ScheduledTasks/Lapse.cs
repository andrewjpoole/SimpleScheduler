using System;

namespace AJP.SimpleScheduler.Intervals
{
    public class Lapse : ILapse
    {
        public string Unit { get; }
        public int Number { get; }

        public const string YearsUnit = "y";
        public const string MonthsUnit = "m";
        public const string DaysUnit = "d";
        public const string HoursUnit = "hh";
        public const string MinutesUnit = "mm";
        public const string SecondsUnit = "ss";

        public Lapse()
        {
        }

        public Lapse(string unit, int number)
        {
            Unit = unit;
            Number = number;
        }

        public override string ToString()
        {
            return $"{Number}{Unit}";
        }

        public static Lapse Years(int number)
        {
            return new Lapse(YearsUnit, number);
        }

        public static Lapse Months(int number)
        {
            return new Lapse(MonthsUnit, number);
        }

        public static Lapse Days(int number) 
        {
            return new Lapse(DaysUnit, number);
        }

        public static Lapse Hours(int number)
        {
            return new Lapse(HoursUnit, number);
        }

        public static Lapse Minutes(int number)
        {
            return new Lapse(MinutesUnit, number);
        }

        public static Lapse Seconds(int number)
        {
            return new Lapse(SecondsUnit, number);
        }

        public static Lapse Parse(string interval)
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
