using UnityEngine;
using System.Collections;

public class ClimaManager : MonoBehaviour
{
    public GameObject particulasChuva;
    public float duracaoChuva = 8f; 
    private bool estaChovendo = false;

    void Start()
    {
        if (particulasChuva != null) particulasChuva.SetActive(false);
    }

    public void AtivarChuva()
    {
        if (estaChovendo) return; 
        StartCoroutine(ProcessoChuva());
    }

    IEnumerator ProcessoChuva()
    {
        estaChovendo = true;
        Debug.Log("O CÉU SE ABRIU! CHUVA INICIADA!");

        if (particulasChuva != null) particulasChuva.SetActive(true);

        // --- A CORREÇÃO MÁGICA ---
        // Em vez de regar uma vez e esperar, vamos regar REPETIDAMENTE
        
        float tempoPassado = 0f;
        float intervaloDeChecagem = 0.5f; // Verifica a cada meio segundo

        while (tempoPassado < duracaoChuva)
        {
            RegarTodasAsPlantas(); // Rega quem estiver com sede AGORA
            yield return new WaitForSeconds(intervaloDeChecagem); // Espera um pouquinho
            tempoPassado += intervaloDeChecagem; // Conta o tempo
        }
        // -------------------------

        // Fim da chuva
        if (particulasChuva != null) particulasChuva.SetActive(false);
        estaChovendo = false;
        Debug.Log("A chuva parou.");
    }

    void RegarTodasAsPlantas()
    {
        // Otimização: Se quiser, pode mover o FindObjects para fora do loop, 
        // mas para esse tamanho de projeto, aqui está ok.
        Ladrilho[] todasPlantas = FindObjectsOfType<Ladrilho>();

        foreach (Ladrilho planta in todasPlantas)
        {
            // O Ladrilho só aceita a rega se estiver Sedento, 
            // então não tem problema chamar isso várias vezes.
            planta.ReceberChuva();
        }
    }
}