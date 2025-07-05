using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace AircraftLib.Managers
{
    public class WaveManager : MonoBehaviour
    {
        public static WaveManager main
        {
            get
            {
                return Player.main.gameObject.EnsureComponent<WaveManager>();
            }
        }

        public float amplitude = 0.2f;
        public float wavelength = 0.5f;
        public float speed = 1f;
        public float offset = 0f;


        public void Update()
        {
            offset += speed * Time.deltaTime;
        }

        public float GetWaveHeight(float x)
        {
            return amplitude * Mathf.Sin(x / wavelength + offset);
        }
    }
}
