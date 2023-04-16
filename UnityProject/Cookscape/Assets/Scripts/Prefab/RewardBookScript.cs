using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RewardBookScript : MonoBehaviour
{
    public static RewardBookScript instance;

    void Awake()
    {
        if ( instance == null)
        {
            instance = this;
        }

        PreviewCamera = new GameObject("Preview Camera").AddComponent<Camera>();
        PreviewCamera.transform.position = new Vector3(0, -300f, 0);
        PreviewCamera.fieldOfView = 10;
        PreviewCamera.enabled = false;
        PreviewCamera.targetTexture = renderTexture;

        NowKeyValue = GameManager.instance.user.hat;
    }

    public Camera PreviewCamera { get; private set; }
    public RenderTexture renderTexture;
    public GameObject currentPreviewObject;

    public GameObject NicknamePage;
    public GameObject NicknamePage_ContentBox;
    public GameObject NicknamePage_Content;
    public Slider NicknamePage_Slider;
    public TextMeshProUGUI NicknamePage_Percentage;

    public string NowKeyValue;

    public GameObject ClosetPage;
    public GameObject ClosetPage_PreviewBox;
    public GameObject ClosetPage_ContentBox;
    public GameObject ClosetPage_Content;

    public GameObject ClosetPage_HaveTrue;
    public GameObject ClosetPage_HaveFalse;
    
    public TextMeshProUGUI ClosetPage_Hat_Name;
    public TextMeshProUGUI ClosetPage_Hat_Description;
    public Slider ClosetPage_Slider;
    public TextMeshProUGUI ClosetPage_Percentage;

    public GameObject ChallengeSoloPage;
    public GameObject ChallengeSoloPage_ContentBox;
    public GameObject ChallengeSoloPage_Content;

    public GameObject ChallengeStepPage;
    public GameObject ChallengeStepPage_ContentBox;
    public GameObject ChallengeStepPage_Content;

}
