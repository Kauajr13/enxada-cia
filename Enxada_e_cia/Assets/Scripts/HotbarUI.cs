using UnityEngine;
using UnityEngine.UI; // Necessário para mexer nas Imagens

public class HotbarUI : MonoBehaviour
{
    [Header("Conexões da UI")]
    public Image[] bordasDosSlots; // As caixas de fora (Slot_1, Slot_2...)
    public Image[] iconesDasSementes; // As imagens de dentro (Icone)

    [Header("Cores")]
    public Color corSelecionado = Color.yellow; // Borda brilha amarelo
    public Color corDeselecionado = Color.white; // Borda normal branca

    // Chamamos isso no Start para pintar os ícones com as cores das plantas
    public void Inicializar(DadosDaPlanta[] sementes)
    {
        for (int i = 0; i < iconesDasSementes.Length; i++)
        {
            if (i < sementes.Length)
            {
                // Pega a cor "Madura" da planta para usar como ícone
                iconesDasSementes[i].color = sementes[i].corMadura;
            }
            else
            {
                // Se não tem semente nesse slot, esconde o ícone
                iconesDasSementes[i].color = Color.clear;
            }
        }
    }

    // Chamamos isso toda vez que apertar 1, 2 ou 3
    public void AtualizarSelecao(int indice)
    {
        for (int i = 0; i < bordasDosSlots.Length; i++)
        {
            if (i == indice)
            {
                // Este é o slot selecionado!
                bordasDosSlots[i].color = corSelecionado;
                
                // Opcional: Aumentar um pouquinho o tamanho para dar destaque
                bordasDosSlots[i].rectTransform.localScale = Vector3.one * 1.1f;
            }
            else
            {
                // Slot normal
                bordasDosSlots[i].color = corDeselecionado;
                bordasDosSlots[i].rectTransform.localScale = Vector3.one;
            }
        }
    }
}