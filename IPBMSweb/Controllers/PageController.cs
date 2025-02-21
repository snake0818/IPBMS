using IPBMSweb.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Hosting;
using System.Diagnostics;
using System.Net.Http.Headers;

namespace IPBMSweb.Controllers
{
    public class PageController : Controller
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<PageController> _logger;
        private readonly IVariableService _variable;
        private readonly string _apiUrl;
        private readonly string _webglUrl;
        private readonly string APIurl_imageMedia;
        private readonly string APIurl_videoMedia;

        public PageController(
            HttpClient httpClient,
            ILogger<PageController> logger,
            IVariableService variableService
        )
        {
            _httpClient = httpClient;
            _logger = logger;
            _variable = variableService;
            _apiUrl = _variable.Api_URL;
            _webglUrl = _variable.Webgl_URL;
            APIurl_imageMedia = $"{_apiUrl}Media/Image/";
            APIurl_videoMedia = $"{_apiUrl}Media/Video/";
        }

        #region 頁面載入

        [HttpGet] public IActionResult Index() => View();
        [HttpGet("/Service/PigSizeEstimation")] public IActionResult Estimate() => View("PigSizeEstimation");
        [HttpGet("/Service/PigActivityTracking")] public IActionResult Tracking() => View("PigActivityTracking");
        [HttpGet("/Service/WebGL")] public IActionResult DigitalTwin() => Redirect(_webglUrl);
        [HttpGet("/ToBeContinue")] public IActionResult ToBeContinue() => View("ToBeContinue");
        [HttpGet("/SystemDesign")] public IActionResult SystemDesign() => View("systemDesign");
        [HttpGet("/ResearchProcess")] public IActionResult ResearchProcess() => View("researchProcess");
        [HttpGet("/ResearchResult")] public IActionResult ResearchResult() => View("researchResult");
        [HttpGet("/Papers")] public IActionResult Papers() => View("papers");
        [HttpGet("/DevTools")] public IActionResult DevTools() { return View("devTools"); }

        #region 錯誤處理
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error() { return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier }); }
        #endregion

        #endregion

        #region 取得媒體檔案
        [HttpGet("Image/{imageId}")]
        public async Task<IActionResult> GetImage(int imageId) => await _GetMediaAsync(APIurl_imageMedia, imageId, "image/jpeg");

        [HttpGet("Video/{videoId}")]
        public async Task<IActionResult> GetVideo(int videoId) => await _GetMediaAsync(APIurl_videoMedia, videoId, "video/mp4");
        #endregion

        #region 上傳媒體檔案
        [HttpPost("Image")]
        public async Task<IActionResult> UploadImage(IFormFile file) => await _UploadMediaAsync(APIurl_imageMedia, file);

        [HttpPost("Video")]
        public async Task<IActionResult> UploadVideo(IFormFile file) => await _UploadMediaAsync(APIurl_videoMedia, file);
        #endregion

        #region 私有共用方法
        private async Task<IActionResult> _GetMediaAsync(string apiUrl, int mediaId, string contentType)
        {
            try
            {
                var res = await _httpClient.GetAsync($"{apiUrl}{mediaId}");
                if (!res.IsSuccessStatusCode)
                {
                    _logger.LogError($"Status Code: {res.StatusCode}, Failed to get media from {apiUrl}{mediaId}");
                    return StatusCode((int)res.StatusCode, "Failed to retrieve media file.");
                }

                var mediaStream = await res.Content.ReadAsStreamAsync();
                return File(mediaStream, contentType);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error while fetching media: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }

        private async Task<IActionResult> _UploadMediaAsync(string apiUrl, IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("未選擇上傳檔案");

            using var content = new MultipartFormDataContent();
            using var ms = new MemoryStream();
            await file.CopyToAsync(ms);
            content.Add(new ByteArrayContent(ms.ToArray()) { Headers = { ContentType = new MediaTypeHeaderValue(file.ContentType) } }, "file", file.FileName);

            try
            {
                var res = await _httpClient.PostAsync(apiUrl, content);
                if (!res.IsSuccessStatusCode)
                {
                    _logger.LogError($"Failed to upload media to {apiUrl}, Status Code: {res.StatusCode}");
                    return StatusCode((int)res.StatusCode, "Failed to upload media file.");
                }

                var result = await res.Content.ReadAsStringAsync();
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error while uploading media: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }
        #endregion

    }
}
