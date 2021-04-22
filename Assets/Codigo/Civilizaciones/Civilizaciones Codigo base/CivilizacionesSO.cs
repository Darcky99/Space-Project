using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "Nueva Civilizacion", menuName = "Civilizaciones/Crear civilizacion")]
public class CivilizacionesSO : ScriptableObject
{
    public string CivilizacionNombre;
    public Color color;
    [TextArea(2, 5)]
    public string Descripcion;

    [Header("Construcciones")]
    public PlanetaSO Planeta;
    public List<string> NombresDePlanetas;

    [Header("Naves")]
    public CazaEstandarSO CazaEstandar;
    public InterceptorSO Interceptor;
    public FundadoraSO Fundadora;

    public GameObject NavePrefab;
}
