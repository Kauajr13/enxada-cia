using UnityEngine;

public class CicloDiaNoite : MonoBehaviour
{
    [Header("Configuração")]
    public float duracaoDoDiaEmSegundos = 60f; // 1 minuto real = 24h no jogo
    
    [Header("Cores do Céu")]
    public Color corDia = new Color(1f, 1f, 1f); // Branco
    public Color corNoite = new Color(0.1f, 0.1f, 0.3f); // Azul escuro

    private Light minhaLuz;

    void Start()
    {
        minhaLuz = GetComponent<Light>();
    }

    void Update()
    {
        // 1. GIRA O SOL (Eixo X)
        // Calcula quanto girar por frame para completar 360 graus no tempo estipulado
        float velocidadeGiro = 360f / duracaoDoDiaEmSegundos * Time.deltaTime;
        transform.Rotate(Vector3.right * velocidadeGiro);

        // 2. MUDA A COR DA LUZ E INTENSIDADE
        // Verifica a rotação atual (0 a 360)
        float anguloX = transform.rotation.eulerAngles.x;

        // Se o sol está alto (Dia) ou baixo (Noite)
        // Essa lógica simples verifica se o sol está "em cima" (0 a 180) ou "em baixo"
        if (anguloX > 0 && anguloX < 180)
        {
            // É DIA
            minhaLuz.color = Color.Lerp(minhaLuz.color, corDia, Time.deltaTime);
            minhaLuz.intensity = Mathf.Lerp(minhaLuz.intensity, 1.0f, Time.deltaTime);
        }
        else
        {
            // É NOITE
            minhaLuz.color = Color.Lerp(minhaLuz.color, corNoite, Time.deltaTime);
            minhaLuz.intensity = Mathf.Lerp(minhaLuz.intensity, 0.2f, Time.deltaTime);
        }
    }
}