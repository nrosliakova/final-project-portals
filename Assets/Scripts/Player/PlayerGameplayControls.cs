using UnityEngine;
using Player;

public class PlayerGameplayControls : MonoBehaviour
{
    private PlayerMovement movement;
    private PlayerLook look;
    private PlayerReset playerReset;
    private PlayerPickup pickup;

    private void Awake()
    {
        movement = GetComponent<PlayerMovement>();
        look = GetComponent<PlayerLook>();
        playerReset = GetComponent<PlayerReset>();
        pickup = GetComponent<PlayerPickup>();
    }

    [ContextMenu("Disable Gameplay")]
    public void DisableGameplay()
    {
        if (pickup != null)
        {
            pickup.DropHeldObject();
            pickup.enabled = false;
        }

        if (movement != null) movement.enabled = false;
        if (playerReset != null) playerReset.enabled = false;
        if (look != null) look.enabled = false;
    }

    [ContextMenu("Enable Gameplay")]
    public void EnableGameplay()
    {
        if (movement != null) movement.enabled = true;
        if (playerReset != null) playerReset.enabled = true;
        if (pickup != null) pickup.enabled = true;
        if (look != null) look.enabled = true;
    }
}