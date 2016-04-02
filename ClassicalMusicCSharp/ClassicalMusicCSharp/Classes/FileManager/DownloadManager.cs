using ClassicalMusicCSharp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Windows.Networking.BackgroundTransfer;
using System.Diagnostics;
using Windows.Storage;
using System.Collections.ObjectModel;

namespace ClassicalMusicCSharp.Classes.FileManager
{
    class DownloadManager
    {
        private static DownloadManager _instance;
        public static DownloadManager Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new DownloadManager();
                return _instance;
            }
        }
        private BackgroundDownloader downloader;
        private CancellationTokenSource cts;
        public ObservableCollection<DownloadItem> ListaDownload { get; set; }
        private DownloadManager()
        {
            downloader = new BackgroundDownloader();
            cts = new CancellationTokenSource();
            ListaDownload = new ObservableCollection<DownloadItem>();
            Init();
        }
        public async void Init()
        {
            IReadOnlyList<DownloadOperation> downloads = null;
            try
            {
                downloads = await BackgroundDownloader.GetCurrentDownloadsAsync();
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Discovery error", ex);
                return;
            }
            if (downloads.Count > 0)
            {
                Debug.WriteLine(downloads.Count + " in attesa");
                List<Task> tasks = new List<Task>();
                foreach (DownloadOperation download in downloads)
                {
                    
                    try
                    {
                        DownloadOperation downAttached = await download.AttachAsync();
                        DownloadItem downItem = SettingsManager.Instance.GetDownloadItem(downAttached.Guid.ToString());
                        if(downItem!=null)
                            ListaDownload.Add(downItem);
                        //tasks.Add(HandleDownloadAsync(downAttached, downItem));
                    }
                    catch(Exception e)
                    {
                        Debug.WriteLine(e.Message);
                    }
                    // Attach progress and completion handlers.
                }

                // Don't await HandleDownloadAsync() in the foreach loop since we would attach to the second
                // download only when the first one completed; attach to the third download when the second one
                // completes etc. We want to attach to all downloads immediately.
                // If there are actions that need to be taken once downloads complete, await tasks here, outside
                // the loop.
                await Task.WhenAll(tasks);
            }
            else
            {
                SettingsManager.Instance.CleanDownloadItems();
            }
        }
        private void DownloadProgress(DownloadOperation download)
        {
            DownloadItem downItem = GetDownload(download.Guid.ToString());
            int progress = (int)(100 * ((double)download.Progress.BytesReceived / (double)download.Progress.TotalBytesToReceive));
            string textProgress = string.Empty;
            switch (download.Progress.Status)
            {
                case BackgroundTransferStatus.Running:
                    {
                        textProgress = "Downloading";
                        break;
                    }
                case BackgroundTransferStatus.PausedByApplication:
                    {
                        textProgress = "Paused";
                        break;
                    }
                case BackgroundTransferStatus.PausedCostedNetwork:
                    {
                        textProgress = "Use a wifi connection";
                        break;
                    }
                case BackgroundTransferStatus.PausedNoNetwork:
                    {
                        textProgress = "Paused - Internet is not available";
                        break;
                    }
                case BackgroundTransferStatus.Error:
                    {
                        textProgress = "Error";
                        break;
                    }
            }
            if (progress >= 100)
            {
                textProgress = "Complete!";
                SettingsManager.Instance.RemoveDownloadId(download.Guid.ToString());
            }
            downItem.ProgressText = $"{textProgress} - {progress}%";
        }
        private async Task HandleDownloadAsync(DownloadOperation download, DownloadItem downItem, bool attach = false)
        {
            try
            {
                ListaDownload.Add(downItem);
                Progress<DownloadOperation> progressCallback = new Progress<DownloadOperation>(DownloadProgress);
                await download.StartAsync().AsTask(cts.Token, progressCallback);
                ResponseInformation response = download.GetResponseInformation();
            }
            catch (TaskCanceledException)
            {
                //LogStatus("Canceled: " + download.Guid, NotifyType.StatusMessage);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                /*
                if (!IsExceptionHandled("Execution error", ex, download))
                {
                    throw;
                }
                */
            }
            finally
            {
                ListaDownload.Remove(downItem);
            }
        }
        public async Task<bool> DownloadTrack(DownloadItem downloadItem)
        {
            Debug.WriteLine("Creo download di " + downloadItem.Url);
            StorageFolder music = KnownFolders.MusicLibrary;
            StorageFolder classicalDir = await music.CreateFolderAsync("ClassicalMusic", CreationCollisionOption.OpenIfExists);

            StorageFolder downloadFolder = await classicalDir.CreateFolderAsync(downloadItem.ComposerName, CreationCollisionOption.OpenIfExists);
            if (!string.IsNullOrEmpty(downloadItem.OperaName))
                downloadFolder = await downloadFolder.CreateFolderAsync(downloadItem.OperaName, CreationCollisionOption.OpenIfExists);

            string filename = downloadItem.Url.Substring(downloadItem.Url.LastIndexOf("/") + 1);

            try
            {
                StorageFile destFile = await downloadFolder.CreateFileAsync(filename, CreationCollisionOption.FailIfExists);
                DownloadOperation download = downloader.CreateDownload(new Uri(downloadItem.Url), destFile);
                downloadItem.IdDownload = download.Guid.ToString();
                Debug.WriteLine("Download avviato di " + downloadItem.Url);
                download.Priority = BackgroundTransferPriority.Default;
                downloadItem.ProgressText = "Download started";
                SettingsManager.Instance.AddDownloadId(download.Guid.ToString(), downloadItem.ComposerName, downloadItem.OperaName, downloadItem.TrackName, downloadItem.Url);
                await HandleDownloadAsync(download, downloadItem);
                return true;
            }
            catch(Exception e)
            {
                downloadItem.ProgressText = e.Message;
                return false;
            }
        }
        public DownloadItem GetDownload(string guid)
        {
            foreach (DownloadItem d in ListaDownload)
            {
                if (d.IdDownload.Equals(guid))
                    return d;
            }
            return SettingsManager.Instance.GetDownloadItem(guid);
        }
    }
}
