using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Transfer;
using Amazon.S3.Util;
using AwsTutorial.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace AwsTutorial.Services
{
    public class S3Service : IS3Service
    {
        private readonly IAmazonS3 _client;

        public S3Service(IAmazonS3 client)
        {
            _client = client;
        }

        public async Task<S3Response> CreateS3BucketAsync(string bucketName)
        {
            try
            {
                if (await AmazonS3Util.DoesS3BucketExistV2Async(_client, bucketName) == false)
                {
                    PutBucketRequest putBucketRequest = new PutBucketRequest()
                    {
                        BucketName = bucketName,
                        UseClientRegion = true
                    };

                    var response = await _client.PutBucketAsync(putBucketRequest);

                    return new S3Response()
                    {
                        Message = response.ResponseMetadata.RequestId,
                        Status = response.HttpStatusCode
                    };
                }
            }
            catch (AmazonS3Exception e)
            {
                return new S3Response()
                {
                    Message = e.Message,
                    Status = e.StatusCode
                };
            }
            catch (Exception e)
            {
                return new S3Response()
                {
                    Message = e.Message,
                    Status = System.Net.HttpStatusCode.InternalServerError
                };
            }

            return new S3Response()
            {
                Message = "Something went wrong!",
                Status = System.Net.HttpStatusCode.InternalServerError
            };
        }

        public async Task DeleteObjectFromS3Bucket(string bucketName, string fileName)
        {
            var request = new DeleteObjectRequest
            {
                BucketName = bucketName,
                Key = fileName
            };

            await _client.DeleteObjectAsync(request);
        }

        public async Task GetObjectFromS3Bucket(string bucketName)
        {
            string keyName = "TestFile.txt";

            try
            {
                var request = new GetObjectRequest()
                {
                    BucketName = bucketName,
                    Key = keyName
                };

                string responseBody;

                using (var response = await _client.GetObjectAsync(request))
                using (var responseStream = response.ResponseStream)
                using (var reader = new StreamReader(responseStream))
                {
                    var title = response.Metadata["x-amz-meta-title"];
                    var contentType = response.Headers["Content-Type"];

                    Console.WriteLine($"Object meta, Title: {title}");
                    Console.WriteLine($"Content type, Title: {contentType}");

                    responseBody = reader.ReadToEnd();
                }

                var pathAndFileName = $"C:\\Users\\patel_pi\\Desktop\\Aws Self Learning\\GetObjectFromS3\\{keyName}";

                File.WriteAllText(pathAndFileName, responseBody);
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task UploadFileToS3BucketAsync(string bucketName)
        {
            string path = "C:\\Users\\patel_pi\\Desktop\\Aws Self Learning\\TestFile.txt";

            try
            {
                var fileTransferUtility = new TransferUtility(_client);

                //Option-1
                await fileTransferUtility.UploadAsync(path, bucketName);

                //Option-2
                await fileTransferUtility.UploadAsync(path, bucketName, "UploadWithKeyName");

                //Option-3
                using (var fileToUpload = new FileStream(path, FileMode.Open, FileAccess.Read))
                {
                    await fileTransferUtility.UploadAsync(fileToUpload, bucketName, "FileStreamUpload");
                }

                //Option-4
                var fileTrasnferUtilityRequest = new TransferUtilityUploadRequest()
                {
                    BucketName = bucketName,
                    FilePath = path,
                    StorageClass = S3StorageClass.Standard,
                    PartSize = 6291456,
                    CannedACL = S3CannedACL.NoACL,
                    Key = "AdvancedUpload"
                };

                fileTrasnferUtilityRequest.Metadata.Add("param1", "value1");
                fileTrasnferUtilityRequest.Metadata.Add("param2", "value2");

                await fileTransferUtility.UploadAsync(fileTrasnferUtilityRequest);
            }
            catch (AmazonS3Exception e)
            {
                Console.WriteLine("Exception" + e.Message);
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception" + e.Message);
            }
        }
    }
}
