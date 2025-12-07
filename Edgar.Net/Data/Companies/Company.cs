namespace Edgar.Net.Data.Companies;

/// <summary>
/// Represents a company registered with the SEC.
/// </summary>
public record Company
{
    /// <summary>
    /// Central Index Key - unique identifier assigned by the SEC.
    /// </summary>
    public required uint CIK { get; init; }

    /// <summary>
    /// Company name as registered with the SEC.
    /// </summary>
    public required string Name { get; init; }

    /// <summary>
    /// Stock ticker symbol.
    /// </summary>
    public required string Ticker { get; init; }

    /// <summary>
    /// Stock exchange where the company is listed.
    /// </summary>
    public string Exchange { get; init; } = string.Empty;
}
