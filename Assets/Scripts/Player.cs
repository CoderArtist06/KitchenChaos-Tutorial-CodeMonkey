using System;
using UnityEngine;

public class Player : MonoBehaviour, IKitchenObjectParent
{
    // Singleton: permette ad altri script di accedere al Player con Player.Instance
    // invece di fare FindObjectOfType o passare riferimenti manualmente
    public static Player Instance { get; private set; }

    [SerializeField] private GameInput gameInput;

    // EVENTO: si "attiva" ogni volta che cambia il counter selezionato dal player.
    // Chi vuole reagire (es. UI che mostra un'icona sopra il counter) si iscrive con +=
    public event EventHandler<OnSelectedCounterChangedEventArgs> onSelectedCounterChanged;
    
    // Classe custom per portare dati extra insieme all'evento (qui: quale counter)
    public class OnSelectedCounterChangedEventArgs : EventArgs
    {
        public BaseCounter selectedCounter { get; set; }
    }
    
    // Movement tuning (editabile da inspector)
    [SerializeField] private float moveSpeed = 7f;
    [SerializeField] private float rotateSpeed = 10f;
    [SerializeField] private float interactDistance = 2f;
    [SerializeField] private LayerMask countersLayerMask; // filtra il raycast: colpisce solo i counter, non tutto
    [SerializeField] private Transform kitchenObjectHoldPoint; // Posso utilizzare sia il tipo Trasform che GameObject per ricevere l'oggetto Prefab

    private bool isWalking;
    private Vector3 lastInteractDir;      // memorizza l'ultima direzione valida, così puoi interagire anche da fermo guardando quella direzione
    private BaseCounter selectedCounter; // counter attualmente "puntato" dal player
    private KitchenObject kitchenObject;

    private void Awake()
    {
        // Guardia anti-duplicati per il singleton: se un altro Player esiste già, è un errore di setup
        if (Instance != null)
        {
            Debug.LogError("There is more than one Player instance");
        }
        Instance = this;
    }

    private void Start()
    {
        // ISCRIZIONE a un evento esterno: quando GameInput rileva il tasto "interagisci",
        // viene chiamato automaticamente GameInput_OnInteractAction qui sotto
        gameInput.OnInteractAction += GameInput_OnInteractAction;
        gameInput.OnInteractAlternateAction += GameInput_OnInteractAlternateAction;
    }

    // Handler chiamato dall'evento di GameInput. La firma (object sender, EventArgs e)
    // è lo standard C# per gli eventi, anche se qui non usiamo sender/e
    private void GameInput_OnInteractAction(object sender, System.EventArgs e)
    {
        if (selectedCounter != null)
        {
            selectedCounter.Interact(this);
        }
    }

    private void GameInput_OnInteractAlternateAction(object sender, System.EventArgs e)
    {
        if (selectedCounter != null)
        {
            selectedCounter.InteractAlternate(this);
        }
    }

    private void Update()
    {
        //OldInputSystem(); // vecchio sistema di input sostituito da gameInput (New Input System). Tenuto commentato come riferimento.
        HandleMovement();
        HandleInteractions();
    }

    // Vecchio input diretto con Input.GetKey, utile da tenere come promemoria di come si faceva, buona da utilizzare per prototipazione
    // "alla vecchia maniera" prima di passare al New Input System / classe GameInput dedicata
    /* private void OldInputSystem()
    {
        // Current input value (x = left/right, y = foward/backward)
        Vector2 inputVector = new Vector2(0, 0);

        if (Input.GetKey(KeyCode.W))
        {
            inputVector.y = +1;
        }
        if (Input.GetKey(KeyCode.S))
        {
            inputVector.y = -1;
        }
        if (Input.GetKey(KeyCode.A))
        {
            inputVector.x = -1;
        }
        if (Input.GetKey(KeyCode.D))
        {
            inputVector.x = +1;
        }

        inputVector = inputVector.normalized;

        Vector3 moveDir = new Vector3(inputVector.x, 0f, inputVector.y);
        transform.position += moveDir * moveSpeed * Time.deltaTime;

        isWalking = moveDir != Vector3.zero;

        transform.forward = Vector3.Slerp(transform.forward, moveDir, Time.deltaTime * rotateSpeed);
    } */

    public bool IsWalking()
    {
        return isWalking;
    }

