using System;
using System.ComponentModel;
using System.IO;
using System.Net;
using UnityEngine;

namespace Core.Net.Http
{
    /// <summary>
    /// 资源下载器
    /// </summary>
    public class Downloader
    {
        /// <summary>
        /// 下载连接数限制
        /// PS:修改该值可以直接简单的限制HTTP下载请求的并发数
        /// </summary>
        public static int DownloadConnectionLimit = 500;

        /// <summary>
        /// 重写的WebClient类
        /// </summary>
        class DownloadWebClient : WebClient
        {
            readonly int timeout;
            public DownloadWebClient(int timeout = 60)
            {
                this.timeout = timeout * 1000;
            }

            protected override WebRequest GetWebRequest(Uri address)
            {                
                HttpWebRequest request = base.GetWebRequest(address) as HttpWebRequest;
                request.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;
                request.Timeout = timeout;
                request.ReadWriteTimeout = timeout;
                request.Proxy = null;
                return request;
            }

            protected override WebResponse GetWebResponse(WebRequest request)
            {
                return base.GetWebResponse(request);
            }
        }

        DownloadWebClient client;

        bool isDone;

        /// <summary>
        /// 是否操作完成
        /// </summary>
        public bool IsDone
        {
            get
            {
                if (false == isDone)
                {
                    CheckTimeout();
                }
                return isDone;
            }
        }

        float progress;

        /// <summary>
        /// 操作进度
        /// </summary>
        public float Progress
        {
            get
            {
                return progress;
            }
        }

        string error;

        /// <summary>
        /// 错误信息
        /// </summary>
        public string Error
        {
            get
            {
                return error;
            }
        }

        long totalSize;

        /// <summary>
        /// 文件总大小
        /// </summary>
        public long TotalSize
        {
            get
            {
                return totalSize;
            }
        }

        long loadedSize;

        /// <summary>
        /// 已完成大小
        /// </summary>
        public long LoadedSize
        {
            get
            {
                return loadedSize;
            }
        }

        /// <summary>
        /// 是否已销毁
        /// </summary>
        public bool IsDisposeed
        {
            get { return client == null ? true : false; }
        }


        string savePath;

        string url;

        /// <summary>
        /// 下载的URL地址
        /// </summary>
        public string Url
        {
            get { return url; }
        }

        /// <summary>
        /// 文件的保存路径
        /// </summary>
        public string SavePath
        {
            get { return savePath; }
        }

        /// <summary>
        /// 下载超时的设置，当指定毫秒内下载进度没有改变时，视为下载超时。
        /// </summary>
        public int Timeout = 15000;

        /// <summary>
        /// 最后进度改变的时间
        /// </summary>
        DateTime _lastProgressChangedDT;

        /// <summary>
        /// 初始化下载类
        /// </summary>
        /// <param name="url">下载文件的URL地址</param>
        /// <param name="savePath">保存文件的本地地址</param>
        /// <param name="version">URL对应文件的版本号</param>
        public Downloader(string url, string savePath, string version = null)
        {           
            this.url = url;
            this.savePath = savePath;
            string saveDir = Path.GetDirectoryName(savePath);
            if (Directory.Exists(saveDir) == false)
            {
                Directory.CreateDirectory(saveDir);
            }
            client = new DownloadWebClient();            
            client.DownloadProgressChanged += OnDownloadProgressChanged;
            client.DownloadFileCompleted += OnDownloadFileCompleted;

            if (null != version)
            {
                string flag;
                if (url.Contains("?"))
                {
                    flag = "&";
                }
                else
                {
                    flag = "?";
                }

                url += $"{flag}unity_download_ver={version}";
            }

            try
            {
                Uri uri = new Uri(url);
                var serverPoint = ServicePointManager.FindServicePoint(uri);
                serverPoint.ConnectionLimit = DownloadConnectionLimit;
                progress = 0;
                _lastProgressChangedDT = DateTime.Now;
                client.DownloadFileAsync(uri, savePath);                                
            }
            catch (Exception ex)
            {
                isDone = true;
                error = ex.Message;
            }
        }

        /// <summary>
        /// 销毁对象，会停止所有的下载
        /// </summary>
        public void Dispose()
        {
            if (client != null)
            {
                client.DownloadProgressChanged -= OnDownloadProgressChanged;
                client.DownloadFileCompleted -= OnDownloadFileCompleted;
                client.CancelAsync();
                client.Dispose();
                client = null;
                if(false ==isDone)
                {                    
                    SetError("Canceled");
                    isDone = true;
                }                
            }
        }

        /// <summary>
        /// 下载文件完成
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnDownloadFileCompleted(object sender, AsyncCompletedEventArgs e)
        {            
            if (e.Error != null)
            {
                SetError(e.Error.Message);                
            }
            else if (loadedSize < totalSize)
            {
                SetError("Disconnected");                
            }
            isDone = true;                      
        }

        /// <summary>
        /// 下载进度改变
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnDownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            if(e.BytesReceived > loadedSize)
            {
                _lastProgressChangedDT = DateTime.Now;
                loadedSize = e.BytesReceived;
                totalSize = e.TotalBytesToReceive;
                if (0 == totalSize)
                {
                    progress = 0;
                }
                else
                {
                    progress = loadedSize / (float)totalSize;
                }
            }      
        }

        /// <summary>
        /// 检查是否超时
        /// </summary>
        void CheckTimeout()
        {
            TimeSpan ts = DateTime.Now - _lastProgressChangedDT;
            //Debug.LogFormat("检查时间差：{0} {1}", ts.TotalMilliseconds, url);
            if(ts.TotalMilliseconds > Timeout)
            {
                //超时
                Dispose();
                SetError("TimeOut");                
            }
        }

        void SetError(string error)
        {
            Debug.LogError($"下载失败 [{url}]:{error}");

            //删除文件
            if(File.Exists(SavePath))
            {
                File.Delete(SavePath);
            }
            this.error = error;
        }
    }
}
