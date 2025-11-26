using UnityEngine;
using TMPro; // Necessário para TextMeshPro

public class TextoFlutuante : MonoBehaviour
{
    public float velocidadeSubida = 2f;
    public float tempoDeVida = 1.5f;
    
    private TextMeshPro textoComponente;

    void Awake()
    {
        textoComponente = GetComponent<TextMeshPro>();
    }

    void Start()
    {
        // 1. Destroi o objeto depois de X segundos para não pesar o jogo
        Destroy(gameObject, tempoDeVida);
    }

    void Update()
    {
        // 2. Faz o texto subir
        transform.position += Vector3.up * velocidadeSubida * Time.deltaTime;

        // 3. Faz o texto olhar sempre para a câmera (Billboarding)
        if (Camera.main != null)
        {
            transform.rotation = Camera.main.transform.rotation;
        }
    }

    // Função para configurar o texto na hora que nasce
    public void Configurar(string mensagem, Color cor)
    {
        if (textoComponente != null)
        {
            textoComponente.text = mensagem;
            textoComponente.color = cor;
        }
    }
}