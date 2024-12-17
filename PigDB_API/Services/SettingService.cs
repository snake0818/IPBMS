namespace PigDB_API.Services
{
  public class SettingService
  {
    public string BasePath { get; private set; }
    public string ModelUrl { get; private set; }
    public bool IsFolderConnected { get; private set; }
    public bool IsModelConnected { get; private set; }
    private readonly IConfiguration _configuration;
    private static readonly HttpClient _httpClient = new HttpClient();

    public SettingService(IConfiguration configuration)
    {
      _configuration = configuration;
      BasePath = LoadBasePath();
      ModelUrl = "|";//string.Empty;
      Log();
    }

    public async Task InitializeAsync()
    {
      BasePath = LoadBasePath();
      ModelUrl = await LoadModelUrl();
      Log();
    }

    // 載入共享資料夾路徑及模型端連接路徑
    private string LoadBasePath()
    {
      var appSettingsBasePath = _configuration["BasePath"];
      var (isConnected, path) = CheckPath(appSettingsBasePath);
      IsFolderConnected = isConnected;
      return path;
    }
    private async Task<string> LoadModelUrl()
    {
      var appSettingsModelURL = _configuration["ModelUrl"];
      var (isConnected, url) = await CheckConnection(appSettingsModelURL);
      IsModelConnected = isConnected;
      return url;
    }

    // 檢查共享資料夾狀態及路徑
    private static (bool, string) CheckPath(string? PATH)
    {
      bool status = false;
      string path = Path.Combine(AppContext.BaseDirectory, "PigDB");

      if (!string.IsNullOrEmpty(PATH) && Directory.Exists(PATH))
      {
        status = true;
        path = PATH;
      }

      Directory.CreateDirectory(path);
      return (status, path);
    }

    // 檢查模型端連接狀態及路徑
    private static async Task<(bool, string)> CheckConnection(string? URL)
    {
      bool status = false;
      string url = "|";

      if (!string.IsNullOrEmpty(URL))
      {
        try
        {
          HttpResponseMessage response = await _httpClient.GetAsync(URL);
          status = true;
          url = URL;
        }
        catch (HttpRequestException ex) { Console.WriteLine($"連線錯誤：{ex.Message}"); }
      }

      return (status, url);
    }

    // 重新檢驗連接並回傳是否發生改變
    public bool ReloadBaseConnect()
    {
      BasePath = LoadBasePath();
      Log();
      return IsFolderConnected;
    }
    public async Task<bool> ReloadModelConnect()
    {
      ModelUrl = await LoadModelUrl();
      Log();
      return IsModelConnected;
    }

    // 添加日誌以確認
    private void Log()
    {
      Console.WriteLine($"FolderConnected: {IsFolderConnected}, BasePath: {BasePath}");
      Console.WriteLine($"ModelConnected:  {IsModelConnected}, ModelUrl: {ModelUrl}");
    }

  }
}