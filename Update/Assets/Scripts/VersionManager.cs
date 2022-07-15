using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Net;
using TMPro;
using UnityEngine;
using Debug = UnityEngine.Debug;

public class VersionManager : MonoBehaviour
{
    
    [Header("Use the 'Build' Menu item to export builds!")] [SerializeField] TMP_Text _screenDebug;
    [SerializeField] string _tempfolder;
    [SerializeField] TMP_InputField _downloadTargetText;
    [SerializeField] string _targetVersion = "V2";

    public void ChangeVersion()
    {
        Download(_url);
    }

    void Awake()
    {
        _url = $"https://raw.githubusercontent.com/tactivesport/update/main/{_targetVersion}.zip";
        _tempfolder = CreateUniqueTempDirectory();
    }

    public string CreateUniqueTempDirectory()
    {
        var uniqueTempDir = Path.GetFullPath(Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString()));
        Directory.CreateDirectory(uniqueTempDir);
        return uniqueTempDir;
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

        var client = new WebClient();
        client.DownloadProgressChanged += (sender, args) => Log($"Download progress: {args.ProgressPercentage}%");
        client.DownloadFileCompleted += ProceedUnzip;
        client.DownloadFileAsync(new Uri(uri), _tempfolder + "/Update.zip");
    }

    void ProceedUnzip(object sender, AsyncCompletedEventArgs e)
    {
        if (e.Cancelled)
        {
            Log("Download cancelled error");
            return;
        }
        
        Log("Attempt unzipping");
        ZipFile.ExtractToDirectory(_tempfolder+"/Update.zip", _tempfolder);
        
        Log("Initiate Quitting next frame");
        Application.Quit();

#if UNITY_EDITOR
        OpenExplorerWindowAtPath(_tempfolder);
#else
        OverrideCurrentBuild();
        Process.Start($"{_targetVersion}.exe");
#endif


    }
    
    void OverrideCurrentBuild() // dont run in editor, otherwise the repository is overwritten.
    {
        /*
        * use /c for closing console instead of /k (which keeps the window open)
        * use /mir to delte content of current folder.
        * Maybe mirror is not needed when both applications have the same prodcut name!!!
        * (then any data files on the side would be preserved)
        * Use powershell for UWP: https://stackoverflow.com/questions/58318026/how-to-uninstall-a-uwp-application-programmatically-without-admin-access-c-sharp
        */
        Log("Attempt to replace the current application");
        var psi = new ProcessStartInfo
        {
            FileName = "cmd.exe",
            Arguments = $"/k robocopy {_tempfolder} . /mir"
        };
        Process.Start(psi);

    }
        
    void OpenExplorerWindowAtPath(string path)
    {
        var process = new Process();
        process.StartInfo.FileName = "explorer.exe";
        process.StartInfo.Arguments = path;
        process.Start();
    }

    void Log(string text)
    {
        Debug.Log(text);
        _screenDebug.text = text;
    }

    string _url;

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
}