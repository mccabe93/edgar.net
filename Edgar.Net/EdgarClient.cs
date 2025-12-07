using System.Text.Json;
using Edgar.Net.Controllers;
using Edgar.Net.Data.Companies;
using Edgar.Net.Managers;

namespace Edgar.Net;

/// <summary>
/// Main entry point for configuring and using the Edgar.NET library.
/// </summary>
public class EdgarClient
{
    /// <summary>
    /// If enabled, the cache manager will be used to store a query and its results as JSON locally
    /// and keep results in memory (in a dictionary).
    /// This can reduce the number of API calls to EDGAR and helps avoid hitting their request cap.
    /// </summary>
    public bool CacheResults { get; set; } = false;

    /// <summary>
    /// Base URL for most basic and bulk EDGAR tasks.
    /// </summary>
    public string BaseUrl { get; set; } = "https://www.sec.gov/";

    /// <summary>
    /// URL for the new REST API functions.
    /// See: https://www.sec.gov/edgar/sec-api-documentation
    /// </summary>
    public string ApiUrl { get; set; } = "https://data.sec.gov/";

    /// <summary>
    /// User agent value passed into header when requesting from EDGAR.
    /// The SEC requires a valid user agent with contact information.
    /// </summary>
    public string UserAgent { get; set; } = "Edgar.NET API User";

    /// <summary>
    /// Dictionary of all companies indexed by CIK.
    /// </summary>
    public Dictionary<uint, Company> Companies { get; private set; } = [];

    /// <summary>
    /// Maximum number of forms per request.
    /// </summary>
    public int MaxFormsCount { get; set; } = 100;

    /// <summary>
    /// JSON serialization options for consistent serialization/deserialization.
    /// </summary>
    public JsonSerializerOptions JsonSettings { get; set; } =
        new() { PropertyNameCaseInsensitive = true, WriteIndented = true };

    /// <summary>
    /// Utilities for HTTP operations and string manipulation.
    /// </summary>
    public EdgarUtilities Utilities { get; }

    /// <summary>
    /// Cache manager for storing and retrieving cached responses.
    /// </summary>
    public EdgarCacheManager Cache { get; }

    /// <summary>
    /// Company manager for retrieving company data.
    /// </summary>
    public EdgarCompanyManager CompanyManager { get; }

    /// <summary>
    /// Form retrieval controller for fetching SEC forms.
    /// </summary>
    public FormRetrievalController Forms { get; }

    private readonly SemaphoreSlim _initLock = new(1, 1);
    private bool _isInitialized;

    public EdgarClient()
    {
        Utilities = new EdgarUtilities(this);
        Cache = new EdgarCacheManager(this);
        CompanyManager = new EdgarCompanyManager(this);
        Forms = new FormRetrievalController(this);
    }

    /// <summary>
    /// Initializes the CIK database with all registered companies.
    /// </summary>
    public async Task InitializeAsync()
    {
        await _initLock.WaitAsync();
        try
        {
            if (_isInitialized)
                return;

            var results = await CompanyManager.GetAllCompaniesAsync();
            var companies = new Dictionary<uint, Company>(results.Count);
            foreach (var result in results)
            {
                companies.TryAdd(result.CIK, result);
            }
            Companies = companies;
            _isInitialized = true;
        }
        finally
        {
            _initLock.Release();
        }
    }
}
