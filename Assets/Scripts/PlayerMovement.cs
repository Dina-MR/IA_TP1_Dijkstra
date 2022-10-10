using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    // Caractéristiques d'un mouvement
    public float moveSpeed;
    public Rigidbody2D rb;
    private Vector2 velocity = Vector2.zero;

    // Types de mouvement & paramète our les diagonales
    float horizontalMovement;
    float verticalMovement;
    float movementLimiter = 0.7f;

    // Etat du joueur
    public bool isMoving = false;

    void FixedUpdate()
    {
        horizontalMovement = Input.GetAxis("Horizontal") * moveSpeed * Time.deltaTime;
        verticalMovement = Input.GetAxis("Vertical") * moveSpeed * Time.deltaTime;
        if (horizontalMovement != 0 && verticalMovement != 0) // Check for diagonal movement
        {
            horizontalMovement *= movementLimiter;
            verticalMovement *= movementLimiter;
        }
        MovePlayer();

        //float characterVelocity = Mathf.Abs(rb.velocity.x) + Mathf.Abs(rb.velocity.y);
    }

    void MovePlayer()
    {
        Vector2 targetVelocity = new Vector2(horizontalMovement, verticalMovement);
        rb.velocity = Vector2.SmoothDamp(rb.velocity, targetVelocity, ref velocity, .05f);
        // Etat du mouvement
        if (rb.velocity != Vector2.zero)
            isMoving = true;
        else
            isMoving = false;
    }
}
