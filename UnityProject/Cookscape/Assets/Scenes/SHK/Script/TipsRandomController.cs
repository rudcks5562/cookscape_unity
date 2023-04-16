using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnityProject.Cookscape {
    public class TipsRandomController : LoadingRandomController
    {
        float time;
        public int waitingTime;

        // Start is called before the first frame update
        void Start()
        {
            Debug.Log(randomNum);
        }

        // Update is called once per frame
        void Update()
        {
            time += Time.deltaTime;
            if ((int)time > waitingTime)
            {
                transform.GetChild(randomNum).gameObject.SetActive(false);
                randomNum = Random.Range(0, transform.childCount);
                transform.GetChild(randomNum).gameObject.SetActive(true);
                time = 0;
            }
        }
    }
}
