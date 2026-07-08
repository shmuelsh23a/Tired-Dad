using UnityEngine;

namespace TiredDad
{
    /// <summary>
    /// Attached to the changing table by RoomLayout. When the father reaches it and
    /// the baby needs a diaper change, it clears the Diaper need (and eases guilt).
    /// Replaces the commented-out OnControllerColliderHit logic in the old Walker.cs.
    /// </summary>
    [RequireComponent(typeof(Collider))]
    public class ChangingTableTrigger : MonoBehaviour
    {
        public float guiltRelief = 10f;

        void OnTriggerEnter(Collider other)
        {
            if (other.GetComponentInParent<PlayerMover>() == null) return;
            var gm = GameManager.Instance;
            if (gm == null || gm.needsSystem == null) return;
            if (!gm.needsSystem.NeedDiaper) return;

            gm.needsSystem.ClearNeed(BabyNeed.Diaper);
            gm.stats.Apply(0, 0, 0, 0, -guiltRelief);
        }
    }
}
