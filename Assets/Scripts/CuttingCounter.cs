using Unity.VisualScripting;
using UnityEngine;

public class CuttingCounter : BaseCounter
{
    [SerializeField] private KitchenObjectSO cutKitchenObjectSO;

    public override void Interact(Player player)
    {
        if(!HasKitchenObject())
        {
            // There is no KitchenObject here
            if(player.HasKitchenObject())
            {
                // Player is carring something
                player.GetKitchenObject().SetKitchenObjectParent(this);
            } else
            {
                // Player not carrying anything
            }
        } else
        {
            // There is KitchenObject here
            if(player.HasKitchenObject())
            {
                // Player is carrying something
            } else
            {
                // Player is not carrying anything
                GetKitchenObject().SetKitchenObjectParent(player);
            }
        }
    }

    public override void InteractAlternate(Player player)
    {
        if(HasKitchenObject())
        {
            // There is a KitchenObject here
            GetKitchenObject().DestroySelf();

            KitchenObject.SpawnKitchenObject(cutKitchenObjectSO, this);
        }
    }
}
