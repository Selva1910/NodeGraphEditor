using UnityEngine;
using System.Collections.Generic;
using System.Collections;

namespace NodeGraph
{
    public class RotatableObject : MonoBehaviour, IRotatableObject
    {
        private bool isRotated = false;
        public bool IsRotated { get; }
        public Vector3 TargetRotation { get; set; }
        public bool IsLocalSpace { get; set; }
        public float Duration { get; set; }
        public void Perform()
        {
            StartCoroutine(Rotate());
        }
        IEnumerator Rotate()
        {
            float elapsedTime = 0f;
            Vector3 startPos = IsLocalSpace ? transform.localRotation.eulerAngles : transform.rotation.eulerAngles;
            Vector3 endPos = TargetRotation;

            while (elapsedTime < Duration)
            {
                float t = elapsedTime / Duration;
                t = Mathf.SmoothStep(0, 1, t); // ease in-out

                Vector3 newPos = Vector3.Lerp(startPos, endPos, t);
                if (IsLocalSpace)
                    transform.localRotation = Quaternion.Euler(newPos);
                else
                    transform.rotation = Quaternion.Euler(newPos);

                elapsedTime += Time.deltaTime;
                yield return null;
            }

            // Final correction
            if (IsLocalSpace)
                transform.localRotation = Quaternion.Euler(endPos);
            else
                transform.rotation = Quaternion.Euler(endPos);

            isRotated = true;
            Debug.Log("Rotate complete!");
            
        }
    }
}