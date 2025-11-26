using UnityEngine;
using TMPro; // Necessário para atualizar o texto flutuante sozinho

public class LojaSementes : MonoBehaviour
{
    [Header("Configuração")]
    public DadosDaPlanta fichaDaPlanta; // Arraste o arquivo (Tomate/Abóbora) aqui
    public int indiceNoInventario = 1; // 1 = Tomate, 2 = Abóbora (Ainda precisamos disso para saber qual slot liberar)
    
    [Header("Visual (Opcional)")]
    public TextMeshPro textoDoPreco; // Arraste o texto 3D aqui
    public AudioClip somVenda;

    void Start()
    {
        // AUTOMATIZAÇÃO: Atualiza o texto 3D sozinho ao dar Play
        if (textoDoPreco != null && fichaDaPlanta != null)
        {
            textoDoPreco.text = $"{fichaDaPlanta.nomeDaPlanta}\n${fichaDaPlanta.precoParaDesbloquear}";
        }
    }

    public void TentarComprar()
    {
        if (fichaDaPlanta == null) return;

        GerenciadorJogo banco = FindObjectOfType<GerenciadorJogo>();
        ControleJogador jogador = FindObjectOfType<ControleJogador>();

        // Verifica se já comprou
        if (jogador.sementesDesbloqueadas[indiceNoInventario] == true)
        {
            Debug.Log("Já comprado!");
            Destroy(gameObject); // Ou desativa
            return;
        }

        // PEGA O PREÇO DIRETO DA FICHA (Sem redundância!)
        int custoReal = fichaDaPlanta.precoParaDesbloquear;

        // Tenta gastar
        if (banco != null && banco.GastarDinheiro(custoReal))
        {
            banco.MostrarTextoFlutuante(transform.position, "-$" + custoReal, Color.red);
            jogador.DesbloquearSemente(indiceNoInventario);
            
            // Toca som se tiver
            AudioSource.PlayClipAtPoint(somVenda, transform.position);

            Destroy(gameObject); 
        }
    }
}