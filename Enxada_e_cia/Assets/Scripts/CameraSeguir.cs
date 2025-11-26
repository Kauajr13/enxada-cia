using UnityEngine;

public class CameraSeguir : MonoBehaviour
{
    public Transform alvo; // O Personagem
    public float suavidade = 0.125f; // Quanto menor, mais suave (e atrasado)
    
    private Vector3 distanciaOriginal; // Guarda a posição relativa inicial

    void Start()
    {
        // Calcula a distância atual entre a câmera e o boneco
        // Assim você pode ajustar a câmera na Scene e ele respeita
        distanciaOriginal = transform.position - alvo.position;
    }

    // LateUpdate roda DEPOIS que o personagem se moveu (evita tremedeira)
    void LateUpdate()
    {
        if (alvo == null) return;

        // Onde a câmera quer ir
        Vector3 posicaoDesejada = alvo.position + distanciaOriginal;
        
        // Move suavemente da posição atual para a desejada
        Vector3 posicaoSuave = Vector3.Lerp(transform.position, posicaoDesejada, suavidade);
        
        transform.position = posicaoSuave;
    }
}