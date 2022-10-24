using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Manages the shop stalls in the actual game environment - the protection disappears when purchasing
/// </summary>
public class ShopStall : Interactible
{
    [SerializeField] int cost;
    [SerializeField] string item;
    [SerializeField] AudioSource purchaseSound;
    [SerializeField] AudioSource rejectSound;
    bool canAfford = false;

    private void Awake()
    {
        if(CharacterStatisticsController.credits >= cost)
        {
            toolTip = "<color=#79FF00FF>Purchase " + item + " for " + cost.ToString() + "?</color>";
            canAfford = true;
            return;
        }
        toolTip = "<color=#FF0000FF>Cannot afford " + cost + " credits!</color>";
    }

    private void Update()
    {
        if (CharacterStatisticsController.credits >= cost)
        {
            toolTip = "<color=#79FF00FF>Purchase " + item + " for " + cost.ToString() + "?</color>";
            canAfford = true;
            return;
        }
        toolTip = "<color=#FF0000FF>Cannot afford " + cost + " credits!</color>";
    }

    public override void Interact()
    {
        if(canAfford)
        {
            purchaseSound.Play();
            CharacterStatisticsController.credits -= cost;

            Destroy(gameObject);
            return;
        }
        rejectSound.Play();
    }
}
