using AJP.SimpleScheduler.Intervals;
using AJP.SimpleScheduler.ScheduledTasks;
using NodaTime;
using NodaTime.Testing;
using NUnit.Framework;
using System;

namespace AJP.SimpleScheduler.Tests
{
    public class ScheduledTaskBuilderTests
    {
        [Test]
        public void ToString_should_return_the_correct_string()
        {
            var instant = Instant.FromUtc(2020, 01, 26, 22, 25, 00);
            var fakeClock = new FakeClock(instant);
            
            var sut = new ScheduledTaskBuilder(fakeClock);

            Assert.That(sut.Run("blah").Now().CreateTask().ToString(), Is.EqualTo("now"));
            Assert.That(sut.Run("blah").At(new DateTime(2020, 01, 24)).CreateTask().ToString(), Is.EqualTo("at|2020-01-24T00:00:00"));
            Assert.That(sut.Run("blah").At("2020-01-24T00:00:00").CreateTask().ToString(), Is.EqualTo("at|2020-01-24T00:00:00"));
            Assert.That(sut.Run("blah").Every(Lapse.Days(3)).CreateTask().ToString(), Is.EqualTo("every|3d"));
            Assert.That(sut.Run("blah").Every(Lapse.Seconds(10), 5).CreateTask().ToString(), Is.EqualTo("every|10ss|x5"));
            Assert.That(sut.Run("blah").After(Lapse.Hours(2)).CreateTask().ToString(), Is.EqualTo("after|2hh"));
        }

        [Test]
        public void FromString_should_load_correct_settings_proven_by_ToString_roundtrip()
        {
            var instant = Instant.FromUtc(2020, 01, 26, 22, 25, 00);
            var fakeClock = new FakeClock(instant);

            var sut = new ScheduledTaskBuilder(fakeClock);

            Assert.That(sut.FromString("now").ToString(), Is.EqualTo("now"));
            Assert.That(sut.FromString("at|2020-01-24T00:00:00").ToString(), Is.EqualTo("at|2020-01-24T00:00:00"));
            Assert.That(sut.FromString("every|3d").ToString(), Is.EqualTo("every|3d"));
            Assert.That(sut.FromString("every|10ss|x5").ToString(), Is.EqualTo("every|10ss|x5"));
            Assert.That(sut.FromString("after|2hh").ToString(), Is.EqualTo("after|2hh"));
        }
    }
}