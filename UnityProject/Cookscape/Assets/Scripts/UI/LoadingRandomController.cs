using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnityProject.Cookscape {
    public class LoadingRandomController : MonoBehaviour
    {
        public int randomNum;

        private void Awake()
        {
            randomNum = Random.Range(0, transform.childCount);
            transform.GetChild(randomNum).gameObject.SetActive(true);
        }
    }
}
