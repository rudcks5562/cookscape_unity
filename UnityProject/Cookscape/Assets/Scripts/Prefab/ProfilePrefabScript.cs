using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ProfilePrefabScript : MonoBehaviour
{
    [Header("Rank")]
    public GameObject GOLD_ICON;
    public GameObject SILVER_ICON;
    public GameObject BRONZE_ICON;

    public TMP_Text NICKNAME;
    public TMP_Text USER_TITLE;

    public Slider RANK_SLIDER;
    public TMP_Text RANK_POINT;

    public void SetBronzeTier()
    {
        GOLD_ICON.SetActive(false);
        SILVER_ICON.SetActive(false);
        BRONZE_ICON.SetActive(true);
    }

    public void SetSilverTier()
    {
        GOLD_ICON.SetActive(false);
        SILVER_ICON.SetActive(true);
        BRONZE_ICON.SetActive(false);
    }

    public void SetGoldTier()
    {
        GOLD_ICON.SetActive(true);
        SILVER_ICON.SetActive(false);
        BRONZE_ICON.SetActive(false);
    }
}
