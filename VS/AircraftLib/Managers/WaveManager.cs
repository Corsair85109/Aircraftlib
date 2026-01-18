using AircraftLib.Utility;
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
        public static WaveManager main = Player.main.gameObject.EnsureComponent<WaveManager>();

        public float baseFrequency = 0.4f;
        public float baseAmplitude = 0.25f;
        public float baseSpeed = 5f;


        public float GetWaveHeight(Vector3 position)
        {
            float time = Time.time;
            float height = 0f;

            float freq = baseFrequency;
            float amp = baseAmplitude;
            float speed = baseSpeed;


            // anti motion sickness solution
            // makes wave height the same at every position, preventing boat roll
            if (!AircraftLibPlugin.ModConfig.useAccurateWaterSurface)
            {
                height = Mathf.Sin(freq + time * speed / 2) * (amp / 2);
                return height;
            }


            // add sine waves to simulate water surface

            freq = freq * 1f;
            amp = amp * 0.7f;
            speed = speed * 1f;
            height += Mathf.Sin(position.x * freq + time * speed) * amp;


            freq = freq * 1f;
            amp = amp * 0.5f;
            speed = speed * 1.5f;
            height += Mathf.Sin(position.z * freq + time * speed) * amp;

            freq = freq * 1f;
            amp = amp * 0.1f;
            speed = speed * 2f;
            height += Mathf.Sin((position.x + position.z) * freq + time * speed) * amp;

            freq = freq * 1f;
            amp = amp * 0.1f;
            speed = speed * 4f;
            height += Mathf.Sin((position.x - position.z) * freq + time * speed) * amp;


            return height;
        }











        // stuff below here was an attempt to get the actual visual height of the water surface

        /*public WaterSurface surface;
        public Texture2D displacementTexture;

        public bool textureLoadedThisFrame = false;

        private const float maxDisplaceY = 300f;
        private const int displaceOffset = 0;


        public void GetWaterSurface()
        {
            surface = WaterSurface.Get();
        }

        public void LateFixedUpdate()
        {
            textureLoadedThisFrame = false;
        }

        // only call this in a FixedUpdate()
        public float GetWaveHeight(Vector3 worldPosition)
        {
            // get surface and texture
            if (surface == null || displacementTexture == null)
            {
                GetWaterSurface();
            }
            if (displacementTexture == null || textureLoadedThisFrame == false)
            {
                GetTexture();
            }



            // get uvs
            float patchLength = surface.GetPatchLength();
            Vector3 surfaceOrigin = surface.transform.position;
            // Convert world XZ to UV space
            float u = (worldPosition.x - surfaceOrigin.x) / patchLength + 0.5f;
            float v = (worldPosition.z - surfaceOrigin.z) / patchLength + 0.5f;
            Vector2 uv = new Vector2(Mathf.Clamp01(u), Mathf.Clamp01(v));



            // sample displacement
            Color pixel = displacementTexture.GetPixelBilinear(uv.x, uv.y);

            // get height
            float displacementY = pixel.g;
            float baseY =surface.waterOffset + displaceOffset;
            float displacement = displacementY + baseY;


            


            
            return displacement;
        }


        public void GetTexture()
        {
            RenderTexture rt = WaterSurface.StaticDisplacementTexture.displacementTexture;
            if (rt == null)
            {
                Debug.LogError("Displacement RenderTexture is null.");
                return;
            }

            // Create or resize the Texture2D if needed
            if (displacementTexture == null ||
                displacementTexture.width != rt.width ||
                displacementTexture.height != rt.height)
            {
                displacementTexture = new Texture2D(rt.width, rt.height, TextureFormat.RGBAFloat, false, true);
            }

            RenderTexture currentRT = RenderTexture.active;
            RenderTexture.active = rt;
            displacementTexture.ReadPixels(new Rect(0, 0, rt.width, rt.height), 0, 0);
            displacementTexture.Apply();
            RenderTexture.active = currentRT;

            textureLoadedThisFrame = true;
        }*/
    }
}
