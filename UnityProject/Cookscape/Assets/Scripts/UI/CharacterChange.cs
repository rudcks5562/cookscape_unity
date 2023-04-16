using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityProject.Cookscape;


public class CharacterChange : MonoBehaviour
{
    public Transform sampleView;
    void Start()
    {
        if (GameManager.instance.currAvatar == null) {
            // GameManager.instance.currAvatar;
        }
    }

    public void Activate()
    {
        GameObject gm = EventSystem.current.currentSelectedGameObject;
        string avatarName = "고기맨";
        string prefabName = "Runner_Meat";
        switch (gm.name) {
            case "Meat":
                avatarName = "고기맨";
                prefabName = "Runner_Meat";
                break;
            case "Tomato":
                avatarName = "토마토맨";
                prefabName = "Runner_Tomato";
                break;
            case "Bell Pepper":
                avatarName = "피망맨";
                prefabName = "Runner_Bell Pepper";
                break;
            case "Coke":
                avatarName = "콜라맨";
                prefabName = "Runner_Coke";
                break;
            case "Cheeze":
                avatarName = "치즈맨";
                prefabName = "Runner_Cheese";
                break;
            case "Strawberry":
                avatarName = "딸기맨";
                prefabName = "Runner_Strawberry";
                break;
            case "Milk":
                avatarName = "우유맨";
                prefabName = "Runner_Milk";
                break;
            case "Egg":
                avatarName = "에그맨";
                prefabName = "Runner_Egg";
                break;
            case "Cabbage":
                avatarName = "양배추맨";
                prefabName = "Runner_Cabbage";
                break;
            case "Croissant":
                avatarName = "크로와상맨";
                prefabName = "Runner_Croissant";
                break;
            case "Bread":
                avatarName = "토스트맨";
                prefabName = "Runner_Toast";
                break;
            case "Shrimp":
                avatarName = "새우맨";
                prefabName = "Runner_Shrimp";
                break;
            case "Flour":
                avatarName = "밀가루맨";
                prefabName = "Runner_Flour";
                break;
            case "Sausage":
                avatarName = "소세지맨";
                prefabName = "Runner_Sausage";
                break;
            default:
            avatarName = "고기맨";
            prefabName = "Runner_Meat";
            break;
        }
        StartCoroutine("ActiveSampleCharacter", prefabName);
        GameManager.instance.currAvatar = GameManager.instance.avatar[avatarName];
    }

    private void HideAllCharacter()
    {
        foreach(Transform child in sampleView)
        {
            child.gameObject.SetActive(false);
        }
    }

    private IEnumerator ActiveSampleCharacter(string prefab)
    {
        HideAllCharacter();
        yield return new WaitForSeconds(0.2f);
        sampleView.Find(prefab).gameObject.SetActive(true);
    }
}
