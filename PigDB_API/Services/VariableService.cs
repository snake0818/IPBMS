using Microsoft.Extensions.Configuration;

public class VariableService(IConfiguration configuration) : IVariableService
{
    public bool StatusSharedFolder { get; set; } = false;
    public string SharedFolder_Path { get; set; } = configuration["AppSettings:Path_SharedFolder"] ?? "";
    public string LocalFolder_Path { get; set; } = Path.Combine(AppContext.BaseDirectory, "PigDB");
    public bool StatusModel { get; set; } = false;
    public string Model_URL { get; set; } = configuration["AppSettings:URL_Model"] ?? "";
}