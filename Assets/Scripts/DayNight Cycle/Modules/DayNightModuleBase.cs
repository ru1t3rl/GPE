using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VRolijk.DayNight
{
    public abstract class DayNightModuleBase : MonoBehaviour
    {
        protected DayNightManager manager;

        void OnEnable()
        {
            manager = this.GetComponent<DayNightManager>();

            if (manager != null)
            {
                manager.AddModule(this);
            }
        }

        void OnDisable()
        {
            if (manager != null)
            {
                manager.RemoveModule(this);
            }
        }

        public abstract void UpdateModule(float intensity);
    }
}