using UnityEngine;

namespace Prisma
{
    public class DisplaySelector : MonoBehaviour
    {
        [SerializeField] Camera[] _cameras;

        void Start()
        {
            // if (Application.isEditor)
            // {
            //     _cameras[0].targetDisplay = 0;
            //     _cameras[1].targetDisplay = 1;
            // }
            // else
            {

                // Triple mode
                _cameras[0].targetDisplay = 0; // Monitor -> Primary
                _cameras[1].targetDisplay = 1; // Front -> Secondary
                TryActivateDisplay(0);
                TryActivateDisplay(1);
            }
        }

        void TryActivateDisplay(int index)
        {
            if (index < Display.displays.Length)
                Display.displays[index].Activate();
        }
    }
}
