using UnityEngine;

public class OlharParaCamera : MonoBehaviour
{
    private Camera cameraPrincipal;

    void Start()
    {
        // Encontra a câmera principal automaticamente
        cameraPrincipal = Camera.main;
    }

    // Usamos LateUpdate para garantir que o texto gire SÓ DEPOIS 
    // que a câmera terminar de se mover (evita tremedeira visual)
    void LateUpdate()
    {
        if (cameraPrincipal != null)
        {
            // O Truque: Em vez de "olhar para" a câmera (o que inverteria o texto),
            // nós copiamos a rotação dela. Assim o texto fica "chapado" na tela.
            transform.rotation = cameraPrincipal.transform.rotation;
        }
    }
}