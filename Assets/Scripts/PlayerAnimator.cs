using UnityEngine;

// Collega lo stato logico del Player (si sta muovendo o no) all'Animator visivo.
// Nessuna logica di gioco qui dentro: è puramente un "ponte" tra dati e animazione
public class PlayerAnimator : MonoBehaviour
{
    // Nome del parametro Bool nell'Animator Controller. Usare una costante invece
    // di scrivere la stringa "IsWalking" ogni volta evita errori di battitura
    // e rende più facile rinominare il parametro in un solo punto
    private const string IS_WALKING = "IsWalking";
    
    private Animator animator;

    // Riferimento al Player per leggere il suo stato (IsWalking())
    [SerializeField] private Player player;
    
    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        // Ogni frame, sincronizza il parametro Bool dell'Animator con lo stato reale del player.
        // L'Animator Controller userà questo Bool per decidere quando passare
        // dall'animazione "idle" a quella "walk" (transizione gestita visivamente nell'Animator, non qui)
        animator.SetBool(IS_WALKING, player.IsWalking());
    }
}
