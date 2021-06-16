﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FateGames
{
    public class ColorManager : MonoBehaviour
    {
        public static ColorManager Instance;
        private static List<ITransition> transitions = null;

        private void Awake()
        {
            if (Instance)
                return;
            Instance = this;
            transitions = new List<ITransition>();
        }

        private interface ITransition
        {
            bool NextFrame();
        }
        private class ColorTransition : ITransition
        {
            private Renderer rend;
            private int materialIndex;
            private Gradient gradient;
            private float time;
            private float currentTime = 0;



            public ColorTransition(Renderer rend, int materialIndex, Gradient gradient, float time)
            {
                this.rend = rend;
                this.materialIndex = materialIndex;
                this.gradient = gradient;
                this.time = time;
            }

            public bool NextFrame()
            {
                Color color = gradient.Evaluate(currentTime / time);
                currentTime = Mathf.Clamp(currentTime + Time.deltaTime, 0, time);
                rend.materials[materialIndex].color = color;
                return currentTime != time;

            }

        }

        private class AlphaTransition : ITransition
        {
            private Renderer rend;
            private int materialIndex;
            private float from;
            private float to;
            private float time;
            private float currentTime = 0;



            public AlphaTransition(Renderer rend, int materialIndex, float to, float time)
            {
                this.rend = rend;
                this.materialIndex = materialIndex;
                this.from = rend.materials[materialIndex].color.a;
                this.to = to;
                this.time = time;
            }

            public bool NextFrame()
            {
                Color color = SetAlpha(rend.materials[materialIndex].color, from + (currentTime / time) * (to - from));
                currentTime = Mathf.Clamp(currentTime + Time.deltaTime, 0, time);
                rend.materials[materialIndex].color = color;
                return currentTime != time;

            }

        }

        private void Update()
        {
            int i = 0;
            while (i < transitions.Count)
            {
                ITransition transition = transitions[i];
                if (!transition.NextFrame())
                {
                    transitions.Remove(transition);
                    continue;
                }
                ++i;
            }
        }

        public static void DoColorTransition(Renderer rend, int materialIndex, Gradient gradient, float time)
        {
            transitions.Add(new ColorTransition(rend, materialIndex, gradient, time));
        }

        public static void DoAlphaTransition(Renderer rend, int materialIndex, float to, float time)
        {
            transitions.Add(new AlphaTransition(rend, materialIndex, to, time));
        }


        public static Gradient CreateGradient(Color firstColor, Color lastColor)
        {
            Gradient gradient = new Gradient();
            GradientColorKey[] colorKeys = new GradientColorKey[2];
            colorKeys[0].color = firstColor;
            colorKeys[0].time = 0.0f;
            colorKeys[1].color = lastColor;
            colorKeys[1].time = 1.0f;
            gradient.colorKeys = colorKeys;
            return gradient;
        }

        public static float GetHue(Color color)
        {
            float H, S, V;
            Color.RGBToHSV(color, out H, out S, out V);
            return H;
        }

        public static Color SetHue(Color color, float newHue)
        {
            float H, S, V;
            Color.RGBToHSV(color, out H, out S, out V);
            Color newColor = Color.HSVToRGB(newHue / 360f, S, V);
            return newColor;
        }

        public static float GetSaturation(Color color)
        {
            float H, S, V;
            Color.RGBToHSV(color, out H, out S, out V);
            return S;
        }

        public static Color SetSaturation(Color color, float newSaturation)
        {
            float H, S, V;
            Color.RGBToHSV(color, out H, out S, out V);
            Color newColor = Color.HSVToRGB(H, newSaturation, V);
            return newColor;
        }
        public static float GetValue(Color color)
        {
            float H, S, V;
            Color.RGBToHSV(color, out H, out S, out V);
            return V;
        }

        public static Color SetValue(Color color, float newValue)
        {
            float H, S, V;
            Color.RGBToHSV(color, out H, out S, out V);
            Color newColor = Color.HSVToRGB(H, S, newValue);
            return newColor;
        }

        public static Color SetAlpha(Color color, float newAlpha)
        {
            Color newColor = new Color(color.r, color.g, color.b, newAlpha);
            return newColor;
        }
    }
}