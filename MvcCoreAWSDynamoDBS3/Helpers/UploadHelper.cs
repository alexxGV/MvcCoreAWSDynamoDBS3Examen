using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace MvcCoreAWSDynamoDBS3.Helpers
{
    public class UploadHelper
    {
        PathProvider pathprovider;

        public UploadHelper(PathProvider pathprovider)
        {
            this.pathprovider = pathprovider;
        }

        public async Task<String> UploadFileAsync(IFormFile formFile
            , Folders folder, String fileName)
        {
            String path =
                this.pathprovider.MapPath(fileName, Folders.Images);
            using (var stream = new FileStream(path, FileMode.Create))
            {
                await formFile.CopyToAsync(stream);
            };
            return path;
        }
    }
}
