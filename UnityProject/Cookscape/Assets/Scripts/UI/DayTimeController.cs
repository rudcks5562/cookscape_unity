using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
// using UnityEngine.Rendering.Universal;

public class DayTimeController : MonoBehaviour
{   
    float m_Time;
    bool m_IsActive = false;
    // [SerializeField] bool m_IsRPCTime = false;

    const float DayForSeconds = 86400f;
    [SerializeField] Color m_NightLightColor;
    [SerializeField] AnimationCurve m_NightTimeCurve;
    [SerializeField] Color m_DayLightColor = Color.white;
    [SerializeField] float m_TimeScale = 60f;
    [SerializeField] GameObject m_GlobalLightAxis;
    private Light m_GlobalLight;

    private Vector3 m_Init_Angle;
    private int days;
    private bool m_IsNight = false;

    #region FOG SYSTEM
    [SerializeField] private float m_NightFogDistance;
    private float m_DayFogDistance = 100000;
    [SerializeField] private float m_FogDensityCalc;
    private float m_CurrentFogDistance = 100000; 
    #endregion

    #region SKY SYSTEM
    private Material Skybox;
    public float speed;
    private float Tfactor;
    public float NightValue;
    public float DayValue;
    public float ChangeSpeed;
    #endregion


    float Hours{
        get{
            return m_Time / 3600f;
        }
    }

    float Minutes{
        get{
            return m_Time % 3600f / 60f;
        }
    }

    private void Awake()
    {
        if (m_GlobalLightAxis != null) {
            m_Init_Angle = m_GlobalLightAxis.transform.eulerAngles;
            Tfactor =  NightValue;
        }
    }

    private void Start()
    {
        SkyBoxInit();
    }

    private void Update()
    {
        // Is DayTimeController Activated?
        if (m_IsActive) {
            m_Time += Time.deltaTime * m_TimeScale; 
            int hh = (int)Hours;
            int mm = (int)Minutes;
            float v = m_NightTimeCurve.Evaluate(Hours);
            Color c = Color.Lerp(m_DayLightColor, m_NightLightColor, v);
            m_GlobalLight.color = c;
            //m_GlobalLightAxis.transform.Rotate(Vector3.right * Time.deltaTime * m_TimeScale * 0.1f);
            
            if (m_GlobalLightAxis.transform.eulerAngles.x >= 160)
                m_IsNight = true;
            else if (m_GlobalLightAxis.transform.eulerAngles.x <= 10)
                m_IsNight = false;

            FogUpdate();
            SkyUpdate();

            // ?˜ì¤‘??? ì§œ ê³„ì‚°???œë‹¤ë©?..
            if(m_Time > DayForSeconds){
                NextDay();
            }
        }
    }

    public void ActiveDayTimeController() {
        SkyBoxInit();
        m_GlobalLightAxis.transform.eulerAngles = m_Init_Angle;
        this.m_IsActive = true;
    }

    public void DisableDayTimeController() {
        m_GlobalLightAxis.transform.eulerAngles = m_Init_Angle;
        this.m_IsActive = false;
    }

    private void NextDay(){
        m_Time = 0;
        days += 1;
    }

    public void SetCurrentFogDensity(float distance) {
        this.m_CurrentFogDistance = distance;
    }

    public void FogUpdate()
    {
        if (!m_IsNight)
        {
            if (m_CurrentFogDistance <= m_DayFogDistance)
            {
                m_CurrentFogDistance += m_FogDensityCalc * Time.deltaTime * 0.5f;
                RenderSettings.fogEndDistance = m_CurrentFogDistance;
            }
        }
        else
        {
            if (m_CurrentFogDistance >= m_NightFogDistance)
            {
                m_CurrentFogDistance -= m_FogDensityCalc * Time.deltaTime * 0.5f;
                RenderSettings.fogEndDistance = m_CurrentFogDistance;
            }
        }
    }

    public void SkyBoxInit()
    {
        Skybox = RenderSettings.skybox;
        m_GlobalLight = m_GlobalLightAxis.transform.Find("Directional Light").GetComponent<Light>();
    }

    public void SkyUpdate()
    {
        Skybox.SetFloat("_Rotation", Skybox.GetFloat("_Rotation") + Time.deltaTime * speed);

        if(m_IsNight == true && Tfactor >= NightValue){
            Tfactor -= Time.deltaTime * ChangeSpeed;

            Skybox.SetColor("_Tint", new Color(Tfactor, Tfactor, Tfactor, 1));

        }else if(m_IsNight == false && Tfactor <= DayValue){
            Tfactor += Time.deltaTime * ChangeSpeed;

            Skybox.SetColor("_Tint", new Color(Tfactor, Tfactor, Tfactor,1));
        }
    }
}