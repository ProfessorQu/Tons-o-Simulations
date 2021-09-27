using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Simulations.Core
{
    public abstract class MasterBase : MonoBehaviour
    {
        public static MasterBase instance = null;

        public abstract float SimulationSpeed { get; set; }

        protected void Awake()
        {
            if (instance != null && instance != this)
            {
                Destroy(gameObject);
			}
            else
            {
                instance = this;
			}
		}

        public abstract void Step();

        public abstract void StartSimulation();
        public abstract void StopSimulation();
    }
}
