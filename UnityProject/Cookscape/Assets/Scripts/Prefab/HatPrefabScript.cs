using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityProject.Cookscape;

public class HatPrefabScript : MonoBehaviour
{
    public string m_HatName;
    public string m_Description;
    public string m_KeyValue;
    public bool m_IsHave;
    public Image m_Icon;

    public List<GameObject> m_HatObjectList;
    public List<Texture2D> m_IconList;
    public static int check = 0;

    public void ChangeHat(bool flag)
    {
        if (flag == true)
        {
            //preview is change
            if ( RewardBookScript.instance.currentPreviewObject != null )
            {
                RewardBookScript.instance.currentPreviewObject.SetActive(false);
            }

            GameObject tmp = null;

            string KEYWORD = m_KeyValue;

            RewardBookScript.instance.NowKeyValue = KEYWORD;

            string targetPrefab = null;
            switch (KEYWORD)
            {
                case nameof(RewardData.REWARD.요리사모자):
                    targetPrefab = "m_ChefHat01";
                    break;
                case nameof(RewardData.REWARD.왕관):
                    targetPrefab = "m_CrownHat01";
                    break;
                case nameof(RewardData.REWARD.보글보글모자):
                    targetPrefab = "m_PirateHat01";
                    break;
                case nameof(RewardData.REWARD.혹):
                    targetPrefab = "m_MushroomHat01";
                    break;
                case nameof(RewardData.REWARD.GOD):
                    targetPrefab = "m_ElephantHat01";
                    break;
                case nameof(RewardData.REWARD.물음표의모자):
                    targetPrefab = "m_PartyHat01";
                    break;
                default:
                    Debug.Log($"{KEYWORD}");
                    break;
            }

            GameObject item;
            for (int i = m_HatObjectList.Count - 1; i >= 0; i--)
            {
                item = m_HatObjectList[i];
                if (targetPrefab.Equals(item.name))
                {
                    tmp = item;
                    break;
                }
            }
            
            RewardBookScript.instance.currentPreviewObject = Instantiate(tmp);
            RewardBookScript.instance.currentPreviewObject.transform.position = new Vector3(0, -302.5f, 50f);
            RewardBookScript.instance.currentPreviewObject.transform.rotation = Quaternion.identity;

            RewardBookScript.instance.PreviewCamera.enabled = true;
            RewardBookScript.instance.PreviewCamera.Render();
            RewardBookScript.instance.PreviewCamera.enabled = false;

            RewardBookScript.instance.ClosetPage_PreviewBox.GetComponentInChildren<RawImage>().texture = RewardBookScript.instance.renderTexture;

            //set text
            RewardBookScript.instance.ClosetPage_Hat_Name.text = m_HatName;
            RewardBookScript.instance.ClosetPage_Hat_Description.text = m_Description;

            if (m_IsHave)
            {
                RewardBookScript.instance.ClosetPage_HaveTrue.SetActive(true);
                RewardBookScript.instance.ClosetPage_HaveFalse.SetActive(false);
            }
            else
            {
                RewardBookScript.instance.ClosetPage_HaveFalse.SetActive(true);
                RewardBookScript.instance.ClosetPage_HaveTrue.SetActive(false);
            }
            
        }
    }
}
