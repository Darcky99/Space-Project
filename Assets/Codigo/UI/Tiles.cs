using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Tiles
{
    public static InfoParaPanelInferior GetTileInfo(string tile_name)
    {
        string nombre = "No incluido";
        string descripcion = "No incliudo";

        if (tile_name.Contains("Nebulosa")) { nombre = "Nebulosa"; descripcion = "huele raro"; }
        else if (tile_name.Contains("Luna")) { nombre = "Luna"; descripcion = "made out of chese"; }
        else if (tile_name.Contains("Planeta")) { nombre = "Planeta"; descripcion = "Colonizable"; }
        else if (tile_name.Contains("Asteroides")) {
            nombre = "Asteroides"; descripcion = "picarlos";
            if (tile_name.Contains("AsteroidesRaros")) { nombre = "Asteroides raros"; descripcion = "PICARLOS"; } }
        else if (tile_name.Contains("Estrella")) { nombre = "Estrella"; descripcion = "ta caliente"; }

        return new InfoParaPanelInferior(nombre, descripcion);
    }

    public static bool HayNodoMovimiento(Vector3Int PosGrid)
    {
        bool HayNodo = false;
        Vector3Int PosEnCero = new Vector3Int(PosGrid.x, PosGrid.y, 0);

        if (Mapa.tileMapUI.HasTile(PosEnCero) && Mapa.tileMapUI.GetSprite(PosEnCero).name.Contains("movimiento"))
            HayNodo = true;
        return HayNodo;
    }
}
