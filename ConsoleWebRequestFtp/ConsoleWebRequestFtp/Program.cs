using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleWebRequestFtp
{
    class Program
    {
        static void Main(string[] args)
        {
            string ftpIp = "127.0.0.1";
            string ftpProt = "21";
            string ftpAccount = "account";
            string ftpPassword = "password";

            string localFile = @"D:\your file*.txt";
            string uploadFtpPath = "/FTP/Folder/";
            string uploadFtpNewName = "upload.txt";

            string ftpMoveFile = "/FTP/Folder/upload.txt";
            string ftpMoveNewPath = "/FTP/Folder/New Folder/";
            string ftpMoveNewFileName = "move.txt";

            string localPath = @"D:\";
            string ftpDownloadFile = "/FTP/Folder/New Folder/move.txt";
            string ftpDownloadNewFileNme = "download.txt";

            string deleteFile = "/FTP/Folder/New Folder/move.txt";

            try
            {
                WebRequestFtp ftp = new WebRequestFtp(ftpIp, ftpProt, ftpAccount, ftpPassword);

                ftp.Upload(localFile, uploadFtpPath, uploadFtpNewName);
                List<string> fileList = ftp.GetFileList(uploadFtpPath);

                ftp.Move(ftpMoveFile, ftpMoveNewPath, ftpMoveNewFileName);
                fileList = ftp.GetFileList(ftpMoveNewPath);
                fileList = ftp.GetFileList(uploadFtpPath);

                ftp.Download(localPath, ftpDownloadFile, ftpDownloadNewFileNme);

                ftp.Delete(deleteFile);
                fileList = ftp.GetFileList(ftpMoveNewPath);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.ReadKey();
            }
        }
    }
}