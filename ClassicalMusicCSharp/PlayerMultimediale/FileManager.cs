using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

namespace PlayerMultimediale
{
    class FileManager
    {
        public static async Task<string> GetPath(string composer, string opera, string trackUrl)
        {
            if (await FileExists(composer, opera, trackUrl))
            {
                string path = await GetFilePath(composer, opera, trackUrl);
                if (!string.IsNullOrEmpty(path))
                    return path;
            }
            return trackUrl;
        }
        private static string getFileName(string url)
        {
            return url.Substring(url.LastIndexOf("/") + 1);
        }
        public static async Task<bool> FileExists(string composer, string opera, string trackUrl)
        {
            string filename = getFileName(trackUrl);
            try
            {
                StorageFolder music = KnownFolders.MusicLibrary;
                StorageFolder classicalDir = await music.GetFolderAsync("ClassicalMusic");

                if (classicalDir == null)
                    return false;

                StorageFolder composerFolder = await classicalDir.GetFolderAsync(composer);
                if (composer == null)
                    return false;

                StorageFolder operaFolder = await composerFolder.GetFolderAsync(opera);
                if (operaFolder == null)
                    return false;

                StorageFile destFile = await operaFolder.GetFileAsync(filename);
                if (destFile == null)
                    return false;
                else
                {
                    if ((await destFile.GetBasicPropertiesAsync()).Size == 0)
                        return false;
                    return true;
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
                return false;
            }
        }
        private static async Task<string> GetFilePath(string composer, string opera, string trackUrl)
        {
            string filename = getFileName(trackUrl);
            try
            {
                StorageFolder music = KnownFolders.MusicLibrary;
                StorageFolder classicalDir = await music.GetFolderAsync("ClassicalMusic");

                if (classicalDir == null)
                    return null;

                StorageFolder composerFolder = await classicalDir.GetFolderAsync(composer);
                if (composer == null)
                    return null;

                StorageFolder operaFolder = await composerFolder.GetFolderAsync(opera);
                if (operaFolder == null)
                    return null;

                StorageFile destFile = await operaFolder.GetFileAsync(filename);
                if (destFile == null)
                    return null;
                else
                {
                    if ((await destFile.GetBasicPropertiesAsync()).Size == 0)
                        return null;
                    return destFile.Path;
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
                return null;
            }
        }
    }
}
