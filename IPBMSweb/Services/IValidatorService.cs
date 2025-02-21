using System.Threading.Tasks;

public interface IValidatorService
{
  Task<bool> IsUrlAccessibleAsync(string? url);
}
