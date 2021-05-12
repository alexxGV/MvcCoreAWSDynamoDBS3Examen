using Amazon.S3;
using Amazon.S3.Model;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace MvcCoreAWSDynamoDBS3.Services
{
    public class ServiceAWSS3
    {
        private String bucketName;
        private IAmazonS3 awsClient;

        public ServiceAWSS3(IAmazonS3 awsclient, IConfiguration configuration)
        {
            this.awsClient = awsclient;
            this.bucketName = configuration["AWSS3:BucketName"];
        }
        public async Task<bool> UploadFileAsync(Stream stream, String fileName)
        {
            PutObjectRequest request = new PutObjectRequest()
            {
                InputStream = stream,
                Key = fileName,
                BucketName = this.bucketName
            };

            PutObjectResponse response = await this.awsClient.PutObjectAsync(request);

            if (response.HttpStatusCode == HttpStatusCode.OK)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public async Task<bool> DeleteFileAsync(string filename)
        {
            DeleteObjectResponse response = await this.awsClient.DeleteObjectAsync(this.bucketName, filename);
            if (response.HttpStatusCode == HttpStatusCode.OK)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

    }
}
