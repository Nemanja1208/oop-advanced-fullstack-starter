namespace Starter.Api.Features.Teams;

/// <summary>
/// Who is asking? In week 3 the answer is simply the <c>X-Team</c> request header. There is no login yet,
/// so we trust the header for now (that changes in week 4). When the header is missing we fall back to the
/// development team, so tools and tests that don't send it still work.
/// </summary>
public sealed class TeamContext
{
    public const string HeaderName = "X-Team";
    public const string DefaultSlug = "utveckling";

    public TeamContext(IHttpContextAccessor accessor)
    {
        var header = accessor.HttpContext?.Request.Headers[HeaderName].FirstOrDefault();
        Slug = string.IsNullOrWhiteSpace(header) ? DefaultSlug : header.Trim().ToLowerInvariant();
    }

    /// <summary>The slug of the team making the current request.</summary>
    public string Slug { get; }
}
