using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace simple_func_pipeline.ChuckNorris;

public interface IChuckNorrisService
{
    Task<ChuckNorrisJoke> GetRandomJokeAsync();
    Task<ChuckNorrisJoke> GetJokeByIdAsync(string id);
    Task<ChuckNorrisJoke> GetRandomJokeFromCategoryAsync(string category);
}

public class ChuckNorrisService : IChuckNorrisService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<ChuckNorrisService> _logger;
    private const string BaseUrl = "https://api.chucknorris.io/jokes";

    public ChuckNorrisService(HttpClient httpClient, ILogger<ChuckNorrisService> logger)
    {
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<ChuckNorrisJoke> GetRandomJokeAsync()
    {
        try
        {
            _logger.LogInformation("Fetching random Chuck Norris joke");
            ChuckNorrisJoke? joke = await _httpClient.GetFromJsonAsync<ChuckNorrisJoke>($"{BaseUrl}/random");
            if (joke is null)
            {
                throw new Exception("error deserializing joke");
            }
            return joke;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching random Chuck Norris joke");
            throw;
        }
    }

    public async Task<ChuckNorrisJoke> GetJokeByIdAsync(string id)
    {
        if (string.IsNullOrEmpty(id))
            throw new ArgumentNullException(nameof(id));

        try
        {
            _logger.LogInformation("Fetching Chuck Norris joke with ID: {JokeId}", id);
            ChuckNorrisJoke? joke = await _httpClient.GetFromJsonAsync<ChuckNorrisJoke>($"{BaseUrl}/{id}");
            if (joke is null)
            {
                throw new Exception("error deserializing joke");
            }
            return joke;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching Chuck Norris joke with ID: {JokeId}", id);
            throw;
        }
    }

    public async Task<ChuckNorrisJoke> GetRandomJokeFromCategoryAsync(string category)
    {
        if (string.IsNullOrEmpty(category))
            throw new ArgumentNullException(nameof(category));

        try
        {
            _logger.LogInformation("Fetching random Chuck Norris joke from category: {Category}", category);
            ChuckNorrisJoke? joke = await _httpClient.GetFromJsonAsync<ChuckNorrisJoke>($"{BaseUrl}/random?category={category}");
            if (joke is null)
            {
                throw new Exception("error deserializing joke");
            }
            return joke;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching random Chuck Norris joke from category: {Category}", category);
            throw;
        }
    }
}
