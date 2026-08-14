using System;
using UnityEngine;

public class ContainerCounter : BaseCounter
{

    public event EventHandler onPlayerGrabbedObject;

    [SerializeField] private KitchenObjectSO kitchenObjectSO;

    public override void Interact(Player player)
    {
        //Debug.Log("Interact");
        if (!HasKitchenObject())
        {
            Transform kitchenObjectTransform = Instantiate(kitchenObjectSO.prefab);
            kitchenObjectTransform.GetComponent<KitchenObject>().SetKitchenObjectParent(player);
            onPlayerGrabbedObject?.Invoke(this, EventArgs.Empty);
        }
    }

}
