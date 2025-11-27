using UnityEngine;

public class OlharParaCamera : MonoBehaviour
{
    private Camera cam;

    void Start()
    {
        cam = Camera.main;
    }

    void LateUpdate()
    {
        if (cam != null)
        {
            // Força o objeto a olhar na mesma direção que a câmera olha
            // Isso previne rotações estranhas de cabeça para baixo
            transform.rotation = cam.transform.rotation;
        }
    }
}