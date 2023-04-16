using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoToJoinButton : MonoBehaviour
{
    const string HOME_PAGE_URL = "https://j8b109.p.ssafy.io/";

    public void GoToHomepage()
    {
        Application.OpenURL(HOME_PAGE_URL);
    }
}
