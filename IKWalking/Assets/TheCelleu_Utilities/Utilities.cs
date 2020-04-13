using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Thecelleu
{

    static class Utilities
    {
        private static System.Random randomGen = new System.Random();

		/// <summary>
		/// Damping towards zero. There is not a fixed timespan to get to 0. Smoothing is the amount of source remaining after 1 second.
		/// </summary>
		/// <param name="source">Source value. Usualy the current Value of an object</param>
		/// <param name="partOfSourceAfterASecond">Smoothing rate: The proportion of source remaining after one second</param>
		/// <param name="dt">usually Time.deltatime</param>
		/// <returns></returns>
		public static float Damp(float source, float partOfSourceAfterASecond, float dt)
        {
            return source * Mathf.Pow(partOfSourceAfterASecond, dt);
        }

		/// <summary>
		/// Framerate aware Damp function. Use instead of a = Mathf.Lerp(a, b, r);.
		/// There is not a fixed timespan to get to 0. Smoothing is the amount of source remaining after 1 second.
		/// See http://www.rorydriscoll.com/2016/03/07/frame-rate-independent-damping-using-lerp/ for an extensive explanation
		/// </summary>
		/// <param name="a">Source value. Usually the current Value of an object</param>
		/// <param name="b">Target value.</param>
		/// <param name="lambda">Smoothing rate: The proportion of source remaining after one second</param>
		/// <param name="dt">usually Time.deltatime</param>
		/// <returns></returns>
		public static float Damp(float a, float b, float lambda, float dt)
        {
            return Mathf.Lerp(a, b, 1 - Mathf.Exp(-lambda * dt));
        }

        /// <summary>
        /// Returns a random enumeration value from all the enums of this type.
        /// </summary>
        /// <typeparam name="T">Enumeration Type</typeparam>
        /// <returns>Random value of this enumeration type</returns>
        public static T RandomEnumValue<T>()
        {
            var enumvaluesAsArray = System.Enum.GetValues(typeof(T));
            return (T)enumvaluesAsArray.GetValue(randomGen.Next(enumvaluesAsArray.Length));
        }

        /// <summary>
        /// Creates a List from an array
        /// </summary>
        /// <typeparam name="T">Object type of the Array</typeparam>
        /// <param name="aSource">Array you want a list of</param>
        /// <returns></returns>
        public static List<T> CreateList<T>(T[] aSource)
        {
            if (aSource == null || aSource.Length == 0)
                return null;
            return new List<T>(aSource);
        }

        /// <summary>
        /// Copies a list, if the list is not empty it adds the elements to the end of the list.
        /// </summary>
        /// <typeparam name="T">List type</typeparam>
        /// <param name="aDest">list to add to</param>
        /// <param name="aSource">list to copy from</param>
        /// <param name="aIndex">Element of the list to copy starting from 0</param>
        public static void Copy<T>(ref List<T> aDest, List<T> aSource, int aIndex)
        {
            if (aSource == null)
                return;
            if (aDest == null)
                aDest = new List<T>();
            aDest.Add(aSource[aIndex]);
        }

        /// <summary>
        /// Get a rotation in degree between two points
        /// </summary>
        /// <param name="normalizedDirectionBetweentPoints">normalized direction from one point to another</param>
        /// <returns>Angle from one point to another in degree</returns>
        public static float GetAngleFromVector(Vector2 normalizedDirectionBetweentPoints)
        {
            return Mathf.Atan2(
                normalizedDirectionBetweentPoints.y,
                normalizedDirectionBetweentPoints.x) * Mathf.Rad2Deg;
        }

        // sound
        // maybe to do https://johnleonardfrench.com/articles/how-to-fade-audio-in-unity-i-tested-every-method-this-ones-the-best/

        /// <summary>
        /// returns true uf point is on the left side and false if point is on the right side of the plane
        /// thanks to Harald Hanche-Olsen https://math.stackexchange.com/a/214194
        /// </summary>
        /// <param name="planeA">plane point A</param>
        /// <param name="planeB">plane point B</param>
        /// <param name="planeC">plane point C</param>
        /// <param name="point">Point to compare</param>
        /// <returns></returns>
        public static bool GetSide(Vector3 planeA, Vector3 planeB, Vector3 planeC, Vector3 point)
        {
            bool result;
            Vector3 BA = planeB - planeA;
            Vector3 CA = planeC - planeA;
            Vector3 PA = point - planeA;

            float determinant3x3 = BA.x * CA.y * PA.z + BA.y * CA.z * PA.x + BA.z * CA.x * PA.y
                - BA.z * CA.y * PA.x - BA.y * CA.x * PA.z - BA.x * CA.z * PA.y;
            if (determinant3x3 > 0f)
            {
                result = true;
            }
            else
            {
                result = false;
            }
			
            return result;
        }

		private static float _currentHueValue = (float) randomGen.NextDouble();
		private static float _goldenRatioConjugate = 0.618033988749895f;
		/// <summary>
		/// Aesthetically pleasing randomness.
		/// The values are very evenly distributed, regardless how many values are used.(Golden Ratio to the rescue)
		/// Thanks to Martin Ankerl https://martin.ankerl.com/2009/12/09/how-to-create-random-colors-programmatically/
		/// </summary>
		/// <param name="saturation">saturation from 0 to 1, 1 is fully saturated</param>
		/// <param name="value">value from 0 to 1, 1 is white 0 is black (if saturation is at 0, that is)</param>
		/// <returns>RGB value generated from HSV space</returns>
		public static Color GetRandomColor(float saturation, float value)
		{
			_currentHueValue += _goldenRatioConjugate;
			float hueValue = _currentHueValue % 1;
			return Color.HSVToRGB(hueValue, saturation, value);
		}
    }

    /// <summary>
    /// The casts to object in the below code are an unfortunate necessity due to
    /// C#'s restriction against a where T : Enum constraint. (There are ways around
    /// this, but they're outside the scope of this simple illustration.)
    /// Thanks to Dan Tao (https://stackoverflow.com/questions/3261451/using-a-bitmask-in-c-sharp)
    /// </summary>
    public static class FlagsHelper
    {
        /// <summary>
        /// Check if a specific bit is set
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="flags">The Bitstring you want to check</param>
        /// <param name="flag">The Bit you want to be checked</param>
        /// <returns></returns>
        public static bool IsSet<T>(T flags, T flag) where T : struct
        {
            int flagsValue = (int)(object)flags;
            int flagValue = (int)(object)flag;

            return (flagsValue & flagValue) != 0;
        }

        /// <summary>
        /// Set a specific bit
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="flags">The Bitstring you want to set the bit</param>
        /// <param name="flag">The Bit you want to be set</param>
        public static void Set<T>(ref T flags, T flag) where T : struct
        {
            int flagsValue = (int)(object)flags;
            int flagValue = (int)(object)flag;

            flags = (T)(object)(flagsValue | flagValue);
        }

        /// <summary>
        /// Unset a specific bit
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="flags">The Bitstring you want to unset the bit</param>
        /// <param name="flag">The Bit you want to be unset</param>
        public static void Unset<T>(ref T flags, T flag) where T : struct
        {
            int flagsValue = (int)(object)flags;
            int flagValue = (int)(object)flag;

            flags = (T)(object)(flagsValue & (~flagValue));
        }
    }
}