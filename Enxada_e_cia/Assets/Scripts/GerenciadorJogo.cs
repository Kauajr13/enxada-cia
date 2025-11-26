using UnityEngine;
using TMPro;

public class GerenciadorJogo : MonoBehaviour
{
    public TextMeshProUGUI textoNaTela; 
    
    // Novas variáveis econômicas
    private int pessoasAlimentadas = 0;
    public int dinheiroAtual = 0; // Começa pobre
    [Header("Efeitos Visuais")]
    public GameObject prefabTextoFlutuante;

    void Start()
    {
        AtualizarUI();
    }

    public void RegistrarColheita(int valorDaVenda)
    {
        pessoasAlimentadas++;
        dinheiroAtual += valorDaVenda; // Ganha dinheiro baseado na planta!
        AtualizarUI();
    }

    // Função para tentar comprar coisas (retorna true se conseguiu)
    public bool GastarDinheiro(int custo)
    {
        if (dinheiroAtual >= custo)
        {
            dinheiroAtual -= custo;
            AtualizarUI();
            return true; // Compra aprovada
        }
        else
        {
            Debug.Log("Dinheiro insuficiente!");
            return false; // Compra negada
        }
    }

    public void MostrarTextoFlutuante(Vector3 posicao, string mensagem, Color cor)
    {
        if (prefabTextoFlutuante != null)
        {
            // Cria o texto um pouco acima do objeto (posicao + up)
            GameObject textoObj = Instantiate(prefabTextoFlutuante, posicao + Vector3.up * 1.5f, Quaternion.identity);
            
            // Configura a mensagem
            textoObj.GetComponent<TextoFlutuante>().Configurar(mensagem, cor);
        }
    }

    void AtualizarUI()
    {
        // Mostra as duas informações
        textoNaTela.text = $"Pessoas: {pessoasAlimentadas} | $: {dinheiroAtual}";
    }
}