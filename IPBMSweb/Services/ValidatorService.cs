using System;
using System.Net.Http;
using System.Threading.Tasks;

public class ValidatorService(HttpClient httpClient, ILogger<ValidatorService> logger) : IValidatorService
{
  private readonly HttpClient _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
  private readonly ILogger<ValidatorService> _logger = logger;

  public async Task<bool> IsUrlAccessibleAsync(string? url)
  {
    if (string.IsNullOrWhiteSpace(url))
      throw new ArgumentException("URL 為空或只包含空白字元!", nameof(url));

    if (!Uri.TryCreate(url, UriKind.Absolute, out _))
      throw new ArgumentException("URL 格式無效!", nameof(url));

    try
    {
      using var request = new HttpRequestMessage(HttpMethod.Head, url);
      using var response = await _httpClient.SendAsync(request);
      if (response.IsSuccessStatusCode) return true;

      // 若 HEAD 失敗，改用 GET 嘗試
      request.Method = HttpMethod.Get;
      using var fallbackResponse = await _httpClient.SendAsync(request);
      return fallbackResponse.IsSuccessStatusCode;
    }
    catch (Exception ex)
    {
      _logger.LogError($"檢查 URL 時發生錯誤: {ex.Message}");
      return false;
    }
  }
}
