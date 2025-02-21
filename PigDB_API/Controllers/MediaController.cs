using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PigDB_API.Data;
using PigDB_API.Models;
using PigDB_API.Services;
using PigDB_API.Utils;

namespace PigDB_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MediaController : Controller
    {
        #region 建構

        private readonly PigDBContext _context; // 宣告 PigDB 物件變數
        private readonly IVariableService _variable;
        private readonly string _imageFolderPath;
        private readonly string _videoFolderPath;

        public MediaController(PigDBContext database, IVariableService variables)
        {
            _context = database;
            _variable = variables;

            var controllerName = GetType().Name.Replace("Controller", "");
            var BaseFolderPath = _variable.StatusSharedFolder ? _variable.SharedFolder_Path : _variable.LocalFolder_Path;
            var folderPath = Path.Combine(BaseFolderPath, "Sources", controllerName);
            _imageFolderPath = Path.Combine(folderPath, "Images");
            _videoFolderPath = Path.Combine(folderPath, "Videos");
            Shared.EnsurePathExists(_imageFolderPath);
            Shared.EnsurePathExists(_videoFolderPath);
        }

        #endregion

        #region 圖片處理
        #region 取得圖像清單
        [HttpGet("Image/List")]
        public async Task<IActionResult> GetImageList()
        {
            try
            {
                // 查詢資料庫中的圖片記錄
                var query = await _context.Images
                    .Select(m => new
                    {
                        m.Id,
                        m.Timestamp,
                        FileName = Path.GetFileName(m.FilePath),
                    }).ToListAsync();
                // 檢查是否存在，並返回適當的結果
                return query.Count != 0 ? Ok(query) : NotFound("圖像清單不存在!");
            }
            // 捕捉例外並回傳 500 狀態碼
            catch (Exception ex) { return StatusCode(500, $"取得圖像清單時發生錯誤: {ex.Message}"); }
        }
        #endregion

        #region 傳入圖片
        [HttpPost("Image")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> UploadImage(IFormFile File)
        {
            if (File == null || File.Length == 0) return BadRequest("沒有檔案被上傳!");
            try // 儲存檔案與記錄
            {
                // 儲存檔案並取得路徑
                string filePath = await Shared.CopyFileStream(File, _imageFolderPath);

                // 儲存紀錄到資料庫
                var newImage = new Image
                {
                    FilePath = filePath,
                    Timestamp = Shared.UnixTime()
                };
                _context.Images.Add(newImage);
                await _context.SaveChangesAsync();

                return Ok(new { Message = "圖像上傳成功!", ImageId = newImage.Id });
            }
            // 捕捉例外並回傳 500 狀態碼
            catch (Exception ex) { return StatusCode(500, $"上傳圖片時發生錯誤: {ex.Message}"); }
        }
        #endregion

        #region 取得圖片
        [HttpGet("Image/{Image_id}")]
        public async Task<IActionResult> GetImage(int Image_id)
        {
            try
            {
                // 查詢資料庫中的圖片記錄
                var record = await _context.Images.FirstOrDefaultAsync(f => f.Id == Image_id);
                if (record == null) return NotFound("該圖片不存在!");
                return PhysicalFile(record.FilePath, "image/jpeg");
            }
            // 捕捉例外並回傳 500 狀態碼
            catch (Exception ex) { return StatusCode(500, $"取得圖片時發生錯誤: {ex.Message}"); }
        }
        #endregion
        #endregion

        #region 影片處理
        #region 取得影像清單
        [HttpGet("Video/List")]
        public async Task<IActionResult> GetVideoList()
        {
            try
            {
                // 查詢資料庫中的影片記錄
                var query = await _context.Videos
                    .Select(m => new
                    {
                        m.Id,
                        m.Timestamp,
                        FileName = Path.GetFileName(m.FilePath),
                    }).ToListAsync();
                // 檢查是否存在，並返回適當的結果
                return query.Count != 0 ? Ok(query) : NotFound("影像清單不存在!");
            }
            // 捕捉例外並回傳 500 狀態碼
            catch (Exception ex) { return StatusCode(500, $"取得影像清單時發生錯誤: {ex.Message}"); }
        }
        #endregion

        #region 傳入影片
        [HttpPost("Video")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> UploadVideo(IFormFile File)
        {
            if (File == null || File.Length == 0) return BadRequest("沒有檔案被上傳!");
            try // 儲存檔案與記錄
            {
                // 儲存檔案並取得路徑
                string filePath = await Shared.CopyFileStream(File, _videoFolderPath);

                // 儲存紀錄到資料庫
                var newVideo = new Video
                {
                    FilePath = filePath,
                    Timestamp = Shared.UnixTime()
                };
                _context.Videos.Add(newVideo);
                await _context.SaveChangesAsync();

                return Ok(new { Message = "影像上傳成功!", VideoId = newVideo.Id });
            }

            // 捕捉例外並回傳 500 狀態碼
            catch (Exception ex) { return StatusCode(500, $"上傳影片時發生錯誤: {ex.Message}"); }
        }
        #endregion

        #region 取得影片
        [HttpGet("Video/{Video_id}")]
        public async Task<IActionResult> GetVideo(int Video_id)
        {
            try
            {
                // 查詢資料庫中的影片記錄
                var record = await _context.Videos.FirstOrDefaultAsync(f => f.Id == Video_id);
                if (record == null) return NotFound("影片不存在!");
                // 回傳影片串流
                return Shared.StreamVideo(record.FilePath);
            }
            // 捕捉例外並回傳 500 狀態碼
            catch (Exception ex) { return StatusCode(500, $"取得影片時發生錯誤: {ex.Message}"); }
        }
        #endregion
        #endregion
    }
}
