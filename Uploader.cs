using Amazon;
using Amazon.S3;
using Amazon.S3.Transfer;
using System;
using System.IO;
using System.Threading.Tasks;

namespace AWS_Service
{
    public static class Uploader
    {
        private static IAmazonS3 s3Client;

        /**
         *  string bucketName = "*** provide bucket name ***";
         *  string keyName = "*** provide a name for the uploaded object ***";
         *  string filePath = "*** provide the full path name of the file to upload ***";
         *  RegionEndpoint bucketRegion = *** Specify your bucket region *** Ex. RegionEndpoint.USWest2;
         */
        public static void Upload (string _bucketName, string _keyName, string _filePath, RegionEndpoint _bucketRegion)
        {
            s3Client = new AmazonS3Client(_bucketRegion);
            UploadFileAsync(_bucketName, _keyName, _filePath).Wait();
        }

        private static async Task UploadFileAsync(string _bucketName, string _keyName, string _filePath)
        {
            try
            {
                var fileTransferUtility =
                    new TransferUtility(s3Client);

                // Option 1. Upload a file. The file name is used as the object key name.
                await fileTransferUtility.UploadAsync(_filePath, _bucketName);
                Console.WriteLine("Upload 1 completed");

                // Option 2. Specify object key name explicitly.
                await fileTransferUtility.UploadAsync(_filePath, _bucketName, _keyName);
                Console.WriteLine("Upload 2 completed");

                // Option 3. Upload data from a type of System.IO.Stream.
                using (var fileToUpload =
                    new FileStream(_filePath, FileMode.Open, FileAccess.Read))
                {
                    await fileTransferUtility.UploadAsync(fileToUpload,
                                               _bucketName, _keyName);
                }
                Console.WriteLine("Upload 3 completed");

                // Option 4. Specify advanced settings.
                var fileTransferUtilityRequest = new TransferUtilityUploadRequest
                {
                    BucketName = _bucketName,
                    FilePath = _filePath,
                    StorageClass = S3StorageClass.StandardInfrequentAccess,
                    PartSize = 6291456, // 6 MB.
                    Key = _keyName,
                    CannedACL = S3CannedACL.PublicRead
                };
                fileTransferUtilityRequest.Metadata.Add("param1", "Value1");
                fileTransferUtilityRequest.Metadata.Add("param2", "Value2");

                await fileTransferUtility.UploadAsync(fileTransferUtilityRequest);
                Console.WriteLine("Upload 4 completed");
            }
            catch (AmazonS3Exception e)
            {
                Console.WriteLine("Error encountered on server. Message:'{0}' when writing an object", e.Message);
            }
            catch (Exception e)
            {
                Console.WriteLine("Unknown encountered on server. Message:'{0}' when writing an object", e.Message);
            }

        }
    }
}
