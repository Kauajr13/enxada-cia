using UnityEngine;

public class ControleJogador : MonoBehaviour
{
    [Header("Movimentação")]
    public float velocidade = 5.0f;
    public float velocidadeGiro = 50.0f;

    [Header("Interação")]
    [Range(0.1f, 1.0f)] 
    public float raioDoSensor = 0.3f; // DIMINUI O TAMANHO DA BOLHA (Mais preciso)
    public float distanciaDoSensor = 0.5f; // Distância do corpo

    [Header("Inventário de Sementes")]
    public DadosDaPlanta[] sementesDisponiveis;
    public bool[] sementesDesbloqueadas;
    private int indiceSelecionado = 0;

    private Rigidbody rb;
    private Transform cameraTransform;
    [Header("Interface")]
    public HotbarUI hotbarVisual; // Arraste o script da UI aqui

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        if (Camera.main != null) cameraTransform = Camera.main.transform;

        // --- NOVO CÓDIGO ---
        if (hotbarVisual != null)
        {
            // Manda as sementes (Milho/Tomate/Abobora) pra UI pintar os ícones
            hotbarVisual.Inicializar(sementesDisponiveis);
            // Já seleciona o primeiro
            hotbarVisual.AtualizarSelecao(0);
        }
    }

    void Update()
    {
        Mover();
        TrocarSemente();
        
        if (Input.GetKeyDown(KeyCode.E) || Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0))
        {
            Interagir();
        }
    }

    void TrocarSemente()
    {
        // Só troca se o índice existir E estiver desbloqueado
        if (Input.GetKeyDown(KeyCode.Alpha1)) TentarTrocar(0);
        if (Input.GetKeyDown(KeyCode.Alpha2)) TentarTrocar(1);
        if (Input.GetKeyDown(KeyCode.Alpha3)) TentarTrocar(2);
    }

    void TentarTrocar(int index)
    {
        // Proteção para não dar erro se configurar errado no Inspector
        if (index >= sementesDesbloqueadas.Length) return;

        if (sementesDesbloqueadas[index] == true)
        {
            indiceSelecionado = index;
            if(hotbarVisual != null) hotbarVisual.AtualizarSelecao(index);
            Debug.Log("Equipou: " + sementesDisponiveis[index].nomeDaPlanta);
        }
        else
        {
            Debug.Log("Semente Bloqueada! Compre na loja.");
            // Opcional: Tocar som de "Erro/Negado"
        }
    }

    public void DesbloquearSemente(int index)
    {
        if (index < sementesDesbloqueadas.Length)
        {
            sementesDesbloqueadas[index] = true;
            Debug.Log("Nova semente aprendida!");
        }
    }

    void Mover()
    {
        float x = Input.GetAxisRaw("Horizontal");
        float z = Input.GetAxisRaw("Vertical");

        if (x == 0 && z == 0) return;

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

    // --- A CORREÇÃO DE ALTURA ESTÁ AQUI ---
    Vector3 CalcularPosicaoDoSensor()
    {
        // 1. Pega a posição do pé do jogador
        Vector3 posicao = transform.position;
        
        // 2. FORÇA a altura para ser quase no chão (0.2 metros)
        // Isso ignora se o personagem é gigante ou anão. O sensor sempre vai raspar o chão.
        posicao.y = 0.2f; 

        // 3. Projeta para frente
        return posicao + (transform.forward * distanciaDoSensor);
    }

    void Interagir()
    {
        Vector3 centroDaBolha = CalcularPosicaoDoSensor();
        Collider[] objetosTocados = Physics.OverlapSphere(centroDaBolha, raioDoSensor);

        foreach (Collider colisor in objetosTocados)
        {
            // 1. Tenta ver se é um LADRILHO (Prioridade)
            Ladrilho planta = colisor.GetComponent<Ladrilho>();
            if (planta != null)
            {
                if (sementesDisponiveis.Length > 0 && indiceSelecionado < sementesDisponiveis.Length)
                    planta.Interagir(sementesDisponiveis[indiceSelecionado]);
                else
                    planta.Interagir(null);
                return;
            }

            // 2. Se não for planta, vê se é uma PLACA DE VENDA
            PlacaDeVenda placa = colisor.GetComponent<PlacaDeVenda>();
            if (placa != null)
            {
                placa.TentarComprar();
                return;
            }

            // 3. Também pode ser a loja de sementes
            LojaSementes loja = colisor.GetComponent<LojaSementes>();
            if (loja != null)
            {
                loja.TentarComprar();
                return;
            }

            // 4. TOTEM DA CHUVA (NOVO!)
            TotemChuva totem = colisor.GetComponent<TotemChuva>();
            if (totem != null)
            {
                totem.TentarAtivar();
                return;
            }
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        // Usa a mesma matemática para você ver exatamente onde está batendo
        // Como o script roda no editor, precisamos recalcular aqui manualmente
        if (transform != null) 
        {
             Vector3 posicao = transform.position;
             posicao.y = 0.2f; // Forçando chão no visual também
             Vector3 centro = posicao + (transform.forward * distanciaDoSensor);
             Gizmos.DrawWireSphere(centro, raioDoSensor);
        }
    }
}