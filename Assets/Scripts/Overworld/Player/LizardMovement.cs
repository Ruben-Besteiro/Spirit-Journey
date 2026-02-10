using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LizardMovement : PlayerMovement
{
    //Vector3 gravityDirection = Vector3.down;
    [SerializeField] Transform modelTransform;

    /*private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (hit.gameObject.CompareTag("ScalableWall"))
        {
            isTouchingWallThisFrame = true;
            wallNormal = hit.normal;

            // Cambiamos la gravedad y la rotación del modelo
            gravityDirection = -wallNormal;
            transform.rotation = Quaternion.FromToRotation(transform.up, wallNormal) * transform.rotation;

            velocity = Vector3.zero;
        }
    }


    protected override void ApplyGravity()
    {
        base.ApplyGravity();
        // Aplicar gravedad en la dirección personalizada
        if (Mathf.Abs(transform.up.y) > .707f)
        {
            base.ApplyGravity();
        }
        else
        {
            velocity += gravityDirection * Mathf.Abs(gravity) * Time.deltaTime;
            controller.Move(velocity * Time.deltaTime);
        }
    }


    // No tocar por favor
    /*protected override void HandleMovement()
    {
        Vector3 camForward = cameraTransform.forward;
        Vector3 camRight = cameraTransform.right;
        Vector3 moveDir;
        Vector3 playerUp = transform.up;  // Perpendicular a la pared

        // Las paredes se tratan diferente de las rampas y los suelos
        bool isOnASlopeOrGround = Mathf.Abs(playerUp.y) > 0.25f;     // Cuanto más cercano a 1, menos inclinación

        if (isOnASlopeOrGround)
        {
            Vector3 wallForward = Vector3.ProjectOnPlane(camForward, playerUp).normalized;
            Vector3 wallRight = Vector3.ProjectOnPlane(camRight, playerUp).normalized;

            // Si miras perpendicular a la rampa, usar playerUp
            if (wallForward.sqrMagnitude < 0.1f)
            {
                wallForward = playerUp;
            }

            moveDir = wallForward * moveInput.y + wallRight * moveInput.x;
        }
        else
        {
            // Para moverse por las paredes es con WASD, mover la cámara no hace nada
            Vector3 camUp = cameraTransform.up;
            Vector3 wallRight = Vector3.ProjectOnPlane(camRight, playerUp).normalized;

            moveDir = camUp * moveInput.y + wallRight * moveInput.x;
        }

        controller.Move(moveDir * moveSpeed * Time.deltaTime);
        RotateTowardsMovement(moveDir);
    }

    protected override void RotateTowardsMovement(Vector3 direction)
    {
        if (direction.sqrMagnitude < 0.001f) return;

        Vector3 projectedDirection = Vector3.ProjectOnPlane(direction, transform.up);

        if (projectedDirection.sqrMagnitude < 0.001f) return;

        Quaternion lookRotation = Quaternion.LookRotation(projectedDirection, transform.up);
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, rotationSpeed * Time.deltaTime);
    }

    protected override void HandleJump()
    {
        if (jumpBufferTimer > 0f)
        {
                // Resetear gravedad a normal
                gravityDirection = Vector3.down;
                transform.rotation = Quaternion.Euler(Vector3.zero);

                velocity = wallNormal * moveSpeed * .5f; // Impulso horizontal
                velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity); // Impulso vertical
                animator.SetTrigger("Jump");
                jumpBufferTimer = 0f;
                coyoteTimer = 0f;
        }
    }

    // Esto sirve para chequear si hemos salido de la pared
    protected override void LateUpdate()
    {
        if (wasOnWallLastFrame && !isTouchingWallThisFrame)
        {
            if (wallExitCoroutine != null)
            {
                StopCoroutine(wallExitCoroutine);
            }
            // Aplicamos un retardo para evitar los falsos positivos
            wallExitCoroutine = StartCoroutine(CheckWallExit());
        }

        if (isTouchingWallThisFrame && wallExitCoroutine != null)
        {
            StopCoroutine(wallExitCoroutine);
            wallExitCoroutine = null;
        }
        wasOnWallLastFrame = isTouchingWallThisFrame;
        isTouchingWallThisFrame = false;
    }*/
}