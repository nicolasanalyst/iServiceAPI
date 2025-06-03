using Microsoft.AspNetCore.Http;
using SixLabors.ImageSharp;
using System.Net;

namespace iServiceServices.Services
{
    public class FtpServices
    {
        private string ftpServer = "149.100.142.179";
        private string ftpUsername = "root_ftp";
        private string ftpPassword = "27_53]KK{Gy(";

        public async Task<string> UploadFileAsync(IFormFile file, string remoteDir, string remoteFileName)
        {
            using (var imageStream = new MemoryStream())
            {
                await file.CopyToAsync(imageStream);
                imageStream.Position = 0;

                using (var image = await Image.LoadAsync(imageStream))
                {
                    var pngStream = new MemoryStream();
                    await image.SaveAsPngAsync(pngStream);
                    pngStream.Position = 0;

                    string url = $"ftp://{ftpServer}/var/www/html/images/{remoteDir}/{remoteFileName}";
                    FtpWebRequest request = (FtpWebRequest)WebRequest.Create(url);
                    request.Method = WebRequestMethods.Ftp.UploadFile;
                    request.Credentials = new NetworkCredential(ftpUsername, ftpPassword);
                    request.UsePassive = true;
                    request.UseBinary = true;
                    request.KeepAlive = false;

                    using (Stream reqStream = await request.GetRequestStreamAsync())
                    {
                        await pngStream.CopyToAsync(reqStream);
                    }

                    using (FtpWebResponse response = (FtpWebResponse)await request.GetResponseAsync())
                    {
                        return $"http://srv452480.hstgr.cloud/images/{remoteDir}/{remoteFileName}";
                    }
                }
            }
        }
    }
}
