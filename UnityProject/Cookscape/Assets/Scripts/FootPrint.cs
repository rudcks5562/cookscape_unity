using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FootPrint : MonoBehaviour
{
    #region EXTERNAL VARIABLES
    // EXTERNAL VARIABLES
    float m_LifeTime = 10.0f;
    #endregion

    #region PRIVATE VARIABLES
    private float m_Mark;
    private Vector3 m_OrigSize;
    #endregion

    #region LIFECYCLE
    void Start()
    {
        m_Mark = Time.time;
        m_OrigSize = this.transform.localScale;
    }

    void Update()
    {
        float ElapsedTime = Time.time - m_Mark;

        if (ElapsedTime != 0) {
            float PercentTimeLeft = (m_LifeTime - ElapsedTime) / m_LifeTime;

            this.transform.localScale = new Vector3(m_OrigSize.x * PercentTimeLeft, m_OrigSize.y * PercentTimeLeft, m_OrigSize.z * PercentTimeLeft);
            if (ElapsedTime > m_LifeTime) {
                Destroy(this.gameObject);
            }
        }
    }
    #endregion
}
