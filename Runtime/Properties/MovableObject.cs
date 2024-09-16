using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NodeGraph
{
    public class MovableObject : MonoBehaviour, IMovableObject
    {
        private bool isMoved;

        public bool IsMoved => isMoved;

        public Vector3 TargetPosition { get; set; }

        public float duration = 3f;


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
            float startTime = Time.time;

            while (Time.time - startTime < duration)
            {
                Debug.Log("Remaining time: " + (duration - (Time.time - startTime)) + " seconds");

                yield return null;
            }

            Debug.Log("Time limit reached!");
            isMoved = true;
        }
    }
}
