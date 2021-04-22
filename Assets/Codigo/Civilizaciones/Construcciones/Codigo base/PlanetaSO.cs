using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Planeta", menuName = "Civilizaciones/Construcciones/Planeta")]
public class PlanetaSO : ConstruccionesSO
{
    [SerializeField]
    public List<string> NombresDePlanetas = new List<string>();
    public int namecount = 0;

    private void OnEnable() => namecount = Random.Range(0, NombresDePlanetas.Count);
    //private void OnDisable() => namecount = 0;

    
}
