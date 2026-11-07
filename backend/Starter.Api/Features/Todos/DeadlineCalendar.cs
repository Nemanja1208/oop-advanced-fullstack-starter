namespace Starter.Api.Features.Todos;

/// <summary>
/// Answers "what date is it for the customer?". Wraps the clock and the time zone, so the rest of the code
/// never calls <c>DateTime.Now</c> and tests can choose the exact moment they run at.
/// </summary>
public sealed class DeadlineCalendar(TimeProvider clock, TimeZoneInfo timeZone)
{
    public DateTimeOffset Now() => clock.GetUtcNow();

    public DateOnly Today() => DateOf(clock.GetUtcNow());

    public DateOnly DateOf(DateTimeOffset moment) => DateOnly.FromDateTime(TimeZoneInfo.ConvertTime(moment, timeZone).DateTime);
}
