using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class MothershipUI : MonoBehaviour
{
    private Text m_ResourcesText;
    private Text m_TimerText;
    private List<Text> m_MessageFeed;
    private GameObject m_AttackPanel;
    private GameObject m_DefensePanel;

    void Awake()
    {
        m_ResourcesText = transform.Find("ResourcesPanel").Find("Text").GetComponent<Text>();
        m_TimerText = transform.Find("TimerPanel").Find("Timer").GetComponent<Text>();
        m_MessageFeed = new List<Text>();
        Transform messagePanel = transform.Find("MessagePanel");
        m_MessageFeed.Add(messagePanel.Find("Text0").GetComponent<Text>());
        m_MessageFeed.Add(messagePanel.Find("Text1").GetComponent<Text>());
        m_MessageFeed.Add(messagePanel.Find("Text2").GetComponent<Text>());
        foreach (Text text in m_MessageFeed)
            text.text = "";
        m_AttackPanel = transform.Find("AttackPanel").gameObject;
        m_DefensePanel = transform.Find("DefensePanel").gameObject;
        m_AttackPanel.SetActive(false);
        m_DefensePanel.SetActive(false);
    }

    void Start()
    {
        Game.Instance.GameUICanvas = gameObject;
    }

    void Update()
    {
        m_TimerText.text = FormatTime(Game.Instance.GameTime);
        m_ResourcesText.text = Game.Instance.Resources.ToString();
        m_AttackPanel.SetActive(Game.Instance.Resources >= Game.Instance.AttackUpgradeCost);
        m_DefensePanel.SetActive(Game.Instance.Resources >= Game.Instance.DefenseUpgradeCost);
    }

    private string FormatTime(float time)
    {
        int seconds = (int)time;
        int minutes = seconds / 60;
        seconds = seconds % 60;

        return minutes + ":" + (seconds < 10 ? "0" : "") + seconds;
    }
}
