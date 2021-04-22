using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using NobleConnect.Mirror;
using TMPro;
using UnityEngine.UI;

public class NetworkManagerTest : NobleNetworkManager
{
    [Header("Configuración del juego")]
    public static Vector2Int Dimensiones = new Vector2Int(100, 100);
    public static int Escala = 5;
    [Header("Referencias")]
    public Button BotonIniciarJuego;

    public override void Start()
    {
        base.Start();
        StartHostLANOnly();
        Invoke("IniciarJuego", 1f);
    }


    [Server]
    public void IniciarJuego()
    {
        singletonKevin.AdminDeTurno.RpcEstablecerJugadores();
        //Genera mapa.
        GeneracionDeTerreno.GenerarMapa(TipoDeMapa.SistemasSolares, new Vector2Int(Dimensiones.x, Dimensiones.y));
        StartCoroutine(AsignarPlanetas());
    }
    [Server]
    IEnumerator AsignarPlanetas()
    {
        yield return new WaitForSeconds(0.01f);
        List<Vector2Int> PlanetasIniciasles = Partida.PlanetasIniciales(SmartBehaviour.smartbehaviours.Count, 35);
        int c = 0;
        foreach (SmartBehaviour player in SmartBehaviour.smartbehaviours) { player.Logica.CmdPrimerSpawn(PlanetasIniciasles[c]); c++; }
        yield return null;
    }
    

    //Extras.
    [Server]
    public void RegenerarMapa()
    {
        GeneracionDeTerreno.LimpiarMapa();
        GeneracionDeTerreno.GenerarMapa(TipoDeMapa.SistemasSolares, new Vector2Int(Dimensiones.x, Dimensiones.y));
    }

    public void Borrar() => GeneracionDeTerreno.LimpiarMapa();



    public override void OnServerDisconnect(NetworkConnection conn)
    {
        GeneracionDeTerreno.LimpiarMapa();

        //Añade a una lista a todos las NetIden excepto al PlayerPref.
        List<NetworkIdentity> NetIdenList = new List<NetworkIdentity>();
        foreach (NetworkIdentity NetIden in conn.clientOwnedObjects)
        {
            if (!NetIden.CompareTag("Player"))
                NetIdenList.Add(NetIden);
        }
        //Se modifica indirectamente para evitar errores.
        foreach (NetworkIdentity NetIden in NetIdenList) { NetIden.RemoveClientAuthority(); }

        base.OnServerDisconnect(conn);
    }


    public override void OnStopServer()
    {
        base.OnStopServer();
        GeneracionDeTerreno.LimpiarMapa();
    }
    public override void OnStopClient()
    {
        base.OnStopClient();
        GeneracionDeTerreno.LimpiarMapa();
    }

    
}