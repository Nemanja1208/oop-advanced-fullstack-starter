using Microsoft.Extensions.Time.Testing;
using Starter.Api.Features.Todos;

namespace Starter.Tests.Features.Todos;

/// <summary>"Today" is a business rule: Swedish time, wherever the server happens to run.</summary>
public sealed class DeadlineCalendarTests
{
    private static readonly TimeZoneInfo Stockholm = TimeZoneInfo.FindSystemTimeZoneById("Europe/Stockholm");

    [Fact]
    [Trait("Krav", "AK-2.1")]
    public void Today_is_still_the_same_day_at_23_59_swedish_time()
    {
        // 22:59 UTC in November is 23:59 in Stockholm (CET, UTC+1).
        var clock = new FakeTimeProvider(new DateTimeOffset(2026, 11, 16, 22, 59, 0, TimeSpan.Zero));

        var today = new DeadlineCalendar(clock, Stockholm).Today();

        Assert.Equal(new DateOnly(2026, 11, 16), today);
    }

    [Fact]
    [Trait("Krav", "AK-2.1")]
    public void Today_changes_at_midnight_swedish_time_not_utc()
    {
        var clock = new FakeTimeProvider(new DateTimeOffset(2026, 11, 16, 23, 0, 0, TimeSpan.Zero));

        var today = new DeadlineCalendar(clock, Stockholm).Today();

        Assert.Equal(new DateOnly(2026, 11, 17), today);
    }

    [Fact]
    [Trait("Krav", "AK-5.1")]
    public void DateOf_converts_a_utc_timestamp_to_the_swedish_date()
    {
        var calendar = new DeadlineCalendar(new FakeTimeProvider(), Stockholm);

        var date = calendar.DateOf(new DateTimeOffset(2026, 11, 16, 23, 30, 0, TimeSpan.Zero));

        Assert.Equal(new DateOnly(2026, 11, 17), date);
    }
}
