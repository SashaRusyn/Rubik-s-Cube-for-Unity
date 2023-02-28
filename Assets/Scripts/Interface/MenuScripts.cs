using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuScripts : MonoBehaviour
{
    public Text textRecord;
    public void Play()
    {
        SceneManager.LoadScene(2);
    }

    public void Simulation()
    {
        SceneManager.LoadScene(1);
    }

    public void Achievement()
    {
        string line = "";
        string text = line;
        using (StreamReader sr = new StreamReader("Assets\\Files\\result.txt"))
        {
            int i = 0;
            while ((line = sr.ReadLine()) != null)
            {
                text += (i + 1).ToString();
                i++;
                text += ". ";
                text += line;
                text += '\n';
            }
        }
        textRecord.text = text;
    }

    public void Exit()
    {
        Application.Quit();
    }
}
