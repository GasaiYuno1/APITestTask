using APITestTask.Services;
using Microsoft.AspNetCore.Mvc;

namespace APITestTask.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class AdPlatformsController : ControllerBase
    {
        private readonly IAdPlatformService _adPlatformService;

        public AdPlatformsController(IAdPlatformService adPlatformService)
        {
            _adPlatformService = adPlatformService;
        }

        /// <summary>
        /// Upload ad platforms data from a file
        /// </summary>
        /// <param name="file">Text file containing ad platform data in format: PlatformName:/location1,/location2</param>
        /// <returns>Success message</returns>
        [HttpPost("upload")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> UploadAdPlatforms(IFormFile? file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("File is required");

            try
            {
                using var reader = new StreamReader(file.OpenReadStream());
                var content = await reader.ReadToEndAsync();
                await _adPlatformService.LoadAdPlatformsFromContentAsync(content);
                return Ok(new { message = "Ad platforms loaded successfully" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        /// <summary>
        /// Upload ad platforms data from text content
        /// </summary>
        /// <param name="content">Text content with ad platform data in format: "PlatformName:/location1,/location2\nPlatformName2:/loc1,loc2"</param>
        /// <returns>Success message</returns>
        [HttpPost("upload-content")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> UploadAdPlatformsFromContent([FromBody] string content)
        {
            if (string.IsNullOrWhiteSpace(content))
                return BadRequest("Content is required");

            try
            {
                await _adPlatformService.LoadAdPlatformsFromContentAsync(content);
                return Ok(new { message = "Ad platforms loaded successfully" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        /// <summary>
        /// Search for ad platforms available in a specific location
        /// </summary>
        /// <param name="location">Location path (e.g., /ru/svrd/revda)</param>
        /// <returns>List of ad platforms available in the specified location</returns>
        [HttpGet("search")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public IActionResult SearchAdPlatforms([FromQuery] string location)
        {
            if (string.IsNullOrWhiteSpace(location))
                return BadRequest("Location parameter is required");

            try
            {
                var platforms = _adPlatformService.FindAdPlatformsForLocation(location);
                return Ok(new { location, platforms });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }
    }
}
