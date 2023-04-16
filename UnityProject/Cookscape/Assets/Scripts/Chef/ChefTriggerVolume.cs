using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnityProject.Cookscape
{
    public class ChefTriggerVolume : MonoBehaviour
    {
        public bool CanImprisonCatchee = false;
        public bool CanCatchFoods = false;

        private void OnTriggerEnter(Collider other)
        {
            CheckTrigger(other, true);
        }

        private void OnTriggerExit(Collider other)
        {
            CheckTrigger(other, false);
        }

        void CheckTrigger(Collider other, bool flag)
        {
            if (other.CompareTag("Jail"))
            {
                CanImprisonCatchee = flag;
            }
            else if (other.CompareTag("Chef"))
            {
                CanCatchFoods = flag;
            }
        }
    }
}
