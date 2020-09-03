using AwsTutorial.Models;
using System.Threading.Tasks;

namespace AwsTutorial.Services
{
    public interface IS3Service
    {
        Task<S3Response> CreateS3BucketAsync(string bucketName);

        Task UploadFileToS3BucketAsync(string bucketName);

        Task GetObjectFromS3Bucket(string bucketName);
        Task DeleteObjectFromS3Bucket(string bucketName, string fileName);
    }
}