using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using System.Text.Json;
using ProgettoAnagrafiche.Models;

namespace WPFClientProgettoAnagrafiche.Services.Api
{
    public class ApiService
    {
        private readonly HttpClient _httpClient;

        public ApiService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<ApiResponse<T>> GetAsync<T>(string endpoint)
        {
            try
            {
                var response = await _httpClient.GetAsync(endpoint);
                return await HandleResponse<ApiResponse<T>>(response);
            }
            catch (HttpRequestException ex)
            {
                throw new InvalidOperationException(
                    $"API GET request failed for endpoint '{endpoint}': {ex.Message}", ex);
            }
            catch (JsonException ex)
            {
                throw new InvalidOperationException(
                    $"Failed to deserialize API response from '{endpoint}': {ex.Message}", ex);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException(
                    $"Unexpected error during API GET request to '{endpoint}': {ex.Message}", ex);
            }
        }

        public async Task<ApiResponse<T>> PostAsync<T>(string endpoint, object data)
        {
            try
            {
                var json = JsonSerializer.Serialize(data);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync(endpoint, content);
                return await HandleResponse<ApiResponse<T>>(response);
            }
            catch (HttpRequestException ex)
            {
                throw new InvalidOperationException(
                    $"API POST request failed for endpoint '{endpoint}': {ex.Message}", ex);
            }
            catch (JsonException ex)
            {
                throw new InvalidOperationException(
                    $"Failed to serialize or deserialize data for '{endpoint}': {ex.Message}", ex);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException(
                    $"Unexpected error during API POST request to '{endpoint}': {ex.Message}", ex);
            }
        }

        public async Task<ApiResponse> PostAsync(string endpoint, object data)
        {
            try
            {
                var json = JsonSerializer.Serialize(data);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync(endpoint, content);
                return await HandleResponse<ApiResponse>(response);
            }
            catch (HttpRequestException ex)
            {
                throw new InvalidOperationException(
                    $"API POST request failed for endpoint '{endpoint}': {ex.Message}", ex);
            }
            catch (JsonException ex)
            {
                throw new InvalidOperationException(
                    $"Failed to serialize or deserialize data for '{endpoint}': {ex.Message}", ex);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException(
                    $"Unexpected error during API POST request to '{endpoint}': {ex.Message}", ex);
            }
        }

        public async Task<ApiResponse<T>> PutAsync<T>(string endpoint, object data)
        {
            try
            {
                var json = JsonSerializer.Serialize(data);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PutAsync(endpoint, content);
                return await HandleResponse<ApiResponse<T>>(response);
            }
            catch (HttpRequestException ex)
            {
                throw new InvalidOperationException(
                    $"API PUT request failed for endpoint '{endpoint}': {ex.Message}", ex);
            }
            catch (JsonException ex)
            {
                throw new InvalidOperationException(
                    $"Failed to serialize or deserialize data for '{endpoint}': {ex.Message}", ex);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException(
                    $"Unexpected error during API PUT request to '{endpoint}': {ex.Message}", ex);
            }
        }

        public async Task<ApiResponse> DeleteAsync(string endpoint)
        {
            try
            {
                var response = await _httpClient.DeleteAsync(endpoint);
                return await HandleResponse<ApiResponse>(response);
            }
            catch (HttpRequestException ex)
            {
                throw new InvalidOperationException(
                    $"API DELETE request failed for endpoint '{endpoint}': {ex.Message}", ex);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException(
                    $"Unexpected error during API DELETE request to '{endpoint}': {ex.Message}", ex);
            }
        }

        public async Task<ApiResponse<bool>> ConvertAsync(string endpoint)
        {
            try
            {
                var response = await _httpClient.PostAsync(endpoint, new StringContent(string.Empty, Encoding.UTF8, "application/json"));
                return await HandleResponse<ApiResponse<bool>>(response);
            }
            catch (HttpRequestException ex)
            {
                throw new InvalidOperationException(
                    $"API CONVERT request failed for endpoint '{endpoint}': {ex.Message}", ex);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException(
                    $"Unexpected error during API CONVERT request to '{endpoint}': {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Handles HTTP response and deserializes content
        /// </summary>
        private async Task<T> HandleResponse<T>(HttpResponseMessage response)
        {
            var content = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                throw new InvalidOperationException(
                    $"API request failed with status code {response.StatusCode}. " +
                    $"Response: {content}");
            }

            // If content is empty, throw exception instead of returning default
            if (string.IsNullOrWhiteSpace(content))
            {
                throw new InvalidOperationException(
                    $"API response is empty. This may indicate the API endpoint returned no data.");
            }

            try
            {
                var result = JsonSerializer.Deserialize<T>(content, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                // Ensure result is not null
                if (result == null)
                {
                    throw new InvalidOperationException(
                        $"Failed to deserialize API response. The response content was: {content}");
                }

                return result;
            }
            catch (JsonException ex)
            {
                throw new InvalidOperationException(
                    $"Failed to parse JSON response: {ex.Message}. Content: {content}", ex);
            }
        }
    }
}