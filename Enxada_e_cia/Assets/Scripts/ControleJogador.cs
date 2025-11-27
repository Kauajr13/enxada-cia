using UnityEngine;
using System.Collections;

public class ControleJogador : MonoBehaviour
{
    [Header("Modelos 3D (Sistema de Dublê)")]
    public GameObject modeloAndando; // Arraste aqui o boneco que anda
    public GameObject modeloAcao;    // Arraste aqui o boneco que colhe

    [Header("Configurações")]
    public float velocidade = 5.0f;
    public float velocidadeGiro = 50.0f;
    public float raioDoSensor = 0.4f;
    public float distanciaDoSensor = 0.4f;

    [Header("Inventário")]
    public DadosDaPlanta[] sementesDisponiveis;
    public bool[] sementesDesbloqueadas;
    private int indiceSelecionado = 0;
    
    [Header("Interface")]
    public HotbarUI hotbarVisual;

    private Rigidbody rb;
    private Transform cameraTransform;
    
    // Animadores separados para cada modelo
    private Animator animAndar; 
    private Animator animAcao; 
    
    private bool estaTrabalhando = false; // Trava o movimento

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        if (Camera.main != null) cameraTransform = Camera.main.transform;

        // CONFIGURAÇÃO DOS DUBLÊS
        if (modeloAndando != null) 
        {
            animAndar = modeloAndando.GetComponent<Animator>();
            modeloAndando.SetActive(true); // Começa visível
        }

        if (modeloAcao != null)
        {
            animAcao = modeloAcao.GetComponent<Animator>();
            modeloAcao.SetActive(false); // Começa escondido
        }

        // Inicializa UI
        if (hotbarVisual != null && sementesDisponiveis.Length > 0)
        {
            hotbarVisual.Inicializar(sementesDisponiveis);
            hotbarVisual.AtualizarSelecao(0);
        }
    }

    void Update()
    {
        if (estaTrabalhando) return; // Se está trabalhando, não obedece WASD

        Mover();
        TrocarSemente();
        
        if (Input.GetKeyDown(KeyCode.E) || Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0))
        {
            Interagir();
        }
    }

    void Mover()
    {
        float x = Input.GetAxisRaw("Horizontal");
        float z = Input.GetAxisRaw("Vertical");

        // Animação de Andar (apenas no modelo de andar)
        bool andando = (x != 0 || z != 0);
        if (animAndar != null) animAndar.SetBool("Andando", andando);

        if (!andando) return;

        Vector3 camFrente = cameraTransform.forward;
        Vector3 camDireita = cameraTransform.right;
        camFrente.y = 0; camDireita.y = 0;
        camFrente.Normalize(); camDireita.Normalize();

        Vector3 direcao = (camFrente * z + camDireita * x).normalized;
        Vector3 movimento = direcao * velocidade * Time.deltaTime;
        rb.MovePosition(transform.position + movimento);

        Quaternion rotacaoAlvo = Quaternion.LookRotation(direcao);
        transform.rotation = Quaternion.Lerp(transform.rotation, rotacaoAlvo, Time.deltaTime * velocidadeGiro);
    }

    void Interagir()
    {
        Vector3 centro = CalcularPosicaoDoSensor();
        Collider[] hits = Physics.OverlapSphere(centro, raioDoSensor);

        foreach (Collider hit in hits)
        {
            // 1. LADRILHO (Lógica da Troca de Boneco)
            Ladrilho planta = hit.GetComponent<Ladrilho>();
            if (planta != null)
            {
                int acao = planta.ObterTipoDeAcao(); // 1=Trabalhar, 2=Colher
                StartCoroutine(ExecutarAcaoComDuble(acao, planta));
                return;
            }

            // Outras interações (Loja, Placa, Totem)...
             PlacaDeVenda placa = hit.GetComponent<PlacaDeVenda>();
             if (placa != null) { placa.TentarComprar(); return; }
             LojaSementes loja = hit.GetComponent<LojaSementes>();
             if (loja != null) { loja.TentarComprar(); return; }
             TotemChuva totem = hit.GetComponent<TotemChuva>();
             if (totem != null) { totem.TentarAtivar(); return; }
        }
    }

    // --- A MÁGICA DA TROCA ---
    IEnumerator ExecutarAcaoComDuble(int tipoAcao, Ladrilho planta)
    {
        estaTrabalhando = true; // Trava WASD

        // 1. ESCONDE O ANDARILHO, MOSTRA O TRABALHADOR
        if (modeloAndando != null) modeloAndando.SetActive(false);
        if (modeloAcao != null) modeloAcao.SetActive(true);

        // Espera 1 frame para o Unity acordar o Animator do novo boneco
        yield return null; 

        // 2. TOCA A ANIMAÇÃO
        if (animAcao != null)
        {
            if (tipoAcao == 1) animAcao.SetTrigger("TrigTrabalhar");
            if (tipoAcao == 2) animAcao.SetTrigger("TrigColher");
        }

        // 3. ESPERA O TEMPO DA ANIMAÇÃO (Ajuste esse 1.5f se precisar)
        yield return new WaitForSeconds(0.6f);

        // 4. EFEITO NA PLANTA (Nascer/Colher)
        if (sementesDisponiveis.Length > 0 && indiceSelecionado < sementesDisponiveis.Length)
             planta.Interagir(sementesDisponiveis[indiceSelecionado]);
        else
             planta.Interagir(null);

        // 5. DESTROCA (Volta ao normal)
        if (modeloAcao != null) modeloAcao.SetActive(false);
        if (modeloAndando != null) modeloAndando.SetActive(true);
        
        estaTrabalhando = false; // Destrava WASD
    }

    void TrocarSemente()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1)) TentarTrocar(0);
        if (Input.GetKeyDown(KeyCode.Alpha2)) TentarTrocar(1);
        if (Input.GetKeyDown(KeyCode.Alpha3)) TentarTrocar(2);
    }

    void TentarTrocar(int index)
    {
        if (index < sementesDesbloqueadas.Length && sementesDesbloqueadas[index])
        {
            indiceSelecionado = index;
            if(hotbarVisual != null) hotbarVisual.AtualizarSelecao(index);
        }
    }
    
    public void DesbloquearSemente(int index) { 
        if(index < sementesDesbloqueadas.Length) sementesDesbloqueadas[index] = true; 
    }

    Vector3 CalcularPosicaoDoSensor() {
        Vector3 p = transform.position; p.y = 0.2f;
        return p + (transform.forward * distanciaDoSensor);
    }
    
    void OnDrawGizmos() {
        Gizmos.color = Color.yellow;
        if(transform != null) {
            Vector3 p = transform.position; p.y = 0.2f;
            Gizmos.DrawWireSphere(p + transform.forward * distanciaDoSensor, raioDoSensor);
        }
    }
}