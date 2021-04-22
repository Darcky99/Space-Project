using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NobleConnect.Mirror;

public abstract class SmartBehaviour : NetworkBehaviour
{
    public static List<SmartBehaviour> smartbehaviours = new List<SmartBehaviour>();
    public static SmartBehaviour local;

    [Header("SmarBehaviour")]
    public CivilizacionesSO Civilizacion;
    public SmartBehaviourMethods Logica;
    ArbolTecnologico Tecnologia = new ArbolTecnologico();

    [Header("Datos de juego")]
    public int cristales;
    [SyncVar]
    public int playerturn;

    
    public override void OnStartAuthority()
    {
        base.OnStartAuthority();

        cristales = 5;
        local = ExtraSmartBehaviour.Getlocal();
        singletonKevin.GemasDisplay.ActualizarValor();
        CmdTurno();
    }


    //En un futuro cambiar la forma en la que se asigna un turno.
    [Command]
    void CmdTurno() => playerturn = NetworkManager.singleton.numPlayers;
    

    private void OnEnable() { smartbehaviours.Add(this); }
    private void OnDisable() => smartbehaviours.Remove(this);
}

public static class ExtraSmartBehaviour
{
    public static SmartBehaviour Getlocal() 
    {
        foreach (SmartBehaviour SB in SmartBehaviour.smartbehaviours) 
        {
            if (SB.hasAuthority) return SB; 
        }
        return null; 
    }
}
