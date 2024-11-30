using UnityEngine;

[RequireComponent (typeof(Animator))]
public class AnimationControl : MonoBehaviour
{
    [Space]
    [Header("Attributes names in animator")]
    [SerializeField] string animatorMoveX;
    [SerializeField] string animatorMoveY;

    [Header("Idle Animations")]
    [SerializeField] string animatorIdle;
    [SerializeField] string animatorIdleX;
    [SerializeField] string animatorIdleY;

    private Animator animator;
    private Rigidbody2D rbCharacter;
    private Vector2 lastMoveDirection = Vector2.down;

    private void Start()
    {
        animator = GetComponent<Animator>();
        rbCharacter = GetComponent<Rigidbody2D>();
    }

    private void FixedUpdate()
    {
        CheckDirection();
    }

    public void CheckDirection()
    {
        if (!animator) return;

        Vector2 moveDirection = rbCharacter.velocity;

       //Set variables for character animator
        animator.SetBool (animatorIdle,  rbCharacter.velocity.sqrMagnitude != 0 ? false : true);
        animator.SetFloat(animatorMoveX, rbCharacter.velocity.x);
        animator.SetFloat(animatorMoveY, rbCharacter.velocity.y);

        //Calculate last direction for characater idle
        if (rbCharacter.velocity.x != 0 && rbCharacter.velocity.y == 0)
        {
            lastMoveDirection = rbCharacter.velocity.x > 0 ? Vector2.right : Vector2.left;
        }
        else if (rbCharacter.velocity.y != 0 && rbCharacter.velocity.x == 0)
        {
            lastMoveDirection = rbCharacter.velocity.y > 0 ? Vector2.up : Vector2.down;
        }
        else if (rbCharacter.velocity.x != 0)
        {
            lastMoveDirection = rbCharacter.velocity.x > 0 ? Vector2.right : Vector2.left;
        }
        else if (rbCharacter.velocity.y != 0)
        {
            lastMoveDirection = rbCharacter.velocity.y > 0 ? Vector2.up : Vector2.down;
        }

        if (rbCharacter.velocity.sqrMagnitude == 0)
        {
            animator.SetFloat(animatorIdleX, lastMoveDirection.x);
            animator.SetFloat(animatorIdleY, lastMoveDirection.y);
        }
    }
}
