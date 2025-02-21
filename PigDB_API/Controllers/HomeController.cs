using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PigDB_API.Services;
using PigDB_API.Utils;

namespace PigDB_API.Controllers
{
    [Route("api/")]
    [ApiController]
    public class HomeController : Controller
    {
        private readonly IVariableService _variable;
        private readonly IValidatorService _validator;

        public HomeController(IVariableService variables, IValidatorService validator)
        {
            _variable = variables;
            _validator = validator;
        }

        [HttpGet("")]
        public IActionResult GetTest() => Ok(new { Message = "API 已啟動!" });

        [HttpGet("status")]
        public async Task<IActionResult> GetStatus()
        {
            await Validates();
            return Ok(new
            {
                SharedFolder = new
                {
                    Status = _variable.StatusSharedFolder,
                    Path = _variable.SharedFolder_Path,
                    Message = _variable.StatusSharedFolder ? "已連至共享資料夾" : "共享資料夾取得失敗"
                },
                Model = new
                {
                    Status = _variable.StatusModel,
                    URL = _variable.Model_URL,
                    Message = _variable.StatusModel ? "已連至模型端" : "模型端連接失敗"
                }
            });
        }

        private async Task Validates()
        {
            bool isSharedFolderAccessible = await _validator.IsSharedFolderAccessibleAsync(_variable.SharedFolder_Path);
            bool isModelAccessible = await _validator.IsUrlAccessibleAsync(_variable.Model_URL);

            // 更新共享狀態
            _variable.StatusSharedFolder = isSharedFolderAccessible;
            _variable.StatusModel = isModelAccessible;
        }
    }
}
