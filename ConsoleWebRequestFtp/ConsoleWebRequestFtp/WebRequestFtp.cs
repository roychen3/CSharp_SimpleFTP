using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.IO;

namespace ConsoleWebRequestFtp
{
    public class WebRequestFtp
    {
        private string _ip;
        private string _userName;
        private string _password;

        public WebRequestFtp(string ip, string port, string userName, string password)
        {
            _ip = ip + (string.IsNullOrEmpty(port) ? "" : ":" + port);
            _userName = userName;
            _password = password;
        }

        /// <summary>
        /// FTP 連線設定
        /// </summary>
        /// <param name="method">取得與命令要傳送至 FTP 伺服器的命令</param>
        /// <param name="ftpPath">要執行 FTP 命令的路徑</param>
        /// <returns></returns>
        private FtpWebRequest openRequest(string method, string ftpPath = "")
        {
            FtpWebRequest request = (FtpWebRequest)WebRequest.Create(_ip + ftpPath);
            request.Method = method;
            request.Credentials = new NetworkCredential(_userName, _password);
            request.UseBinary = true;
            request.Proxy = null;
            request.KeepAlive = true;
            request.UsePassive = false;
            return request;
        }

        /// <summary>
        /// FTP上傳檔案
        /// </summary>
        /// <param name="localFile">本機檔案 (要上傳的檔案)</param>
        /// <param name="uploadPath">要上傳到 FTP 的路徑</param>
        /// <param name="uploadFileName">指定新檔名 (若無，預設為原本的檔名)</param>
        public void Upload(string localFile, string uploadPath, string uploadFileName = "")
        {
            try
            {
                StreamReader sourceStream = new StreamReader(localFile);
                byte[] fileContents = Encoding.UTF8.GetBytes(sourceStream.ReadToEnd());

                uploadPath = setFullFileName(uploadPath, uploadFileName, localFile);
                FtpWebRequest request = openRequest(WebRequestMethods.Ftp.UploadFile, uploadPath);
                request.ContentLength = fileContents.Length;

                Stream requestStream = request.GetRequestStream();
                requestStream.Write(fileContents, 0, fileContents.Length);

                sourceStream.Close();
                requestStream.Close();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// FTP 搬移檔案
        /// </summary>
        /// <param name="moveFile">要搬移的檔案</param>
        /// <param name="newPath">要搬移到的新路徑</param>
        /// <param name="newFileName">指定新檔名 (若無，預設為原本的檔名)</param>
        public void Move(string moveFile, string newPath, string newFileName = "")
        {
            try {
                newPath = setFullFileName(newPath, newFileName, moveFile);
                FtpWebRequest request = openRequest(WebRequestMethods.Ftp.Rename, moveFile);
                request.RenameTo = newPath;

                FtpWebResponse response = (FtpWebResponse)request.GetResponse();
                response.Close();
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// FTP 下載檔案
        /// </summary>
        /// <param name="localPath">本機端路徑 (要下載到本機的路徑)</param>
        /// <param name="downloadFile">要下載 FTP 的檔案</param>
        /// <param name="localFileName">指定新檔名 (若無，預設為原本的檔名)</param>
        public void Download(string localPath, string downloadFile, string localFileName = "")
        {
            try
            {
                FtpWebRequest request = openRequest(WebRequestMethods.Ftp.DownloadFile, downloadFile);
                FtpWebResponse response = (FtpWebResponse)request.GetResponse();
                Stream responseStream = response.GetResponseStream();
                StreamReader reader = new StreamReader(responseStream);
                byte[] fileContents = Encoding.UTF8.GetBytes(reader.ReadToEnd());

                localPath = setFullFileName(localPath, localFileName, downloadFile);
                FileStream fs = new FileStream(localPath, FileMode.Create);
                fs.Write(fileContents, 0, fileContents.Length);

                response.Close();
                responseStream.Close();
                reader.Close();
                fs.Close();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// FTP 刪除檔案
        /// </summary>
        /// <param name="deleteFile">要刪除的檔案</param>
        public void Delete(string deleteFile)
        {
            try
            {
                FtpWebRequest request = openRequest(WebRequestMethods.Ftp.DeleteFile, deleteFile);
                FtpWebResponse response = (FtpWebResponse)request.GetResponse();
                response.Close();
            }
            catch(Exception ex){
                throw ex;
            }
        }

        /// <summary>
        /// 取得 FTP 路徑上的檔案與文件夾
        /// </summary>
        /// <param name="ftpPath">指定FTP下的路徑</param>
        /// <param name="filenameExtension">副檔名</param>
        public List<string> GetFileList(string ftpPath, string filenameExtension = "")
        {
            try
            {
                FtpWebRequest request = openRequest(WebRequestMethods.Ftp.ListDirectory, ftpPath + filenameExtension);
                FtpWebResponse response = (FtpWebResponse)request.GetResponse();
                Stream responseStream = response.GetResponseStream();
                StreamReader reader = new StreamReader(responseStream);
                string line;
                List<string> fileList = new List<string>();
                while ((line = reader.ReadLine()) != null)
                {
                    fileList.Add(line);
                }

                response.Close();
                responseStream.Close();
                reader.Close();
                return fileList;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 設定路徑與檔名
        /// </summary>
        /// <param name="path">路徑</param>
        /// <param name="fileName">檔名</param>
        /// <param name="sourceFullFileName">來源路徑與檔名</param>
        private string setFullFileName(string path, string fileName, string sourceFullFileName)
        {
            fileName = string.IsNullOrEmpty(fileName) ? Path.GetFileName(sourceFullFileName) : fileName;
            path += fileName;
            return path;
        }
    }
}
