using UnityEngine;

public class BaseCounter : MonoBehaviour, IKitchenObjectParent
{
    [SerializeField] private Transform counterTopPoint; // Posso utilizzare sia il tipo Trasform che GameObject per ricevere l'oggetto Prefab

    private KitchenObject kitchenObject;
    
    public virtual void Interact(Player player) // invece di virtual, si può rendere abstract
    {
        Debug.LogError("BaseCounter.Interact();");
    }

    public Transform GetKitchenObjectFollowTransform()
    {
        return counterTopPoint;
    }

    public void SetKitchenObject(KitchenObject kitchenObject)
    {
        this.kitchenObject = kitchenObject;
    }

    public KitchenObject GetKitchenObject()
    {
        return kitchenObject;
    }

    public void ClearKitchenObject()
    {
        kitchenObject = null;
    }

    public bool HasKitchenObject()
    {
        return kitchenObject != null;
    }
}
