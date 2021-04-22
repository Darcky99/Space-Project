using UnityEngine;

public abstract class ConstruccionesSO : ScriptableObject
{
    public string Nombre;
    [TextArea(3, 8)]
    public string Descripcion;
    public Sprite Imagen;
    public GameObject Prefab;
}
