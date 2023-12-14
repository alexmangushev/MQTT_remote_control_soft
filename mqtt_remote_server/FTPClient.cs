using Azure.Core;
using mqtt_remote_server.Controllers;
using System.Net;
using System.Text;

namespace mqtt_remote_server
{
    public class FTPClient
    {
        private readonly FtpWebRequest request;
        private readonly string UserName;
        private readonly string Password;
        public FTPClient ()
        {
            UserName = "Program";
            Password = "10102001";
            request = (FtpWebRequest)WebRequest.Create("ftp://127.0.0.1/Firmware.txt");
            request.Method = WebRequestMethods.Ftp.UploadFile;
            request.Credentials = new NetworkCredential(UserName, Password);
        }

        public bool SendFile(string file)
        {
            byte[] fileContents = Convert.FromBase64String(file);
            request.ContentLength = fileContents.Length;

            Stream requestStream = request.GetRequestStream();
            requestStream.Write(fileContents, 0, fileContents.Length);
            requestStream.Close();

            FtpWebResponse response = (FtpWebResponse)request.GetResponse();
            
            bool success = response.StatusCode.Equals(FtpStatusCode.ClosingData);
            response.Close();
            return success;
        }

    }
}
