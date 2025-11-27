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
    

    private Coroutine rotinaCrescimento;
    private Vector3 escalaOriginal;

    [Header("Áudio")]
    public AudioClip somPlantar;
    public AudioClip somRegar;
    public AudioClip somColher;
    [Header("3D")]
    private GameObject plantaVisualInstanciada;

    [Header("Visual")]
    public GameObject prefabGotaSede; // Arraste o prefab da Gota aqui
    private GameObject gotaInstanciada;

    void Start()
    {
        escalaOriginal = transform.localScale; 
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

    // Função Auxiliar para limpar o modelo antigo e criar o novo
    void AtualizarModeloVisual(GameObject novoPrefab, float alturaExtra = 0f)
    {
        // 1. Apaga o modelo anterior (se existir)
        if (plantaVisualInstanciada != null) Destroy(plantaVisualInstanciada);

        // 2. Cria o novo modelo (se a ficha tiver um)
        if (novoPrefab != null)
        {
            Vector3 pos = transform.position + Vector3.up * alturaExtra;
            plantaVisualInstanciada = Instantiate(novoPrefab, pos, Quaternion.identity);
            plantaVisualInstanciada.transform.SetParent(transform);
        }
    }

    IEnumerator CicloDeVida()
    {
        estadoAtual = EstadoDaPlanta.Crescendo;
        
        // VISUAL: Mostra o brotinho (modeloCrescendo)
        // Se não tiver modelo, usa a cor antiga como fallback
        if (dadosAtuais.modeloCrescendo != null)
            AtualizarModeloVisual(dadosAtuais.modeloCrescendo);
        
        yield return new WaitForSeconds(dadosAtuais.tempoParaCrescer / 2);

        EntrarEstadoSedento();
        
        yield return new WaitForSeconds(dadosAtuais.tempoDeSede);

        FicarMaduro();
    }

    void EntrarEstadoSedento()
    {
        if (estadoAtual == EstadoDaPlanta.Crescendo)
        {
            estadoAtual = EstadoDaPlanta.Sedento;
            if (prefabGotaSede != null)
            {
                Vector3 posGota = transform.position + Vector3.up * 1.5f;
                gotaInstanciada = Instantiate(prefabGotaSede, posGota, Quaternion.identity);
            }
        }
    }

    void RegarPlanta()
    {
        // TOCA O SOM DE REGAR
        if (somRegar != null) 
            AudioSource.PlayClipAtPoint(somRegar, transform.position);
        if (gotaInstanciada != null) Destroy(gotaInstanciada);
        StopCoroutine(rotinaCrescimento);
        FicarMaduro();
    }

    void FicarMaduro()
    {
        estadoAtual = EstadoDaPlanta.Maduro;
        if (gotaInstanciada != null) Destroy(gotaInstanciada);
        // VISUAL: Troca o brotinho pela planta adulta (modeloVisual)
        // Ajuste o 0.0f se precisar subir a planta
        AtualizarModeloVisual(dadosAtuais.modeloVisual, 0.0f); 


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
        if (plantaVisualInstanciada != null)
        {
            Destroy(plantaVisualInstanciada);
        }

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

    // O Jogador pergunta: "O que eu posso fazer aqui?"
    // Retorna: 0=Nada, 1=Plantar/Regar, 2=Colher
    public int ObterTipoDeAcao()
    {
        if (estadoAtual == EstadoDaPlanta.Vazio || estadoAtual == EstadoDaPlanta.Sedento)
        {
            return 1; // Animação de "Trabalhar" (Enxada)
        }
        else if (estadoAtual == EstadoDaPlanta.Maduro)
        {
            return 2; // Animação de "Colher" (Gathering)
        }
        return 0;
    }
}

