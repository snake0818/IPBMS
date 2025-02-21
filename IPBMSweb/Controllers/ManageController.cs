using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Threading.Tasks;

namespace IPBMSweb.Controllers
{
  [Route("manage")]
  public class ManageController(IVariableService variableService, IValidatorService validator) : Controller
  {
    private readonly IVariableService _variable = variableService;
    private readonly IValidatorService _validator = validator;

    [HttpGet("status")]
    public async Task<IActionResult> GetStatus()
    {
      await Validates();
      return Ok(new
      {
        API_Status = _variable.StatusApi,
        API_URL = _variable.Api_URL,
        WebGL_Status = _variable.StatusWebgl,
        WebGL_URL = _variable.Webgl_URL,
      });
    }
    
    private async Task Validates()
    {
      bool isApiAccessible = await _validator.IsUrlAccessibleAsync(_variable.Api_URL);
      bool isWebglAccessible = await _validator.IsUrlAccessibleAsync(_variable.Webgl_URL);

      // 更新共享狀態
      _variable.StatusApi = isApiAccessible;
      _variable.StatusWebgl = isWebglAccessible;
    }
  }
}