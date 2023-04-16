using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StepChallengeCardScript : MonoBehaviour
{
    public TextMeshProUGUI m_Title;
    public TextMeshProUGUI m_Describe;

    public TextMeshProUGUI m_TotalCount;
    public TextMeshProUGUI m_UserCount;

    [Header("StepCount")]

    public TextMeshProUGUI m_Step1Count;
    public TextMeshProUGUI m_Step2Count;
    public TextMeshProUGUI m_Step3Count;
    
    [Header("StepSlider")]

    public Slider m_Step1Slider;
    public Slider m_Step2Slider;
    public Slider m_Step3Slider;

    [Header("Firset Reward")]

    public GameObject m_TitleIcon_1;
    public GameObject m_ClosetIcon_1;
    public TextMeshProUGUI m_Name_1;
    public GameObject m_FalseButton_1;
    public GameObject m_TrueButton_1;
    public GameObject m_FinishButton_1;

    [Header("Second Reward")]

    public GameObject m_TitleIcon_2;
    public GameObject m_ClosetIcon_2;
    public TextMeshProUGUI m_Name_2;
    public GameObject m_FalseButton_2;
    public GameObject m_TrueButton_2;
    public GameObject m_FinishButton_2;

    [Header("Third Reward")]

    public GameObject m_TitleIcon_3;
    public GameObject m_ClosetIcon_3;
    public TextMeshProUGUI m_Name_3;
    public GameObject m_FalseButton_3;
    public GameObject m_TrueButton_3;
    public GameObject m_FinishButton_3;
}
