using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Net;
using UnityEngine;
using Debug = UnityEngine.Debug;

public class UpgradeManager : MonoBehaviour
{
    const string PATH = "https://raw.githubusercontent.com/tactivesport/update/main/";
    readonly string CURFOLD = Environment.CurrentDirectory + @"/UpdateFolder/";
    
    public void Upgrade()
    {
        if (!Directory.Exists(CURFOLD))
        {
            Debug.Log("Creating UpdateFolder");
            Directory.CreateDirectory(CURFOLD);
        }

        Debug.Log("Downloading V2");
        using (var client = new WebClient())
        {
            client.DownloadFile(PATH + "VersionV2.zip", CURFOLD + "VersionV2.zip");
            client.DownloadFileCompleted += ProceedUnzip;
        }
    }

    void ProceedUnzip(object sender, AsyncCompletedEventArgs e)
    {
        if (e.Cancelled)
        {
            Debug.Log("Download error!");
            return;
        }

        Debug.Log("Unzipping file.");
        ZipFile.ExtractToDirectory(CURFOLD + "VersionV2.zip", CURFOLD);
        
        Debug.Log("Starting V2.");
        Process.Start(CURFOLD + "VersionV2/V2.exe");
        
        Debug.Log("Quitting V1.");
        Application.Quit();
    }
}
