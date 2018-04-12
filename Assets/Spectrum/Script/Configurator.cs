﻿using UnityEngine;

namespace Spectrum
{
    public class Configurator : MonoBehaviour
    {
        void Start()
        {
            if (Display.displays.Length > 1)
                Display.displays[1].Activate();
        }
    }
}
