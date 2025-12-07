namespace Edgar.Net.Http.Forms;

/// <summary>
/// Interface for forms that can be parsed from raw text data.
/// </summary>
public interface IParsableForm
{
    /// <summary>
    /// Parses form data from the raw text content.
    /// </summary>
    /// <param name="data">The raw text data to parse.</param>
    void ParseData(string data);
}
