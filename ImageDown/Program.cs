using System;
using System.IO;
using System.Net;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace ImageDown
{
    class Program
    {
        #region Main
        static void Main(string[] args)
        {
            /*
            BatchFile
            (
            @"C:\Users\Administrator\Desktop\fulisoso\link\下载失败的链接.txt",
            @"C:\Users\Administrator\Desktop\fulisoso\link\下载失败的链接\",
            @"E:\下载失败的链接.txt",
            false
            );
            */

            BatchFile
            (
            @"E:\a.txt",
            @"E:\d\",
            @"E:\下载失败的链接.txt",
            false
            );
        } 
        #endregion

        #region BatchFile
        //要下载的文本路径 保存的目录
        public static void BatchFile(string ReadPath, string SavePath, string NolinkPath, bool isrename)
        {
            StringBuilder sbrNo = new StringBuilder();
            FileStream fs = new FileStream(ReadPath, FileMode.Open, FileAccess.Read);
            try
            {
                using (StreamReader sr = new StreamReader(fs, System.Text.Encoding.Default))
                {
                    //要下载目标URL
                    string line;

                    //保存的文件名
                    string saveFilePath = string.Empty;

                    int i = 1;

                    while ((line = sr.ReadLine()) != null)
                    {
                        saveFilePath = string.Concat(SavePath, GetFileName(line, isrename, i));

                        if (DownFile(line, saveFilePath) == true)
                        {
                            Console.WriteLine(string.Format("正在处理第{0}个文件，下载成功", i));
                        }
                        else
                        {
                            sbrNo.Append(line + Environment.NewLine);
                            Console.WriteLine(string.Format("下载失败！{0}", i));
                        }
                        i++;
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            fs.Close();
            bool bl = CreateFile(sbrNo.ToString(), NolinkPath);
            Console.WriteLine("创建文件" + bl.ToString());
        }
        #endregion

        #region GetFileName
        /// <summary>
        /// 生成要下载的文件名
        /// </summary>
        /// <param name="strImgUrl"></param>
        /// <param name="isRename"></param>
        /// <returns></returns>
        public static string GetFileName(string strImgUrl, bool isRename, int i, string defaultExt = ".torrent")
        {
            //http://www.bt537.com/down/100901188/a.txt
            //http://www.bt537.com/down/100901188/
            Regex re = new Regex(@"/down/(\d+)/");
            Match m = re.Match(strImgUrl);
            if (m.Success)
            {
                return string.Concat(m.Value.Replace("/down/", "").Trim('/'), defaultExt);
            }
            return "no.torrent";

            /*
            string fileName = "noName.jpg";
            try
            {
                //url可能不含后扩展名
                if (!string.IsNullOrEmpty(strImgUrl))
                {
                    int lastIndex = strImgUrl.LastIndexOf("/");

                    if (lastIndex != -1)
                    {
                        //被下载的文件名
                        fileName = strImgUrl.Substring(lastIndex + 1);
                        string ext = Path.GetExtension(fileName);
                        fileName = Path.GetFileNameWithoutExtension(fileName);

                        if (isRename)
                        {
                            fileName = i.ToString();
                        }
                        fileName = string.Concat(fileName, ext);
                    }
                }
            }
            catch
            {
                return fileName;
            }
            return fileName;
            */
        }
        #endregion

        #region DownFile
        public static bool DownFile(string stringUrl, string savefilePath)
        {
            #region note
            //string stringUrl = "http://www.meileg.com/beautyleg/photo/big/256-Ruby-81/0001.jpg";
            //获取请求文件的类型
            //Console.WriteLine(httpResponse.ContentType);
            //获得请求数据的大小，字节数114072byte
            //Console.WriteLine(httpResponse.ContentLength);
            //从请求响应中获得响应流 
            //获得当前应用程序的所在的更目录
            //Console.WriteLine(Environment.CurrentDirectory);
            //Console.WriteLine(System.Windows.Forms.Application.StartupPath); 
            #endregion

            bool bl = true;
            try
            {
                #region 处理逻辑
                if (!string.IsNullOrEmpty(stringUrl))
                {
                    //根据URL创建请求的实例对象
                    WebRequest httpRequest = WebRequest.Create(stringUrl);
                    //由请求对象获得响应对象
                    WebResponse httpResponse = httpRequest.GetResponse();
                    //获得响应流
                    using (Stream netStream = httpResponse.GetResponseStream())
                    {
                        using (FileStream fileStream = new FileStream(savefilePath, FileMode.Create, FileAccess.Write))
                        {
                            //初始化缓冲区的大小,每次读取时的单位长度
                            //byte[] downloadBuffer = new byte[4096];
                            byte[] downloadBuffer = new byte[60];
                            int bufferSize;//接收每次返回的字节大小
                            int curDownSize = 0;//当前下载数据的大小
                            while ((bufferSize = netStream.Read(downloadBuffer, 0, downloadBuffer.Length)) > 0)
                            {
                                curDownSize += bufferSize;
                                //double speed = (httpResponse.ContentLength - curDownSize);
                                //Console.WriteLine(speed);
                                fileStream.Write(downloadBuffer, 0, bufferSize);
                            }
                        }
                    }
                }
                else
                {
                    bl = false;
                }
                #endregion
            }
            catch (WebException e)
            {
                #region 异常处理
                bl = false;
                e = new WebException("404");
                //Console.WriteLine(e.Message);
                //写入日志
                //CommonSpace.Conmmon.CreateFile(ImgHelper.strLog);
                //WriteLog(ImgHelper.strLog);
                //Console.WriteLine(webe.Status.ToString());
                #endregion
            }
            return bl;
        }
        #endregion

        #region CreateFile
        /// <summary>
        /// 创建指定格式的文件
        /// </summary>
        /// <param name="strContent">写入的文件内容</param>
        /// <param name="strGobalPaht">保存的路径</param>
        /// <returns></returns>
        public static bool CreateFile(string strContent, string strGobalPath)
        {
            try
            {
                using (FileStream fs = new FileStream(strGobalPath, FileMode.Create, FileAccess.Write))
                {
                    using (StreamWriter sr = new StreamWriter(fs, Encoding.Default))
                    {
                        sr.WriteLine(strContent);
                    }
                }
                return true;
            }
            catch
            {
                //throw new Exception(ex.Message);
                return false;
            }
        }
        #endregion
    }
}