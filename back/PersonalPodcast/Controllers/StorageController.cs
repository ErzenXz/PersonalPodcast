using System;
using System.IO;
using System.Security.Permissions;
using System.Threading.Tasks;
using Amazon.S3;
using Amazon.S3.Transfer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace PersonalPodcast.Controllers
{
    [ApiController]
    [Route("files")]
    public class StorageController : ControllerBase
    {
        private readonly IAmazonS3 _s3Client;
        private const string BucketName = "personal-podcast-life-2";

        public StorageController(IAmazonS3 s3Client)
        {
            _s3Client = s3Client;
        }

        [HttpPost, Authorize(Roles = "Admin,SuperAdmin")]
        [Route("upload")]
        public async Task<IActionResult> UploadFile(IFormFile file)
        {
            try
            {
                if (file == null || file.Length == 0)
                    return BadRequest(new { Message = "No file provided.",Code = 55 });

                // Check if the file is img, audio or video if else deny

                if (file.ContentType.Contains("image") || file.ContentType.Contains("audio") || file.ContentType.Contains("video"))
                {
                    // Do nothing
                } else
                {
                    return BadRequest( new { Message = "File type not supported. Please upload an image, audio or video file.", Code = 57 });
                }

                // Check if the file is too large

                if (file.Length > 1073741824)
                {
                    return BadRequest(new { Message = "File is too large. Max file size is 1GB", Code = 56 });
                }

                // Generate a unique key for the S3 object (using Guid)
                var fileExtension = Path.GetExtension(file.FileName);
                var s3ObjectKey = Guid.NewGuid().ToString();

                var s3ObjectKeyWithExtension = $"{s3ObjectKey}{fileExtension}";

                using (var stream = file.OpenReadStream())
                {
                    var transferUtility = new TransferUtility(_s3Client);
                    await transferUtility.UploadAsync(stream, BucketName, s3ObjectKeyWithExtension);
                }

                var s3ObjectUrl = $"https://{BucketName}.s3.amazonaws.com/{s3ObjectKeyWithExtension}";
                return Ok(new { Code = 60, Message = "File uploaded successfully!", S3ObjectUrl = s3ObjectUrl });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error uploading file: {ex.Message}");
            }
        }

        [HttpDelete, Authorize(Roles = "Admin,SuperAdmin")]
        [Route("delete")]
        public async Task<IActionResult> DeleteFile(string key)
        {
            try
            {
                if (string.IsNullOrEmpty(key))
                    return BadRequest(new { Message = "No key provided.", Code = 58 });

                await _s3Client.DeleteObjectAsync(BucketName, key);

                return Ok(new { Code = 59, Message = "File deleted successfully!" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error deleting file: {ex.Message}");
            }
        }
    }
}
