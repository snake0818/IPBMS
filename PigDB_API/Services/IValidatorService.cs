using System.Threading.Tasks;

public interface IValidatorService
{
  Task<bool> IsUrlAccessibleAsync(string? url);
  Task<bool> IsSharedFolderAccessibleAsync(string? url);
}
