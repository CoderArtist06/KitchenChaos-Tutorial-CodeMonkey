using UnityEngine;

[CreateAssetMenu()]
public class KitchenObjectSO : ScriptableObject
{
    
    public Transform prefab; // Possibile scriverlo in privato ed poi creare una funzione publica get, che rende più sicuro l'attributo
    public Sprite sprite; // Per icona dell'oggetto
    public string objectName;

}