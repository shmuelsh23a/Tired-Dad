using UnityEngine;

namespace TiredDad
{
    /// <summary>
    /// Moves the father via a CharacterController. Forward motion is gated by the
    /// tap-tempo from TouchInputController; horizontal swipes rotate 90°.
    /// Replaces the movement half of the old Walker.cs.
    /// </summary>
    [RequireComponent(typeof(CharacterController))]
    public class PlayerMover : MonoBehaviour
    {
        [Header("References")]
        public TouchInputController input;

        [Header("Movement")]
        public float moveSpeed = 2.5f;      // metres/sec at full tempo
        public float gravity = 9.81f;

        private CharacterController _cc;
        private float _verticalVel;

        /// <summary>True when the father is actually moving this frame.</summary>
        public bool IsWalking { get; private set; }
        /// <summary>Normalised walk tempo 0..1, forwarded from input for stat scaling.</summary>
        public float Tempo01 => input != null ? input.Tempo01 : 0f;

        void Awake()
        {
            _cc = GetComponent<CharacterController>();
            if (input == null) input = GetComponent<TouchInputController>();
        }

        void Update()
        {
            if (input == null) { IsWalking = false; return; }

            // Turning: apply a queued 90° swipe.
            if (input.TurnRequest != 0)
                transform.rotation *= Quaternion.Euler(0f, 90f * input.TurnRequest, 0f);

            // Forward motion scales with tempo.
            Vector3 move = Vector3.zero;
            IsWalking = input.Tempo > 0;
            if (IsWalking)
                move = transform.forward * (moveSpeed * input.Tempo01);

            // Simple gravity so the controller stays grounded.
            if (_cc.isGrounded) _verticalVel = -1f;
            else _verticalVel -= gravity * Time.deltaTime;
            move.y = _verticalVel;

            _cc.Move(move * Time.deltaTime);
        }
    }
}
