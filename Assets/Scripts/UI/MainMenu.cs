using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class MainMenu : MonoBehaviour
{
    private string m_Username;
    private Button m_PlayButton;

    void Start()
    {
        m_PlayButton = Transform.FindObjectOfType<Canvas>().transform.Find("PlayButton").GetComponent<Button>();
        m_PlayButton.enabled = false;
    }

    public void SetUsername(string username)
    {
        m_Username = username;
        m_PlayButton.enabled = username.Length > 0;
    }

    public void Play()
    {
        Debug.Log(m_Username);
    }
}
