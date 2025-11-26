using UnityEngine;
using UnityEngine.SceneManagement; // Necessário para mudar de cena

public class MenuManager : MonoBehaviour
{
    public void IniciarJogo()
    {
        SceneManager.LoadScene("CenaDaFazenda");
    }

    public void SairDoJogo()
    {
        Debug.Log("Saindo...");
        Application.Quit();
    }
}