using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Net;
using TMPro;
using UnityEngine;
using Debug = System.Diagnostics.Debug;

public class UpgradeManager : MonoBehaviour
{
    [SerializeField] TMP_Text _screenDebug;
    
    readonly string URIV1 = "https://raw.githubusercontent.com/tactivesport/update/main/VersionV1.zip";
    readonly string URIV2 = "https://raw.githubusercontent.com/tactivesport/update/main/VersionV2.zip";
    readonly string CURFOLDER = Environment.CurrentDirectory + @"/UpdateFolder/";
    readonly string TEMPPATH = Path.Combine(Path.GetTempPath(), "Update.zip");

    public void Upgrade()
    {
        Log("Downloading version 2");
        Download(URIV2, "V2/V2.exe");
    }

    public void Downgrade()
    {
        Log("Downloading version 1");
        Download(URIV1, "V1/V1.exe");
    }

    void Download(string uri, string executable)
    {
        _cacheExecutable = executable;
        if (Directory.Exists(CURFOLDER))
        {
            Directory.Delete(CURFOLDER, true);
            Directory.CreateDirectory(CURFOLDER);
        }

        var client = new WebClient();
        client.DownloadProgressChanged += (sender, args) => Log($"Download progress: {args.ProgressPercentage}%");
        client.DownloadFileCompleted += ProceedUnzip;
        client.DownloadFileAsync(new Uri(uri), TEMPPATH);
    }

    void ProceedUnzip(object sender, AsyncCompletedEventArgs e)
    {
        if (e.Cancelled)
        {
            Log("Download cancelled error");
            return;
        }

        Log("Download completed, attempt unzipping after wiping target folder");
        
        ZipFile.ExtractToDirectory(TEMPPATH, CURFOLDER);

        Log("Starting");
        Process.Start(CURFOLDER + _cacheExecutable);

        Log("Quitting");
        Application.Quit();
        
        // Powershell? https://stackoverflow.com/questions/58318026/how-to-uninstall-a-uwp-application-programmatically-without-admin-access-c-sharp
    }

    void Log(string text)
    {
        UnityEngine.Debug.Log(text);
        _screenDebug.text = text;
    }

    string _cacheExecutable;
}