    // Raycast in avanti per rilevare quale ClearCounter il player sta "puntando",
    // e notifica il cambiamento tramite l'evento onSelectedCounterChanged
    private void HandleInteractions()
    {
        Vector2 inputVector = gameInput.GetMovementVectorNormalized();
        Vector3 moveDir = new Vector3(inputVector.x, 0f, inputVector.y);

        // Se il player si sta muovendo, aggiorna la direzione di interazione.
        // Se è fermo, mantiene l'ultima direzione valida (altrimenti da fermo non potrebbe mai interagire)
        if (moveDir != Vector3.zero)
        {
            lastInteractDir = moveDir;
        }

        if (Physics.Raycast(transform.position, lastInteractDir, out RaycastHit raycastHit,interactDistance, countersLayerMask) == true)
        {
            if (raycastHit.transform.TryGetComponent(out BaseCounter baseCounter) == true)
            {
                // Il raycast ha colpito un ClearCounter
                //clearCounter.Interact(); // vecchia chiamata diretta, sostituita dal sistema evento-based tramite GameInput_OnInteractAction
                if (baseCounter != selectedCounter)
                {
                    selectedCounter = baseCounter;
                    //Debug.Log(selectedCounter);

                    SetSelectedCounter(selectedCounter);
                }
            } else
            {
                // Ha colpito qualcosa ma non è un ClearCounter -> deseleziona
                selectedCounter = null;
                SetSelectedCounter(selectedCounter);
            }
        } else
        {
            // Raycast non ha colpito nulla -> deseleziona
            selectedCounter = null;
            SetSelectedCounter(selectedCounter);
        }
    }

    // Movimento con collision detection "a scalini": prova il movimento completo,
    // se bloccato prova solo asse X, poi solo asse Z. Questo permette di "scivolare"
    // lungo i muri invece di bloccarsi di colpo quando ci si muove in diagonale contro un ostacolo
    private void HandleMovement()
    {
        Vector2 inputVector = gameInput.GetMovementVectorNormalized();
        Vector3 moveDir = new Vector3(inputVector.x, 0f, inputVector.y);

        float moveDistance = moveSpeed * Time.deltaTime;
        float playerRadius = .7f;
        float playerHeight = 2f;

        // CapsuleCast: verifica se una capsula (forma approssimata del player) può muoversi
        // in moveDir per moveDistance senza colpire ostacoli
        bool canMove = !Physics.CapsuleCast(transform.position, transform.position + Vector3.up * playerHeight, playerRadius, moveDir, moveDistance);

        if (!canMove)
        {
            // Movimento pieno bloccato -> prova solo componente X
            Vector3 moveDirX = new Vector3(moveDir.x, 0, 0).normalized;
            canMove = moveDir.x != 0 && !Physics.CapsuleCast(transform.position, transform.position + Vector3.up * playerHeight, playerRadius, moveDirX, moveDistance);

            if (canMove)
            {
                // Si può muovere solo in X
                moveDir = moveDirX;
            } else
            {
                // Anche X bloccato -> prova solo componente Z
                Vector3 moveDirZ = new Vector3(0, 0, moveDir.z).normalized;
                canMove = moveDir.z != 0 && !Physics.CapsuleCast(transform.position, transform.position + Vector3.up * playerHeight, playerRadius, moveDirZ, moveDistance);

                if (canMove)
                {
                    // Si può muovere solo in Z
                    moveDir = moveDirZ;
                } else
                {
                    // Bloccato in ogni direzione -> non si muove
                }
            }
        }

        if (canMove)
        {
            transform.position += moveDir * moveSpeed * Time.deltaTime;
        }

        isWalking = moveDir != Vector3.zero;

        // Rotazione fluida verso la direzione di movimento (Slerp = interpolazione sferica)
        transform.forward = Vector3.Slerp(transform.forward, moveDir, Time.deltaTime * rotateSpeed);
    }

    // Aggiorna il counter selezionato e AVVISA chi è iscritto (es. UI) tramite l'evento.
    // Questo è il punto in cui la "campana" viene suonata (?.Invoke)
    private void SetSelectedCounter(BaseCounter selectedCounter)
    {
        this.selectedCounter = selectedCounter;

        onSelectedCounterChanged?.Invoke(this, new OnSelectedCounterChangedEventArgs 
        {
            selectedCounter = selectedCounter
        });
    }

    public Transform GetKitchenObjectFollowTransform()
    {
        return kitchenObjectHoldPoint;
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