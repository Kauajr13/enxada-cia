using UnityEngine;
using System.Collections;

public class Ladrilho : MonoBehaviour
{
    private enum EstadoDaPlanta { Vazio, Crescendo, Sedento, Maduro }
    private EstadoDaPlanta estadoAtual = EstadoDaPlanta.Vazio;

    [Header("Componentes")]
    public GameObject particulaColheitaPrefab;

    // Esta variável vai guardar a ficha (Milho/Tomate) que o jogador escolheu
    private DadosDaPlanta dadosAtuais; 
    
    private Renderer meuRenderizador;
    private Coroutine rotinaCrescimento;
    private Vector3 escalaOriginal;

    [Header("Áudio")]
    public AudioClip somPlantar;
    public AudioClip somRegar;
    public AudioClip somColher;

    void Start()
    {
        meuRenderizador = GetComponent<Renderer>();
        escalaOriginal = transform.localScale; 
        
        // Garante que começa branco
        meuRenderizador.material.color = Color.white;
    }

    // --- FUNÇÃO DE INTERAÇÃO (Chamada pelo Jogador) ---
    // O parâmetro 'semente' é opcional. Se não mandar nada, é null.
    public void Interagir(DadosDaPlanta semente = null)
    {
        // 1. TENTANDO PLANTAR
        if (estadoAtual == EstadoDaPlanta.Vazio)
        {
            if (semente != null)
            {
                dadosAtuais = semente; // Salva a ficha recebida
                rotinaCrescimento = StartCoroutine(CicloDeVida());
            }
            else
            {
                Debug.Log("O Ladrilho está vazio, mas você não escolheu uma semente!");
            }
        }
        // 2. TENTANDO REGAR
        else if (estadoAtual == EstadoDaPlanta.Sedento)
        {
            RegarPlanta();
        }
        // 3. TENTANDO COLHER
        else if (estadoAtual == EstadoDaPlanta.Maduro)
        {
            ColherPlanta();
        }
    }

    IEnumerator CicloDeVida()
    {
        estadoAtual = EstadoDaPlanta.Crescendo;
        
        if (somPlantar != null) 
            AudioSource.PlayClipAtPoint(somPlantar, transform.position);

        // Usa a cor da ficha da planta
        meuRenderizador.material.color = dadosAtuais.corCrescendo;
        
        // Espera metade do tempo definido na ficha
        yield return new WaitForSeconds(dadosAtuais.tempoParaCrescer / 2);

        EntrarEstadoSedento();
        
        // Espera o tempo de sede definido na ficha
        yield return new WaitForSeconds(dadosAtuais.tempoDeSede);

        FicarMaduro();
    }

    void EntrarEstadoSedento()
    {
        if (estadoAtual == EstadoDaPlanta.Crescendo)
        {
            estadoAtual = EstadoDaPlanta.Sedento;
            // Usa a cor de sede da ficha (ou azul padrão se esquecer de configurar)
            if(dadosAtuais.corSedenta != Color.clear)
                meuRenderizador.material.color = dadosAtuais.corSedenta;
            else
                meuRenderizador.material.color = Color.blue;
        }
    }

    void RegarPlanta()
    {
        // TOCA O SOM DE REGAR
        if (somRegar != null) 
            AudioSource.PlayClipAtPoint(somRegar, transform.position);
        Debug.Log("Regado!");
        StopCoroutine(rotinaCrescimento);
        FicarMaduro();
    }

    void FicarMaduro()
    {
        estadoAtual = EstadoDaPlanta.Maduro;
        meuRenderizador.material.color = dadosAtuais.corMadura;
        StartCoroutine(AnimacaoPop());
    }

    void ColherPlanta()
    {
        // TOCA O SOM DE COLHER
        if (somColher != null) 
            AudioSource.PlayClipAtPoint(somColher, transform.position);

        estadoAtual = EstadoDaPlanta.Vazio;
        
        GerenciadorJogo gerente = FindObjectOfType<GerenciadorJogo>();
        
        // --- MUDANÇA AQUI ---
        if (gerente != null) 
        {
            // Agora passamos o VALOR da ficha da planta (ex: 10 ou 50)
            gerente.RegistrarColheita(dadosAtuais.valorDeVenda);
            gerente.MostrarTextoFlutuante(transform.position, "+$" + dadosAtuais.valorDeVenda, Color.green);
        }
        // --------------------

        if (particulaColheitaPrefab != null) Instantiate(particulaColheitaPrefab, transform.position, Quaternion.identity);

        meuRenderizador.material.color = Color.white;
        dadosAtuais = null;
    }

    IEnumerator AnimacaoPop()
    {
        transform.localScale = escalaOriginal * 1.2f; 
        yield return new WaitForSeconds(0.1f);
        transform.localScale = escalaOriginal; 
    }

    public void ReceberChuva()
    {
        // Se a planta estiver com sede, a chuva resolve!
        if (estadoAtual == EstadoDaPlanta.Sedento)
        {
            RegarPlanta();
        }
    }
}

