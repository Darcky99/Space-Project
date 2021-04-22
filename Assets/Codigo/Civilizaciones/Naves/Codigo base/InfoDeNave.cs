using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Mirror;

public enum Ataque { Disponible, Agotado }
public delegate void MyAccionespDelegate();

public class InfoDeNave : NetworkBehaviour
{
    [Header("Logica")]
    public LogicaNave Logica;
    public SpriteRenderer Spriterenderer;
    public List<MyAccionespDelegate> Acciones = new List<MyAccionespDelegate>();

    [Header("Caracteristicas")]
    public NavesSO TipoDeNaveSO;

    [Header("UI")]
    public TextMeshProUGUI Vida_diplay;

    [Header("Datos")]
    public Ataque EstadoAtaque = Ataque.Disponible;

    [HideInInspector]
    public int VidaDiponible = 0;
    [HideInInspector]
    public int MovimientoDisponible = 0;
    [HideInInspector]
    
    //Suscribe a evento y toma SpriteRender..
    public void Inicializacion()
    {
        Spriterenderer = GetComponent<SpriteRenderer>();
        AdministradorDeTurnos.NuevoTurno += Logica.NuevoTurno;
        
        Spriterenderer.sprite = TipoDeNaveSO.Sprites.Oeste_Idle;
        VidaDiponible = TipoDeNaveSO.Vida;
        MovimientoDisponible = TipoDeNaveSO.Movilidad;
        Logica.ActualizarVida();
        AñadirAcciones();
    }


    void AñadirAcciones(){
        if(TipoDeNaveSO.Caracteristicas_.Contains(Caracteristicas.Reparar))
        Acciones.Add(AccionesUI.Reparar);
        if(TipoDeNaveSO.Caracteristicas_.Contains(Caracteristicas.Construccion))
        Acciones.Add(AccionesUI.Colonizar);

    }

    private void OnDisable()=>AdministradorDeTurnos.NuevoTurno -= Logica.NuevoTurno;
    
}