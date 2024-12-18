using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace GrassRandoV2.IC.Modules
{
    class Behaviour : MonoBehaviour
    {
        public event EventHandler? OnUpdate;

        public void Update()
        {
            OnUpdate?.Invoke(this, EventArgs.Empty);
        }

        public static Behaviour CreateBehaviour()
        {
            // A game object that will do nothing quietly in the corner until
            // the end of time.
            GameObject dummy = new("Behavior Container", typeof(Behaviour));
            DontDestroyOnLoad(dummy);

            return dummy.GetComponent<Behaviour>();
        }
    }
}
