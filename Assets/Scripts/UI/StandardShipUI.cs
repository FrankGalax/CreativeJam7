using UnityEngine;
using UnityEngine.UI;

public class StandardShipUI : MonoBehaviour
{
    Text m_TimerText;

    void Start()
    {
        m_TimerText = transform.Find("TimerPanel").Find("Timer").GetComponent<Text>();
    }

    void Update()
    {
        m_TimerText.text = FormatTime(Game.Instance.GameTime);
    }

    private string FormatTime(float time)
    {
        int seconds = (int)time;
        int minutes = seconds / 60;
        seconds = seconds % 60;

        return minutes + ":" + (seconds < 10 ? "0" : "") + seconds;
    }
}