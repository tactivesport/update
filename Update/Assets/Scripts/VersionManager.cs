using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Net;
using TMPro;
using UnityEngine;

public class VersionManager : MonoBehaviour
{

    [Header("Use the 'Build' Menu item to export builds!")]
    [SerializeField] TMP_Text _screenDebug;
    [SerializeField] TMP_InputField _downloadTargetText;
    [SerializeField] string _targetVersion = "V2";

    public readonly string CURFOLDER = Environment.CurrentDirectory + @"/UpdateFolder/";
    public readonly string TEMPPATH = Path.Combine(Path.GetTempPath(), "Update.zip");

    public void ChangeVersion()
    {
        Download(_url);
    }

    void Awake()
    {
        _url = $"https://raw.githubusercontent.com/tactivesport/update/main/{_targetVersion}.zip";
    }

    void Start()
    {
        _downloadTargetText.text = _url;
    }

    void OnDownloadTargetChanged(string newUrl)
    {
        _url = newUrl;
    }

    void Download(string uri)
    {
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
        Process.Start(CURFOLDER + $"{_targetVersion}.exe");

        Log("Quitting");
        Application.Quit();

        // Powershell? https://stackoverflow.com/questions/58318026/how-to-uninstall-a-uwp-application-programmatically-without-admin-access-c-sharp
    }

    void Log(string text)
    {
        UnityEngine.Debug.Log(text);
        _screenDebug.text = text;
    }

    #region Events

    void OnEnable()
    {
        _downloadTargetText.onValueChanged.AddListener(OnDownloadTargetChanged);
    }

    void OnDisable()
    {
        _downloadTargetText.onValueChanged.RemoveListener(OnDownloadTargetChanged);
    }

    #endregion

    string _url;
}