using UnityEngine;

public class PlacaDeVenda : MonoBehaviour
{
    public GameObject terrenoParaLiberar; // O grupo de ladrilhos que vai aparecer
    public int preco = 100;
    public AudioClip somVenda;
    
    // Quando comprado, a placa se destroi ou muda de cor
    public void TentarComprar()
    {
        GerenciadorJogo banco = FindObjectOfType<GerenciadorJogo>();
        
        if (banco != null)
        {
            // Tenta gastar o dinheiro
            if (banco.GastarDinheiro(preco))
            {
                // SUCESSO!
                Debug.Log("Terreno Comprado!");

                AudioSource.PlayClipAtPoint(somVenda, transform.position);
                banco.MostrarTextoFlutuante(transform.position, "-$" + preco, Color.red);
                
                // Ativa os ladrilhos escondidos
                if(terrenoParaLiberar != null) 
                    terrenoParaLiberar.SetActive(true);
                
                // Some com a placa (já que já comprou)
                Destroy(gameObject); 
                
                // Opcional: Tocar som de caixa registradora aqui
            }
        }
    }
}