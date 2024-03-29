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

            Assert.That(sut.WithJobData("blah").Now().CreateTask().ToString(), Is.EqualTo("now"));
            Assert.That(sut.WithJobData("blah").At(new DateTime(2020, 01, 24)).CreateTask().ToString(), Is.EqualTo("at|2020-01-24T00:00:00"));
            Assert.That(sut.WithJobData("blah").At("2020-01-24T00:00:00").CreateTask().ToString(), Is.EqualTo("at|2020-01-24T00:00:00"));
            Assert.That(sut.WithJobData("blah").Every(Lapse.Days(3)).CreateTask().ToString(), Is.EqualTo("every|3d"));
            Assert.That(sut.WithJobData("blah").Every(Lapse.Seconds(10), 5).CreateTask().ToString(), Is.EqualTo("every|10ss|x5"));
            Assert.That(sut.WithJobData("blah").After(Lapse.Hours(2)).CreateTask().ToString(), Is.EqualTo("after|2hh"));
            Assert.That(sut.WithJobData("blah").EveryStartingAt(Lapse.Days(3), "2020-01-24T09:30:00").CreateTask().ToString(), Is.EqualTo("everyStartingAt|3d|2020-01-24T09:30:00"));
            Assert.That(sut.WithJobData("blah").EveryStartingAt(Lapse.Days(3), DateTime.Parse("2020-01-24T09:30:00")).CreateTask().ToString(), Is.EqualTo("everyStartingAt|3d|2020-01-24T09:30:00"));
        }

        [Test]
        public void FromString_should_load_correct_settings_proven_by_ToString_roundtrip()
        {
            var instant = Instant.FromUtc(2020, 01, 26, 22, 25, 00);
            var fakeClock = new FakeClock(instant);

            var sut = new ScheduledTaskBuilder(fakeClock);

            Assert.That(sut.FromString("now", "some job data").ToString(), Is.EqualTo("now"));
            Assert.That(sut.FromString("at|2020-01-24T00:00:00", "some job data").ToString(), Is.EqualTo("at|2020-01-24T00:00:00"));
            Assert.That(sut.FromString("every|3d", "some job data").ToString(), Is.EqualTo("every|3d"));
            Assert.That(sut.FromString("every|10ss|x5", "some job data").ToString(), Is.EqualTo("every|10ss|x5"));
            Assert.That(sut.FromString("after|2hh", "some job data").ToString(), Is.EqualTo("after|2hh"));
            Assert.That(sut.FromString("everyStartingAt|3d|2020-01-24T09:30:00", "some job data").ToString(), Is.EqualTo("everyStartingAt|3d|2020-01-24T09:30:00"));
        }
    }
}