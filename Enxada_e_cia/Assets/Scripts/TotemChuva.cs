using UnityEngine;

public class TotemChuva : MonoBehaviour
{
    public int precoDaOferenda = 300 ; // Custa $50 para fazer chover
    public AudioClip somVenda;

    public void TentarAtivar()
    {
        GerenciadorJogo banco = FindObjectOfType<GerenciadorJogo>();
        ClimaManager clima = FindObjectOfType<ClimaManager>();

        if (banco != null && clima != null)
        {
            // Tenta pagar a oferenda
            if (banco.GastarDinheiro(precoDaOferenda))
            {
                // Sucesso! Faz chover.
                clima.AtivarChuva();
                
                AudioSource.PlayClipAtPoint(somVenda, transform.position);
                banco.MostrarTextoFlutuante(transform.position, "-$" + precoDaOferenda, Color.red);
                // Opcional: Efeito visual ou som no Totem
                Debug.Log("Oferenda aceita! Vai chover.");
            }
            else
            {
                Debug.Log("Dinheiro insuficiente para a oferenda da chuva.");
            }
        }
    }
}