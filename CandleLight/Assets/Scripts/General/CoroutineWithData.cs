/*
* Project: CandleLight 
* Author: Shahir Chowdhury
* Date: June 1, 2019
* 
* The CoroutineWithData class is used to create a coroutine that can return a value
* Original code from https://answers.unity.com/questions/24640/how-do-i-return-a-value-from-a-coroutine.html
*/

using System.Collections;
using UnityEngine;  

namespace AssetManagers {

    public class CoroutineWithData {
        
        public Coroutine coroutine { get; private set; }    /// <value> Coroutine to run </value>
        private IEnumerator target;                         /// <value> IEnumerator to iterate through </value>
        public object result;                               /// <value> Result to return </value>
        

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="owner"> Monobehaviour to start the coroutine </param>
        /// <param name="target"> IEnumerator to run </param>
        public CoroutineWithData(MonoBehaviour owner, IEnumerator target) {
            this.target = target;
            this.coroutine = owner.StartCoroutine(Run());
        }

        /// <summary>
        /// Iterate through the target IEnumerator
        /// </summary>
        /// <returns> Returns the yield return value of the coroutine </returns>
        private IEnumerator Run() {
            while(target.MoveNext()) {
                result = target.Current;
                yield return result;
            }
        }
    }
}