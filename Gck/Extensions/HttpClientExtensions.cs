using System.Net.Http.Json;

namespace Gck.Extensions;
public static class HttpClientExtensions
{
    /// <summary>
    /// Sends a GET request and ensures the response is successful, throwing an exception if not.
    /// </summary>
    public static async Task<T?> GetFromJsonWithExceptionAsync<T>(this HttpClient client, string? requestUri)
    {
        var response = await client.GetAsync(requestUri);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<T>();
    }

    /// <summary>
    /// Sends a POST request and ensures the response is successful, throwing an exception if not.
    /// </summary>
    public static async Task<HttpResponseMessage> PostAsJsonWithExceptionAsync<T>(this HttpClient client, string? requestUri, T value)
    {
        var response = await client.PostAsJsonAsync(requestUri, value);
        response.EnsureSuccessStatusCode();
        return response;
    }

    /// <summary>
    /// Sends a POST request and ensures the response is successful, returning the deserialized response.
    /// </summary>
    public static async Task<TResponse?> PostAsJsonWithExceptionAsync<TRequest, TResponse>(this HttpClient client, string? requestUri, TRequest value)
    {
        var response = await client.PostAsJsonAsync(requestUri, value);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<TResponse>();
    }

    /// <summary>
    /// Sends a PUT request and ensures the response is successful, throwing an exception if not.
    /// </summary>
    public static async Task<HttpResponseMessage> PutAsJsonWithExceptionAsync<T>(this HttpClient client, string? requestUri, T value)
    {
        var response = await client.PutAsJsonAsync(requestUri, value);
        response.EnsureSuccessStatusCode();
        return response;
    }

    /// <summary>
    /// Sends a DELETE request and ensures the response is successful, throwing an exception if not.
    /// </summary>
    public static async Task<HttpResponseMessage> DeleteWithExceptionAsync(this HttpClient client, string? requestUri)
    {
        var response = await client.DeleteAsync(requestUri);
        response.EnsureSuccessStatusCode();
        return response;
    }
}
