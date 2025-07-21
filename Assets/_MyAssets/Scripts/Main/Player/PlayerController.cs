using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEngine.InputSystem.InputAction;

namespace DashSync.Main
{
    [RequireComponent(typeof(Rigidbody))]
    public class PlayerController : MonoBehaviour
    {
        private Rigidbody thisRigidbody;
        [SerializeField]
        private float angularSpeed;
        [SerializeField]
        private Transform pivot;
        private float angle;
        private bool isTouchEnabled = false;
        private Vector2 delta;
        private float radius;
        private float screenWidth;

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            thisRigidbody = GetComponent<Rigidbody>();
            radius = Vector3.Distance(transform.position, pivot.position);
            screenWidth = Screen.width;
        }

        private void FixedUpdate()
        {
            if (!isTouchEnabled)
                return;

            if(delta.x < screenWidth / 2)
                angle -= angularSpeed * Time.fixedDeltaTime;
            else
                angle += angularSpeed * Time.fixedDeltaTime;

            float rad = angle * Mathf.Deg2Rad;

            // Calculate new position around the pivot
            var xPos = radius * Mathf.Cos(rad);
            var yPos = radius * Mathf.Sin(rad);
            Vector3 newPos = pivot.position + new Vector3(xPos, yPos, 0);

            thisRigidbody.MovePosition(newPos);
            //Face normal towards pivot
            Vector3 dir = (newPos - pivot.position).normalized;
            Quaternion lookRotation = Quaternion.LookRotation(dir);
            thisRigidbody.MoveRotation(lookRotation);
        }

        public void OnTouchPosition(CallbackContext context)
        {
            if (isTouchEnabled)
                delta = context.ReadValue<Vector2>();
            else
                delta = Vector2.zero;
        }

        public void OnTouchPress(CallbackContext context)
        {
            isTouchEnabled = context.ReadValueAsButton();
        }
    }
}