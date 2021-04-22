using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Planetas : Construcciones
{
    public PlanetaSO Planeta_Tipo;
    //public List<MyAccionespDelegate> Acciones = new List<MyAccionespDelegate>();
    int GemasPorTurno = 1;

    #region Inicialización
    private void Start() {
        //Suscribirse a evento de nuevo turno.
        AdministradorDeTurnos.NuevoTurno += NuevoTurno;
    }  
    private void OnDisable() => AdministradorDeTurnos.NuevoTurno -= NuevoTurno; 

    public void AsignarNombre()
    {
        //Toma una nombre y aumenta el índice de nombre.
        NombreDisplay.text = Planeta_Tipo.NombresDePlanetas[Planeta_Tipo.namecount];

        if(Planeta_Tipo.namecount >= Planeta_Tipo.NombresDePlanetas.Count - 1)
        Planeta_Tipo.namecount = 0;
        else
        Planeta_Tipo.namecount++;
    }
    #endregion


    public void Terraformar()
    {
        _SpriteRenderer.sprite = Planeta_Tipo.Imagen;
    }



    void NuevoTurno(int Turno){
        //Si tengo autoridad y es mi turno...
        if(hasAuthority && Turno == SmartBehaviour.local.playerturn){
            //Sumar mis gemas, reactivar acciones.
            SmartBehaviour.local.Logica.AumentarGemas(GemasPorTurno);
        }
    }

}