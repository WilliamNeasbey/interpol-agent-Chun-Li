using UnityEngine;

public class AiCombatController : MonoBehaviour
{
    private CombatScript playerCombat;

    private void Start()
    {
        playerCombat = GetComponent<CombatScript>();
    }

    private void Update()
    {
        // Check if the AI has hit the required number of times for Spinning Bird Kick
        if (playerCombat.hitCount >= 20)
        {
            // Perform Spinning Bird Kick
            SpinningBirdKick();
        }
        else
        {
            // Continuously attack using left-click
            Attack();
        }
        if (playerCombat.hitCounterLightningKicks >= 45)
        {
            // Perform Spinning Bird Kick
            LightningKicks();
        }
        else
        {
            // Continuously attack using left-click
            Attack();
        }
    }

    private void Attack()
    {
        // Call the player's attack function
        playerCombat.AttackCheck();
    }

    private void SpinningBirdKick()
    {
        // Call the player's Spinning Bird Kick function
        playerCombat.SpinningBirdKick();
    }

    private void LightningKicks()
    {
        // Call the player's Spinning Bird Kick function
        playerCombat.LightningKicks();
    }
}
