using UnityEngine;

public class AiCombatController : MonoBehaviour
{
    private CombatScript playerCombat;
    private int regularAttackCount = 0;
    private int lightningKicksCount = 0;

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
        else if (playerCombat.hitCounterLightningKicks >= 45)
        {
            // Perform Lightning Kicks
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

        // Increment the regular attack count
        regularAttackCount++;

        // Check if it's time to trigger Kamehameha after 15 regular attacks
        if (regularAttackCount >= 15)
        {
            // Perform Kamehameha
            Kamehameha();

            // Reset the regular attack count
            regularAttackCount = 0;
        }

        // Check if it's time to trigger Sephiroth after 25 regular attacks
        if (regularAttackCount >= 25)
        {
            // Perform Sephiroth
            Sephiroth();

            // Reset the regular attack count
            regularAttackCount = 0;
        }
    }

    private void SpinningBirdKick()
    {
        // Call the player's Spinning Bird Kick function
        playerCombat.SpinningBirdKick();
    }

    private void LightningKicks()
    {
        // Call the player's Lightning Kicks function
        playerCombat.LightningKicks();
    }

    private void Kamehameha()
    {
        // Call the player's Kamehameha function
        playerCombat.Kamehameha();
    }

    private void Sephiroth()
    {
        // Call the player's Sephiroth function
        playerCombat.Sephiroth();
    }
}
