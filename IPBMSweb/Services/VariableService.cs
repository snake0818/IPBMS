using Microsoft.Extensions.Configuration;

public class VariableService(IConfiguration configuration) : IVariableService
{
    public bool StatusApi { get; set; } = false;
    public string Api_URL { get; set; } = configuration["AppSettings:URL_API"] ?? "";
    public bool StatusWebgl { get; set; } = false;
    public string Webgl_URL { get; set; } = configuration["AppSettings:URL_WebGL"] ?? "";
}