using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public static class MarcarSeleccion
{
    public static readonly Vector3Int Vector3Int_PorDefecto = Vector3Int.zero; //[ --> new Vector3(0,0,-1) ] no funcaría.

    private static Vector3Int SeleccionActual = new Vector3Int();
    private static SpriteRenderer SpriteMarcado = new SpriteRenderer();

    //"Marca" la selección colocando una tile.
    public static void Marcarseleccion(Vector3Int Nueva_Posicion)
    {
        //Borra el GameObject marcado anterior.
        if (SpriteMarcado != null) MarcarSeleccionGO(null);
        //Si Nueva_Posicion es new Vector3Int(); Entonces borra la seleccion y sal del metodo.
        if (Nueva_Posicion == Vector3Int_PorDefecto)
        {
            Mapa.tileMapUI.SetTile(SeleccionActual, null);
            SeleccionActual = Nueva_Posicion;
            AdministradorDeUI.CurrentTile = SeleccionActual;
            return;
        }
        //Si no, borra la tile anterior.
        Nueva_Posicion.z = 0;
        Mapa.tileMapUI.SetTile(SeleccionActual, null);
        //Marca en el tilemap la nueva posicion.
        Mapa.tileMapUI.SetTile(Nueva_Posicion, singletonKevin.AdminUI.SeleccionDefault);
        SeleccionActual = Nueva_Posicion;
    }

    //"Marca" la selección cambiando el material del objeto.
    public static void MarcarSeleccionGO(SpriteRenderer _GOSpriteRenderer)
    {
        //Quita la tile marcada anterior.
        if (SeleccionActual != Vector3Int_PorDefecto) Marcarseleccion(Vector3Int_PorDefecto);
        //Se entra aquí si hay instruccion de borrar la marca.
        if (_GOSpriteRenderer == null)
        {
            //Si hay gameObject marcado, regresalo a deseleccionado.
            if (SpriteMarcado != null)
                SpriteMarcado.sharedMaterial = singletonKevin.AdminUI.SpriteDefault;
            //Luego el spriteMarcado valdrá "null" y salimos de aquí.
            SpriteMarcado = _GOSpriteRenderer;
            return;
        }
        //Borra el anterior.
        if (SpriteMarcado != null)
            SpriteMarcado.sharedMaterial = singletonKevin.AdminUI.SpriteDefault;
        //Marcar con outlines.
        _GOSpriteRenderer.sharedMaterial = singletonKevin.AdminUI.SpriteOutline;
        SpriteMarcado = _GOSpriteRenderer;
    }


    public static void ReiniciarSeleccion()
    {
        AdministradorDeUI.CurrentSelect = null;
        AdministradorDeUI.CurrentTile = Vector3Int.zero;
        Marcarseleccion(new Vector3Int());
        MarcarSeleccionGO(null);
        MoverPaneles.MoverPanelInferiorUI(null);
        singletonKevin.Acciones.DesactivarTodosLosBotones();
    }
}
