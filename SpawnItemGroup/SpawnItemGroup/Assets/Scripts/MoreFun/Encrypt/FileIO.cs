using UnityEngine;
using System;
using System.Collections;
using System.Linq;
using System.Text;
using System.IO;
using MoreFun;

namespace MoreFun
{
    public class FileIO
    {
        public readonly string FILE_DATA_PATH = Application.persistentDataPath;

        // Use this for initialization
        public void SaveUID(string uid)
        {
            WriteToFile(uid);
        }

        // Update is called once per frame
        public string GetUID()
        {
            return ReadFromFile();
        }

        void WriteToFile(string str)
        {
            FileStream fs = new FileStream(FILE_DATA_PATH  + "/UID.txt", FileMode.OpenOrCreate, FileAccess.Write);
            try
            {
                using (StreamWriter m_streamWriter = new StreamWriter(fs))
                {
                    m_streamWriter.Flush();
                    m_streamWriter.WriteLine(str);
                    m_streamWriter.Flush();
                    m_streamWriter.Dispose();
                    m_streamWriter.Close();
                }
            }
            catch (Exception ee)
            {
                Console.WriteLine(ee.Message);
            }
            finally
            {
                fs.Dispose();
                fs.Close();
            }
        }

        string ReadFromFile()
        {
            try
            {
                FileStream fs = new FileStream(FILE_DATA_PATH + "/UID.txt", FileMode.OpenOrCreate, FileAccess.Read);
                StreamReader m_streamReader = new StreamReader(fs);

                //用StreamReader类来读取文件
                m_streamReader.BaseStream.Seek(0, SeekOrigin.Begin);

                //从数据流中读取第一行
                string strLine = m_streamReader.ReadLine();
                
                m_streamReader.Close();
                return strLine;
            }
            catch (Exception ee)
            {
#if DEV_BUILD
                Debug.LogWarning("有异常：" + ee.Message);
#endif
                return null;
            }

        }    
    } 
}
