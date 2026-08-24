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

    public enum Binding
    {
        Move_Up,
        Move_Down,
        Move_Left,
        Move_Right,
        Interact,
        InteractAlternate,
        Pause
    }

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

        //Debug.Log(GetBindingText(Binding.Interact));
    }

    private void OnDestroy()
    {
        playerInputActions.Player.Interact.performed -= InteractPerformed;
        playerInputActions.Player.InteractAlternate.performed -= InteractAlternatePerformed;
        playerInputActions.Player.Pause.performed -= PausePerformed;

        playerInputActions.Dispose();
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

    public string GetBindingText(Binding binding)
    {
        switch(binding)
        {
            default:
            case Binding.Move_Up:
                return playerInputActions.Player.Move.bindings[1].ToDisplayString();
            case Binding.Move_Down:
                return playerInputActions.Player.Move.bindings[2].ToDisplayString();
            case Binding.Move_Left:
                return playerInputActions.Player.Move.bindings[3].ToDisplayString();
            case Binding.Move_Right:
                return playerInputActions.Player.Move.bindings[4].ToDisplayString();
            case Binding.Interact:
                return playerInputActions.Player.Interact.bindings[0].ToDisplayString();
            case Binding.InteractAlternate:
                return playerInputActions.Player.InteractAlternate.bindings[0].ToDisplayString();
            case Binding.Pause:
                return playerInputActions.Player.Pause.bindings[0].ToDisplayString();
        }
    }

    public void RebindBinding(Binding binding, Action onActionRebound)
    {
        playerInputActions.Player.Disable();
        
        InputAction inputAction;
        int bindingIndex;
        

        switch(binding)
        {
            case Binding.Move_Up:
                playerInputActions.Player.Move.PerformInteractiveRebinding(1).OnComplete(callback =>
                {
                    //Debug.Log(callback.action.bindings[1].path);
                    //Debug.Log(callback.action.bindings[1].overridePath);
                    callback.Dispose();

                    playerInputActions.Player.Enable();
                    onActionRebound();
                }).Start();
                break;
            case Binding.Move_Down:
                playerInputActions.Player.Move.PerformInteractiveRebinding(2).OnComplete(callback =>
                {
                    callback.Dispose();

                    playerInputActions.Player.Enable();
                    onActionRebound();
                }).Start();
                break;
            case Binding.Move_Left:
                playerInputActions.Player.Move.PerformInteractiveRebinding(3).OnComplete(callback =>
                {
                    callback.Dispose();

                    playerInputActions.Player.Enable();
                    onActionRebound();
                }).Start();
                break;
            case Binding.Move_Right:
                playerInputActions.Player.Move.PerformInteractiveRebinding(4).OnComplete(callback =>
                {
                    callback.Dispose();

                    playerInputActions.Player.Enable();
                    onActionRebound();
                }).Start();
                break;
            case Binding.Interact:
                playerInputActions.Player.Interact.PerformInteractiveRebinding(0).OnComplete(callback =>
                {
                    callback.Dispose();

                    playerInputActions.Player.Enable();
                    onActionRebound();
                }).Start();
                break;
            case Binding.InteractAlternate:
                playerInputActions.Player.InteractAlternate.PerformInteractiveRebinding(0).OnComplete(callback =>
                {
                    callback.Dispose();

                    playerInputActions.Player.Enable();
                    onActionRebound();
                }).Start();
                break;
            case Binding.Pause:
                playerInputActions.Player.Pause.PerformInteractiveRebinding(0).OnComplete(callback =>
                {
                    callback.Dispose();

                    playerInputActions.Player.Enable();
                    onActionRebound();
                }).Start();
                break;
        }
    }
}
