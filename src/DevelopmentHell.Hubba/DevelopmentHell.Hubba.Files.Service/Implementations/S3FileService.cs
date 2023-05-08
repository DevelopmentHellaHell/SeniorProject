using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Transfer;
using DevelopmentHell.Hubba.Files.Service.Abstractions;
using DevelopmentHell.Hubba.Models;
using DevelopmentHell.Hubba.Logging.Service.Abstractions;
using Microsoft.AspNetCore.Http;
using static System.Net.WebRequestMethods;

namespace DevelopmentHell.Hubba.Files.Service.Implementations
{
    public class S3FileService : IFileService
    {
        private readonly string _bucketName;
        private readonly ILoggerService _loggerService;
        private readonly AmazonS3Client _s3Client;
        private readonly TransferUtility _transferUtility;

        public S3FileService(string bucketName, string accessKeyId, string secretKey, ILoggerService loggerService)
        {
            _bucketName = bucketName;
            _loggerService = loggerService;

            _s3Client = new AmazonS3Client(accessKeyId, secretKey, Amazon.RegionEndpoint.USEast1);
            _transferUtility = new TransferUtility(_s3Client);
        }

        public async Task<Result> CreateDir(string dirPath)
        {
            try
            {
                var putObjectRequest = new PutObjectRequest
                {
                    BucketName = _bucketName,
                    Key = dirPath.EndsWith("/") ? dirPath : dirPath + "/",
                    ContentBody = string.Empty
                };
                await _s3Client.PutObjectAsync(putObjectRequest).ConfigureAwait(false);
                return Result.Success();
            }
            catch (Exception ex)
            {
                return Result.Failure($"Unable to create Directory: {ex.Message}");
            }
        }

        public async Task<Result> DeleteDir(string dirPath)
        {
            try
            {
                var listObjectsRequest = new ListObjectsV2Request
                {
                    BucketName = _bucketName,
                    Prefix = dirPath.EndsWith("/") ? dirPath : dirPath + "/"
                };
                var listObjectsResponse = await _s3Client.ListObjectsV2Async(listObjectsRequest).ConfigureAwait(false);
                var deleteObjectsRequest = new DeleteObjectsRequest { BucketName = _bucketName };

                deleteObjectsRequest.Objects.AddRange(
                    listObjectsResponse.S3Objects.Select(obj => new KeyVersion { Key = obj.Key }));

                await _s3Client.DeleteObjectsAsync(deleteObjectsRequest).ConfigureAwait(false);
                return Result.Success();
            }
            catch (Exception ex)
            {
                return Result.Failure($"Unable to delete Directory: {ex.Message}");
            }
        }

        public async Task<Result> DeleteFile(string filePath)
        {
            try
            {
                var deleteObjectRequest = new DeleteObjectRequest
                {
                    BucketName = _bucketName,
                    Key = filePath
                };
                await _s3Client.DeleteObjectAsync(deleteObjectRequest).ConfigureAwait(false);
                return Result.Success();
            }
            catch (Exception ex)
            {
                return Result.Failure($"Failed to delete file: {ex.Message}");
            }
        }

        public Task<Result> Disconnect()
        {
            // Not required for Amazon S3
            return Task.FromResult(Result.Success());
        }

        public async Task<Result<string>> GetFileReference(string filePath)
        {
            try
            {
                var request = new GetObjectMetadataRequest
                {
                    BucketName = _bucketName,
                    Key = filePath
                };
                await _s3Client.GetObjectMetadataAsync(request).ConfigureAwait(false);
                var baseUrl = $"https://{_bucketName}.s3.amazonaws.com/";
                return Result<string>.Success(baseUrl + filePath);
            }
            catch (Exception)
            {
                return Result<string>.Failure("Failed to Check for file existence");
            }
        }

        public async Task<Result<List<string>>> GetFilesInDir(string dirPath)
        {
            try
            {
                var listObjectsRequest = new ListObjectsV2Request
                {
                    BucketName = _bucketName,
                    Prefix = dirPath.EndsWith("/") ? dirPath : dirPath + "/"
                };
                var listObjectsResponse = await _s3Client.ListObjectsV2Async(listObjectsRequest).ConfigureAwait(false);
                var outFiles = new List<string>();
                foreach (var obj in listObjectsResponse.S3Objects)
                {
                    if (obj.Key.Equals(dirPath.EndsWith("/") ? dirPath : dirPath + "/"))
                    {
                        continue;
                    }
                    outFiles.Add($"https://{_bucketName}.s3.amazonaws.com/{obj.Key}");
                }
                return Result<List<string>>.Success(outFiles);
            }
            catch (Exception ex)
            {
                return Result<List<string>>.Failure($"Failed to get files in directory: {ex.Message}");
            }
        }

        public async Task<Result> UploadDir(string dirPath, List<Tuple<string, byte[]>> fileNameData)
        {
            var putTasks = new List<Task>();
            foreach (var file in fileNameData)
            {
                var request = new TransferUtilityUploadRequest
                {
                    BucketName = _bucketName,
                    Key = $"{dirPath}/{file.Item1}",
                    InputStream = new MemoryStream(file.Item2)
                };
                putTasks.Add(_transferUtility.UploadAsync(request));
            }

            try
            {
                await Task.WhenAll(putTasks).ConfigureAwait(false);
                return Result.Success();
            }
            catch (Exception ex)
            {
                return Result.Failure($"Unable to upload one or more of the files provided: {ex.Message}");
            }
        }

        public async Task<Result> UploadFile(string filePath, string fileName, byte[] fileData)
        {
            try
            {
                var request = new TransferUtilityUploadRequest
                {
                    BucketName = _bucketName,
                    Key = $"{filePath}/{fileName}",
                    InputStream = new MemoryStream(fileData)
                };
                await _transferUtility.UploadAsync(request).ConfigureAwait(false);
                return Result.Success();
            }
            catch (Exception ex)
            {
                return Result.Failure($"Upload failed: {ex.Message}");
            }
        }

        public async Task<Result> UploadIFormFile(string filePath, string fileName, IFormFile file)
        {
            try
            {
                var request = new TransferUtilityUploadRequest
                {
                    BucketName = _bucketName,
                    Key = $"{filePath}/{fileName}",
                    InputStream = file.OpenReadStream()
                };
                await _transferUtility.UploadAsync(request).ConfigureAwait(false);
                return Result.Success();
            }
            catch (Exception ex)
            {
                return Result.Failure($"Upload failed: {ex.Message}");
            }
        }
    }
}