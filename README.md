# SimpleScheduler

## What is it?

Obviously there is Quartz.net, but I dislike cron strings and fancied a go a writing scheduler myself.

I intend to support the following modes and represent them with a hopefully simple string syntax:
* `"now"` (run the task immediately)
* `"at|2020-01-24T00:00:00"` (run the task at a specific time, the date must be parsable as a UTC DateTime in dotnet)
* `"after|4d"` or `"after|2hh"` or `"after|50ss"` (run the task after a specified timespan)
* `"every|4d"` (run the task every 4 days forever or until schedule is changed or deleted)
* `"every|1mm|x5"` (run the task every minute, 5 times then stop)
* `"everyStartingAt|1mm|2020-01-24T00:00:00"` (run the task every minute, starting at a particular date and time)

Timespans are described using a combination of a number and a letter(s) where `y`=years, `m`=months, `d`=days, the time components use two letters; `hh`=hours, `mm`=minutes and `ss`=seconds.

Internally, SimpeScheduler depends on the excellent NodaTime library, all future datetimes are calculated by NodaTime in the london timezone and then stored as UTC BCL DateTime.

## How to use it

`ScheduledTasks` have a `JobData` string property and a `JobDataTypeName` propety. If `JobData` can be an ordinary string or a json serialised objec, where `JobDataTypeName` contains the name of the Type i.e. either System.String or whatever the Type of the object is. This enables us to register handlers for specific JobData Types.

```c#
var sched = new SimpleSchedule().FromString("now");
var sched = new SimpleSchedule().FromString("at|2020-01-24T00:00:00");
var sched = new SimpleSchedule().FromString("every|3d");
var sched = new SimpleSchedule().FromString("every|10ss|x5");
var sched = new SimpleSchedule().FromString("after|2hh");
```
Calling `ToString()` on a SimpleScheduler will return the string representation.

Tasks can also be defined in code using fluent api
```c#
var sched = new SimpleSchedule().Run().Now();
var sched = new SimpleSchedule().Run().At(new DateTime(2020, 01, 24));
var sched = new SimpleSchedule().Run().Every(Interval.Days(3));
var sched = new SimpleSchedule().Run().Every(Interval.Seconds(10), 5);
var sched = new SimpleSchedule().Run().EveryStartingAt(Interval.Seconds(10), new DateTime(2020, 1, 24, 9, 30, 0));
var sched = new SimpleSchedule().Run().After(Interval.Hours(2));
```

```c#
// Given a payload class...
public class SpecificTaskPayload
{
    public string SomethingSpecific { get; set; }

    public string Run() => $"something specific {SomethingSpecific} @ {DateTime.UtcNow}";
}
...

// Request an instance of the IScheduledTaskRepository and add some tasks to it...
_taskRepository.AddScheduledTask(_taskBuilderFactory.CreateBuilder().WithJobData("* run task now!").Now());
_taskRepository.AddScheduledTask(_taskBuilderFactory.CreateBuilder().WithJobData("* run task after 10 seconds").After(Lapse.Seconds(30)));
_taskRepository.AddScheduledTask(_taskBuilderFactory.CreateBuilder().WithJobData("* run task at a specified DateTime").At(DateTime.UtcNow.AddSeconds(20)));
_taskRepository.AddScheduledTask(_taskBuilderFactory.CreateBuilder().WithJobData("* run task every 5 seconds for 3 times").Every(Lapse.Seconds(5), 3));
_taskRepository.AddScheduledTask(_taskBuilderFactory.CreateBuilder().WithJobData("* run task every 20 seconds starting at now + 1mm").EveryStartingAt(Lapse.Seconds(20), DateTime.UtcNow.AddMinutes(1)).CreateTask());
var payload = new SpecificTaskPayload()
{
    SomethingSpecific = "Bob"
};
_taskRepository.AddScheduledTask(_taskBuilderFactory.CreateBuilder().WithJobData(payload).Now().CreateTask());

// Request an instance of IDueTaskJobQueue from DI and setup a task handler for all tasks
dueTaskJobQueue.RegisterHandlerForAllTasks((scheduledTask) =>
{
    _logger.LogInformation($"{scheduledTask.JobData} {scheduledTask.Id}");
});

// Or setup a specific task handler for tasks which have a SpecificTaskPayload as their jobdata
dueTaskJobQueue.RegisterHandlerWhen((scheduledTask) =>
{
    var payload = JsonSerializer.Deserialize<SpecificTaskPayload>(scheduledTask.JobData);
    _logger.LogInformation($"SpecificTaskPayload {payload.Run()} {scheduledTask.Id}");
}, task => task.JobDataTypeName == nameof(SpecificTaskPayload));

```

## How it works

The main parts are:
1) A maintained list of scheduled tasks, persisted somewhere in-memory or via an implementation of the IScheduledTaskRepository, a local json file implementation is included.
2) A timer which elapses by default every 10 seconds (this is the minimum resolution, i.e. you cant schedule tasks more frequequently than this). The timer evaluates whether any of the tasks have become due and enqueues any due tasks Action<IJob> in the DueTaskJobQueue
3) The DueTaskJobQueue, using a TPL DataFlow job queue, will execute Actions until there are none left, this doesn't hold up the timer, so you need to be careful about long tasks completing before they become due again.

## Future stuff

* distribution / leader election - leader should enqueue a stillAlive task every 30s ect, to be persisted, watched by leaders-in-waiting, if not updated, another node becomes the leader? Use Action<ISystemJob> etc
