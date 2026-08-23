using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameInput : MonoBehaviour
{
    public static GameInput Instance { get; private set; }

    // EVENTO: si attiva quando il tasto "interagisci" viene premuto.
    // Player.cs si iscrive a questo nello Start() con:
    //   gameInput.OnInteractAction += GameInput_OnInteractAction;
    // Uso EventHandler "semplice" (non generico) perché qui non servono dati extra da passare
    public event EventHandler OnInteractAction;
    public event EventHandler OnInteractAlternateAction;
    public event EventHandler OnPauseAction;

    // Riferimento alle Input Actions generate dal New Input System
    private PlayerInputActions playerInputActions;

    private void Awake()
    {
        Instance = this;

        playerInputActions = new PlayerInputActions();
        playerInputActions.Player.Enable(); // attiva la action map "Player" (serve altrimenti gli input non vengono letti)

        // ISCRIZIONE all'evento nativo del New Input System: "performed" scatta
        // quando l'azione Interact viene eseguita (es. tasto premuto una volta)
        playerInputActions.Player.Interact.performed += InteractPerformed;
        playerInputActions.Player.InteractAlternate.performed += InteractAlternatePerformed;
        playerInputActions.Player.Pause.performed += PausePerformed;
    }

    private void PausePerformed(InputAction.CallbackContext obj)
    {
        OnPauseAction?.Invoke(this, EventArgs.Empty);
    }

    private void InteractAlternatePerformed(InputAction.CallbackContext obj)
    {
        OnInteractAlternateAction?.Invoke(this, EventArgs.Empty);
    }

    // Handler chiamato dal New Input System quando Interact scatta.
    // Da qui NON gestiamo direttamente la logica di interazione (quella sta in Player.cs):
    // ci limitiamo a "rilanciare" l'informazione tramite il nostro evento OnInteractAction.
    // Questo è il punto chiave del disaccoppiamento: GameInput sa leggere l'input,
    // ma non sa (e non deve sapere) cosa fare quando l'utente interagisce
    private void InteractPerformed(InputAction.CallbackContext obj)
    {
        OnInteractAction?.Invoke(this, EventArgs.Empty); // Se OnInteractAction ha almeno un iscritto, lo invoca
    }

    // Legge il vettore di movimento dal New Input System (WASD / stick analogico)
    // e lo normalizza, così muoversi in diagonale non è più veloce che muoversi dritto
    public Vector2 GetMovementVectorNormalized()
    {
        // Current input value (x = left/right, y = foward/backward)
        Vector2 inputVector = playerInputActions.Player.Move.ReadValue<Vector2>();

        inputVector = inputVector.normalized;

        return inputVector;
    }
}
