using UnityEngine;

namespace CharacterMovement
{
    public class GroundDetector : MonoBehaviour
    {
        [SerializeField] private LayerMask _groundLayer;
        private float _maxRaycastDistance = 30f;
        private float _upwardOffset = 3f;

        public float GetGroundHeightAtPosition(Vector2 worldPosition)
        {
            RaycastHit2D hit = Physics2D.Raycast(worldPosition + Vector2.up * _upwardOffset,
                Vector2.down, _maxRaycastDistance, _groundLayer);

            if(hit.collider != null)
            {
                return hit.point.y;
            }

            return worldPosition.y;
        }

    }
}
