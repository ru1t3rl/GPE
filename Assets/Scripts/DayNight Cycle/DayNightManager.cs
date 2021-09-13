using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VRolijk.DayNight
{
    public class DayNightManager : MonoBehaviour
    {
        public bool pause = false;
        public bool syncAtStart = false;

        [Header("Time")]
        [Tooltip("Day Length in Minutes")]
        [SerializeField] float _targetDayLength = .5f;
        public float targetDayLength => _targetDayLength;

        [SerializeField, Range(0f, 1f)] float _timeOfDay;
        public float timeOfDay => _timeOfDay;

        [SerializeField] int _dayNumber;
        public int dayNumber => _dayNumber;

        [SerializeField] int _yearNumber;
        public int yearNumber => _yearNumber;

        float _timeScale = 0f;
        [SerializeField] int _yearLength = 365;
        public int yearLength => _yearLength;

        [Header("Sun Light")]
        [SerializeField] Transform dailyRotation;
        [SerializeField] Light sun;
        float intensity;

        [Header("Seasonal")]
        [SerializeField] Transform seansonalRotation;
        [SerializeField, Range(-45f, 45f)] float maxSeasonalTilt;

        [Header("Modules")]
        List<DayNightModuleBase> modules = new List<DayNightModuleBase>();

        private void Start()
        {
            if (syncAtStart)
            {
                float minute = System.DateTime.Now.Minute * 1.0f / 60.0f;
                float realTime = System.DateTime.Now.Hour + minute;
                _timeOfDay = realTime / 24;
            }
        }

        void Update()
        {
            if (!pause)
            {
                UpdateTimeScale();
                UpdateTime();
            }

            AdjustSunRotation();
            UpdateIntensity();

            UpdateModules();
        }

        void UpdateTimeScale()
        {
            _timeScale = 24 / (_targetDayLength / 60);
        }

        void UpdateTime()
        {
            _timeOfDay += Time.deltaTime * _timeScale / 86400; // seconds in a day
            if (_timeOfDay > 1)
            {
                _dayNumber++;
                _timeOfDay -= 1;

                // Check for new year
                if (_dayNumber > _yearLength)
                {
                    _yearNumber++;
                    _dayNumber = 0;
                }
            }
        }


        void AdjustSunRotation()
        {
            float sunAngle = timeOfDay * 360;
            dailyRotation.transform.localRotation = Quaternion.Euler(0, 0, sunAngle);

            float seasonalAngle = -maxSeasonalTilt * Mathf.Cos((float)dayNumber / (float)yearLength * 2f * Mathf.PI);
            seansonalRotation.transform.localRotation = Quaternion.Euler(seasonalAngle, 0f, 0f);
        }

        void UpdateIntensity()
        {
            intensity = Vector3.Dot(sun.transform.forward, Vector3.down);
            intensity = Mathf.Clamp01(intensity);
        }

        public void AddModule(DayNightModuleBase module)
        {
            modules.Add(module);
        }

        public void RemoveModule(DayNightModuleBase module)
        {
            modules.Remove(module);
        }

        void UpdateModules()
        {
            for (int iModule = 0; iModule < modules.Count; iModule++)
            {
                modules[iModule].UpdateModule(intensity);
            }
        }

        public void MakeDay()
        {
            _timeOfDay = .5f;
        }

        public void MakeNight()
        {
            _timeOfDay = 0f;
        }

        public void PauseTime()
        {
            pause = true;
        }

        public void ContinueTime()
        {
            pause = false;
        }

        public void MakeSunSet()
        {
            _timeOfDay = .72f;
        }

        public void MakeSunRise()
        {
            _timeOfDay = .255f;
        }
    }
}