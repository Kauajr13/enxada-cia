using UnityEngine;

[CreateAssetMenu(fileName = "Nova Planta", menuName = "Fazenda/Planta")]
public class DadosDaPlanta : ScriptableObject
{
    public string nomeDaPlanta;
    public Color corCrescendo;
    public Color corSedenta; // Adicionei cor específica para sede na ficha
    public Color corMadura;
    public float tempoParaCrescer;
    public float tempoDeSede;
    public int valorDeVenda;
    public int precoParaDesbloquear;
}