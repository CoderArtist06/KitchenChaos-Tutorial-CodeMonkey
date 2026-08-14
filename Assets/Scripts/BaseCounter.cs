using UnityEngine;

public class BaseCounter : MonoBehaviour
{
    
    public virtual void Interact(Player player) // invece di virtual, si può rendere abstract
    {
        Debug.LogError("BaseCounter.Interact();");
    }
}
