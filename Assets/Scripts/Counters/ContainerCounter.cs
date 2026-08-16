using System;
using UnityEngine;

public class ContainerCounter : BaseCounter
{

    public event EventHandler onPlayerGrabbedObject;

    [SerializeField] private KitchenObjectSO kitchenObjectSO;

    public override void Interact(Player player)
    {
        //Debug.Log("Interact");
        if (!player.HasKitchenObject())
        {
            // Player is not carrying anything
            KitchenObject.SpawnKitchenObject(kitchenObjectSO, player);

            onPlayerGrabbedObject?.Invoke(this, EventArgs.Empty);
        }
    }

}
