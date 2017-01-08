using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class MothershipUI : MonoBehaviour
{
    private Text m_ResourcesText;
    private Text m_TimerText;
    private GameObject m_AttackPanelUnavailable;
    private GameObject m_AttackPanelAvailable;
    private GameObject m_AttackPanelActivated;

    private GameObject m_DefensePanelUnavailable;
    private GameObject m_DefensePanelAvailable;
    private GameObject m_DefensePanelActivated;

    void Start()
    {
        m_ResourcesText = transform.Find("ResourcesPanel").Find("Text").GetComponent<Text>();
        m_TimerText = transform.Find("TimerPanel").Find("Timer").GetComponent<Text>();

        GameObject attackPanel = transform.Find("AttackPanel").gameObject;
        GameObject defensePanel = transform.Find("DefensePanel").gameObject;

        m_AttackPanelUnavailable = attackPanel.transform.Find("AttackImageUnavailable").gameObject;
        m_AttackPanelAvailable = attackPanel.transform.Find("AttackImageAvailable").gameObject;
        m_AttackPanelActivated = attackPanel.transform.Find("AttackImageActivated").gameObject;
        m_DefensePanelUnavailable = defensePanel.transform.Find("DefenseImageUnavailable").gameObject;
        m_DefensePanelAvailable = defensePanel.gameObject.transform.Find("DefenseImageAvailable").gameObject;
        m_DefensePanelActivated = defensePanel.gameObject.transform.Find("DefenseImageActivated").gameObject;

        transform.Find("AttackPanel").gameObject.transform.Find("Text").GetComponent<Text>().text = GameObject.FindObjectOfType<FatAssShip>().offensePowerUp.price.ToString();
        transform.Find("DefensePanel").gameObject.transform.Find("Text").GetComponent<Text>().text = GameObject.FindObjectOfType<FatAssShip>().defensePowerUp.price.ToString();

        m_AttackPanelUnavailable.SetActive(true);
        m_AttackPanelAvailable.SetActive(false);
        m_AttackPanelActivated.SetActive(false);
        m_DefensePanelUnavailable.SetActive(true);
        m_DefensePanelAvailable.SetActive(false);
        m_DefensePanelActivated.SetActive(false);
    }

    void Update()
    {
        m_TimerText.text = FormatTime(Game.Instance.GameTime);
        m_ResourcesText.text = Game.Instance.Resources.ToString();

        FatAssShip fas = GameObject.FindObjectOfType<FatAssShip>();
        if (fas.m_offenseCD > 0.0f)
        {
            m_AttackPanelActivated.SetActive(true);
            m_AttackPanelUnavailable.SetActive(false);
            m_AttackPanelAvailable.SetActive(false);
        }
        else if (fas.offensePowerUp.price <= Game.Instance.Resources)
        {
            m_AttackPanelActivated.SetActive(false);
            m_AttackPanelUnavailable.SetActive(false);
            m_AttackPanelAvailable.SetActive(true);
        }
        else
        {
            m_AttackPanelActivated.SetActive(false);
            m_AttackPanelUnavailable.SetActive(true);
            m_AttackPanelAvailable.SetActive(false);
        }

        if (fas.m_defenseCD > 0.0f)
        {
            m_DefensePanelActivated.SetActive(true);
            m_DefensePanelUnavailable.SetActive(false);
            m_DefensePanelAvailable.SetActive(false);
        }
        else if (fas.defensePowerUp.price <= Game.Instance.Resources)
        {
            m_DefensePanelActivated.SetActive(false);
            m_DefensePanelUnavailable.SetActive(false);
            m_DefensePanelAvailable.SetActive(true);
        }
        else
        {
            m_DefensePanelActivated.SetActive(false);
            m_DefensePanelUnavailable.SetActive(true);
            m_DefensePanelAvailable.SetActive(false);
        }
    }

    private string FormatTime(float time)
    {
        int seconds = (int)time;
        int minutes = seconds / 60;
        seconds = seconds % 60;

        return minutes + ":" + (seconds < 10 ? "0" : "") + seconds;
    }
}
