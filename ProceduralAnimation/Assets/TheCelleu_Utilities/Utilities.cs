using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Thecelleu
{
    static class Utilities
    {
        /// <summary>
        /// Damping towards zero. This function is not time boxed, there is not a fixed timespan to get to 0.
        /// </summary>
        /// <param name="source">Source value. Usualy the current Value of an object</param>
        /// <param name="smoothing">Smoothing rate dictates the proportion of source remaining after one second</param>
        /// <param name="dt">Deltatime</param>
        /// <returns></returns>
        public static float Damp(float source, float smoothing, float dt)
        {
            return source * Mathf.Pow(smoothing, dt);
        }

        /// <summary>
        /// Framerate aware Damp function. Use instead of a = Mathf.Lerp(a, b, r);.
        /// See http://www.rorydriscoll.com/2016/03/07/frame-rate-independent-damping-using-lerp/ for an extensive explanation
        /// </summary>
        /// <param name="a">Source value. Usualy the current Value of an object</param>
        /// <param name="b">Target value</param>
        /// <param name="lambda">Smoothing rate dictates the proportion of source remaining after one second</param>
        /// <param name="dt">Deltatime</param>
        /// <returns></returns>
        public static float Damp(float a, float b, float lambda, float dt)
        {
            return Mathf.Lerp(a, b, 1 - Mathf.Exp(-lambda * dt));
        }

        // sound
        // maybe to do https://johnleonardfrench.com/articles/how-to-fade-audio-in-unity-i-tested-every-method-this-ones-the-best/
    }
}