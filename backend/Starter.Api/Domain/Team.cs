namespace Starter.Api.Domain;

/// <summary>
/// A team that uses the app. In week 3 the caller's team is read from the <c>X-Team</c> request header;
/// real login arrives in week 4. The <see cref="Slug"/> is the value sent in that header.
/// </summary>
public sealed class Team
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    /// <summary>The identifier sent in the <c>X-Team</c> header, e.g. <c>ekonomi</c>.</summary>
    public string Slug { get; set; } = string.Empty;
}
