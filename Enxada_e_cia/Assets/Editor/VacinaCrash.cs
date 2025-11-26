using UnityEngine;
using UnityEditor; // Necessário para mexer no Editor

[InitializeOnLoad] // Faz esse script rodar assim que o Unity abre
public class VacinaCrash
{
    // Construtor estático é chamado na hora que o Unity carrega
    static VacinaCrash()
    {
        // O valor padrão costuma ser 0 ou 5. 
        // Vamos mudar para 10000 (dez mil segundos)
        int valorAtual = EditorPrefs.GetInt("BusyProgressDelay", 0);

        if (valorAtual < 5000)
        {
            // Injeta a configuração direto no registro do Unity
            EditorPrefs.SetInt("BusyProgressDelay", 10000);
            Debug.Log("VACINA APLICADA: BusyProgressDelay alterado para 10000 para evitar crashes no Linux!");
        }
    }
}