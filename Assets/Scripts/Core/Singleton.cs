using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Simulations.Core
{
    public class Singleton : MonoBehaviour
    {
        public static Singleton instance = null;

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
    }
}
