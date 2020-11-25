using Assets.Scripts.Helpers;
using UnityEngine;

namespace Assets.Scripts.Player
{
    public class Collision : Singleton<Collision>
    {
        [Header("Layers")]
        public LayerMask groundLayer;

        [Space]
        public bool onGround;
        public bool onWall;
        public bool onRightWall;
        public bool onLeftWall;
        public int wallSide;

        [Header("Collision")]
        public float collisionRadius = 0.25f;
        public Vector2 bottomOffset, rightOffset, leftOffset;

        private void Update()
        {  
            onGround = Physics2D.OverlapCircle((Vector2)transform.position + bottomOffset, collisionRadius, groundLayer);
            onWall = Physics2D.OverlapCircle((Vector2)transform.position + rightOffset, collisionRadius, groundLayer) 
                     || Physics2D.OverlapCircle((Vector2)transform.position + leftOffset, collisionRadius, groundLayer);

            onRightWall = Physics2D.OverlapCircle((Vector2)transform.position + rightOffset, collisionRadius, groundLayer);
            onLeftWall = Physics2D.OverlapCircle((Vector2)transform.position + leftOffset, collisionRadius, groundLayer);

            wallSide = onRightWall ? -1 : 1;
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;

            Gizmos.DrawWireSphere((Vector2)transform.position  + bottomOffset, collisionRadius);
            Gizmos.DrawWireSphere((Vector2)transform.position + rightOffset, collisionRadius);
            Gizmos.DrawWireSphere((Vector2)transform.position + leftOffset, collisionRadius);
        }
    }
}
