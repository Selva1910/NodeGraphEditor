using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace NodeGraph
{
    public class MovableObject : MonoBehaviour, IMovableObject
    {
        private bool isMoved;

        public bool IsMoved => isMoved;

        public Vector3 TargetPosition { get; set; }

        public float Duration {get; set;}
        
        public bool IsLocalSpace {get; set;}
       
        private void Update()
        {
            if (Vector3.Distance(transform.position, TargetPosition) <= 0.2f)
                isMoved = true;
        }

        
        public void StartMove()
        {
            StartCoroutine(MoveUntil());
        }

        IEnumerator MoveUntil()
        {
            float elapsedTime = 0f;
            Vector3 startPos = IsLocalSpace ? transform.localPosition : transform.position;
            Vector3 endPos = TargetPosition;

       

            while (elapsedTime < Duration)
            {
                float t = elapsedTime / Duration;
                t = Mathf.SmoothStep(0, 1, t); // ease in-out

                Vector3 newPos = Vector3.Lerp(startPos, endPos, t);
                if (IsLocalSpace)
                    transform.localPosition = newPos;
                else
                    transform.position = newPos;

                elapsedTime += Time.deltaTime;
                yield return null;
            }

            // Final correction
            if (IsLocalSpace)
                transform.localPosition = endPos;
            else
                transform.position = endPos;

            isMoved = true;
            Debug.Log("Move complete!");
            
        }
    }
}
