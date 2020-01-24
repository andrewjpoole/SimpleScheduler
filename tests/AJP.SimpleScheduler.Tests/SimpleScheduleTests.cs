using AJP.SimpleScheduler.Intervals;
using NUnit.Framework;
using SimpleScheduler;
using System;

namespace AJP.SimpleScheduler.Tests
{
    public class SimpleScheduleTests
    {
        [Test]
        public void ToString_should_return_the_correct_string()
        {
            Assert.That(new SimpleSchedule().Run("blah").Now().ToString(), Is.EqualTo("now"));
            Assert.That(new SimpleSchedule().Run("blah").At(new DateTime(2020, 01, 24)).ToString(), Is.EqualTo("at|2020-01-24T00:00:00"));
            Assert.That(new SimpleSchedule().Run("blah").Every(Interval.Days(3)).ToString(), Is.EqualTo("every|3d"));
            Assert.That(new SimpleSchedule().Run("blah").Every(Interval.Seconds(10), 5).ToString(), Is.EqualTo("every|10ss|x5"));
            Assert.That(new SimpleSchedule().Run("blah").After(Interval.Hours(2)).ToString(), Is.EqualTo("after|2hh"));
        }

        [Test]
        public void Parse_should_load_correct_settings_proven_by_ToString_roundtrip()
        {
            Assert.That(new SimpleSchedule().FromString("now").ToString(), Is.EqualTo("now"));
            Assert.That(new SimpleSchedule().FromString("at|2020-01-24T00:00:00").ToString(), Is.EqualTo("at|2020-01-24T00:00:00"));
            Assert.That(new SimpleSchedule().FromString("every|3d").ToString(), Is.EqualTo("every|3d"));
            Assert.That(new SimpleSchedule().FromString("every|10ss|x5").ToString(), Is.EqualTo("every|10ss|x5"));
            Assert.That(new SimpleSchedule().FromString("after|2hh").ToString(), Is.EqualTo("after|2hh"));
        }
    }
}