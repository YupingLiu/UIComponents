
using System.IO;
using UnityEngine;
using System;

namespace MoreFun
{
    public class MoreFileUtil 
    {
        public static string GenerateFilePath(string relativePath, string fileNamePrefix, string fileNamePostfix)
        {
            DateTime now = DateTime.Now;
            
            string path = Application.persistentDataPath + "/" + relativePath + "/" +
                fileNamePrefix +
                now.Year.ToString() + now.Month.ToString() + now.Day.ToString() +
                    now.Hour.ToString() + now.Minute.ToString() + now.Second.ToString() +
                    fileNamePostfix;

            return path;
        }

        /// <summary>
        /// Tries to the create the folder for a file.
        /// </summary>
        /// <returns><c>true</c>, if create file folder was tryed, <c>false</c> otherwise.</returns>
        /// <param name="filePath">File path.</param>
        public static void TryCreateFileFolder(string filePath)
        {
            string dirPath = Path.GetDirectoryName(filePath);
            if(false == Directory.Exists(dirPath))
            {
                Directory.CreateDirectory(dirPath);
            }
        }
        
        public static bool SaveFile(string filePath, byte[] content)
        {
            FileStream resultFs = null;
            try
            {
                
                if (File.Exists(filePath))
                {
                    resultFs = File.OpenWrite(filePath);
                } else
                {
                    TryCreateFileFolder(filePath);
                    
                    resultFs = File.Create(filePath);
                }
                
            } catch (System.Exception ex)
            {
                MoreDebug.LogError(typeof(MoreFileUtil), ex);
                return false;
            }
            
            if (null != resultFs && null != content)
            {
                resultFs.Write(content, 0, content.Length);
            }
            
            resultFs.Close();
            return true;
        }

    }
    

}