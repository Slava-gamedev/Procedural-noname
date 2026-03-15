using UnityEngine;

namespace CharacterMovement
{
    public class TerrainAnalyzer : MonoBehaviour
    {
        [SerializeField] private LayerMask _groundLayer;
        [SerializeField] private float _footWidth;
        private float _maxRaycastDistance = 30f;
        private float _upwardOffset = 3f;

        public float GetGroundHeightAtPosition(Vector2 worldPosition)
        {
            RaycastHit2D hit = Physics2D.CircleCast(worldPosition + Vector2.up * _upwardOffset,
                _footWidth / 2, Vector2.down, _maxRaycastDistance, _groundLayer);

            if(hit.collider != null)
            {
                return hit.point.y;
            }

            return worldPosition.y;
        }

        public RaycastHit2D ProjectBodyOnTheGround(Vector2 origin, float bodyRadius)
        {
            RaycastHit2D hit = Physics2D.CircleCast(transform.position, bodyRadius, Vector2.down, _maxRaycastDistance, _groundLayer);

            return hit;
        }

    }
}
