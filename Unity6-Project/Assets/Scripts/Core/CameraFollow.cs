using UnityEngine;

namespace TiredDad
{
    /// <summary>
    /// Simple angled top-down camera that follows the father. Attach to the Camera
    /// (or let DemoBootstrap create it) and assign the target.
    /// </summary>
    public class CameraFollow : MonoBehaviour
    {
        public Transform target;
        public Vector3 offset = new Vector3(0f, 14f, -7f);
        public float smooth = 5f;

        void LateUpdate()
        {
            if (target == null) return;
            Vector3 desired = target.position + offset;
            transform.position = Vector3.Lerp(transform.position, desired, smooth * Time.deltaTime);
            transform.LookAt(target.position + Vector3.up * 0.5f);
        }
    }
}
