using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AwsTutorial.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AwsTutorial.Controllers
{
    public class S3BucketController : ControllerBase
    {
        private readonly IS3Service _service;

        public S3BucketController(IS3Service service)
        {
            _service = service;
        }

        [HttpPost]
        [Route("api/s3bucket/{bucketName}")]
        public async Task<IActionResult> CreateS3Bucket([FromRoute] string bucketName)
        {
            var response = await _service.CreateS3BucketAsync(bucketName);
            return Ok(response);
        }

        [HttpPost]
        [Route("api/s3bucket/upload/{bucketName}")]
        public async Task<IActionResult> UploadFileToS3Bucket([FromRoute] string bucketName)
        {
            await _service.UploadFileToS3BucketAsync(bucketName);
            return Ok();
        }

        [HttpPost]
        [Route("api/s3bucket/getfile/{bucketName}")]
        public async Task<IActionResult> GetObjectFromS3Bucket([FromRoute] string bucketName)
        {
            await _service.GetObjectFromS3Bucket(bucketName);
            return Ok();
        }

        [HttpDelete]
        [Route("api/s3bucket/{bucketName}/deletefile/{fileName}")]
        public async Task<IActionResult> GetObjectFromS3Bucket([FromRoute] string bucketName, [FromRoute] string fileName)
        {
            await _service.DeleteObjectFromS3Bucket(bucketName, fileName);
            return Ok();
        }
    }
}