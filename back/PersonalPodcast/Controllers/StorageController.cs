using System;
using System.IO;
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

        [HttpPost, Authorize(Roles = "Admin")]
        [Route("upload")]
        public async Task<IActionResult> UploadFile(IFormFile file)
        {
            try
            {
                if (file == null || file.Length == 0)
                    return BadRequest("No file provided.");

                // Check if the file is img, audio or video if else deny

                if (file.ContentType.Contains("image") || file.ContentType.Contains("audio") || file.ContentType.Contains("video"))
                {
                    // Do nothing
                } else
                {
                    return BadRequest( new { Message = "File type not supported. Please upload an image, audio or video file.", Code = 5 });
                }

                // Check if the file is too large

                if (file.Length > 1073741824)
                {
                    return BadRequest("File is too large. Max file size is 1GB");
                }

                // Generate a unique key for the S3 object (e.g., using Guid)
                var s3ObjectKey = Guid.NewGuid().ToString();

                using (var stream = file.OpenReadStream())
                {
                    var transferUtility = new TransferUtility(_s3Client);
                    await transferUtility.UploadAsync(stream, BucketName, s3ObjectKey);
                }

                var s3ObjectUrl = $"https://{BucketName}.s3.amazonaws.com/{s3ObjectKey}";
                return Ok(new { Code = 7, Message = "File uploaded successfully!", S3ObjectUrl = s3ObjectUrl });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error uploading file: {ex.Message}");
            }
        }
    }
}
