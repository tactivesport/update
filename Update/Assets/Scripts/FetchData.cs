using System;
using System.IO;
using System.Net;
using TMPro;
using UnityEngine;

public class FetchData : MonoBehaviour
{
    const string PATH = "https://raw.githubusercontent.com/tactivesport/update/main/";
    readonly string CURFOLD = Environment.CurrentDirectory + @"\UpdateFolder\";
    
    public void Fetch(TMP_Text txt)
    {
        if (!Directory.Exists(CURFOLD))
            Directory.CreateDirectory(CURFOLD);
        
        using (var client = new WebClient())
        {
            client.DownloadFile(PATH + "test.txt", CURFOLD + "test.txt");
            client.DownloadFileCompleted += (sender, args) => Debug.Log("File Downloaded!");
        }

        txt.text = File.ReadAllText(CURFOLD + "test.txt");
    }
}
