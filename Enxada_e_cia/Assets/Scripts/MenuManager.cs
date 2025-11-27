using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    [Header("UI")]
    public GameObject painelInstrucoes; // O Painel Novo
    public GameObject painelMenuPrincipal; // O Painel Antigo (Título + Botões)

    // ... (IniciarJogo e SairDoJogo continuam iguais) ...
    public void IniciarJogo()
    {
        SceneManager.LoadScene("CenaDaFazenda");
    }

    public void SairDoJogo()
    {
        Application.Quit();
    }

    // --- AQUI MUDA A LÓGICA ---

    public void AbrirInstrucoes()
    {
        // Mostra Instruções, Esconde Menu
        if(painelInstrucoes != null) painelInstrucoes.SetActive(true);
        if(painelMenuPrincipal != null) painelMenuPrincipal.SetActive(false);
    }

    public void FecharInstrucoes()
    {
        // Esconde Instruções, Mostra Menu
        if(painelInstrucoes != null) painelInstrucoes.SetActive(false);
        if(painelMenuPrincipal != null) painelMenuPrincipal.SetActive(true);
    }
}