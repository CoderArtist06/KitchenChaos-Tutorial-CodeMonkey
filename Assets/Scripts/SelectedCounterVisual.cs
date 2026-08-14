using System;
using UnityEngine;

// Script che sta su OGNI singolo ClearCounter nella scena (uno per counter).
// Il suo unico scopo: mostrare/nascondere un indicatore visivo (es. bordo giallo)
// quando IL SUO counter specifico è quello selezionato dal player
public class SelectedCounterVisual : MonoBehaviour
{
    // Riferimento al counter "proprietario" di questo visual, serve per confrontarlo
    // con il counter selezionato che arriva dall'evento
    [SerializeField] private BaseCounter baseCounter;
    // Il GameObject grafico da accendere/spegnere (es. un outline/highlight)
    [SerializeField] private GameObject[] visualGameObjectArray;

    private void Start()
    {
        // ISCRIZIONE all'evento di Player: da qui in poi ogni volta che il player
        // cambia counter selezionato (in QUALSIASI punto della scena), questo script
        // viene notificato e decide se riguarda LUI o no
        Player.Instance.onSelectedCounterChanged += Player_OnSelectedCounterChanged;
    }

    // Handler: riceve il counter appena selezionato (dentro e.selectedCounter)
    // e lo confronta con il proprio. Solo se coincidono si accende il visual
    private void Player_OnSelectedCounterChanged(object sender, Player.OnSelectedCounterChangedEventArgs e)
    {
        if (e.selectedCounter == baseCounter)
        {
            Show();
        } else
        {
            Hide();
        }
    }

    private void Show()
    {
        foreach(GameObject visualGameObject in visualGameObjectArray)
        {
            visualGameObject.SetActive(true);
        }
    }

    private void Hide()
    {
        foreach(GameObject visualGameObject in visualGameObjectArray)
        {
            visualGameObject.SetActive(false);
        }
    }
}
