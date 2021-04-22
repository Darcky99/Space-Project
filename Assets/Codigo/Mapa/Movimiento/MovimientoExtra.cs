using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public static class MovimientoExtra
{
    public static int NodoCaso(Vector3Int posicion)
    {
        Tilemap tiles = Mapa.tileMap;
        Grid grid = singletonKevin.mapa.grid_;
        Vector2 PosWorl = grid.CellToWorld(posicion);   PosWorl.y += 0.25f;
        
        
        Collider2D[] colliders = Physics2D.OverlapCircleAll(PosWorl, 0.01f);
        

        if (tiles.HasTile(posicion) == false && colliders.Length == 0) return 0;  //Si no hay tile ni colider.

        //Si llegamos hasta aqui, hay una contruccion, una nave o una tile.
        if (colliders.Length == 1)
        {
            string tag = colliders[0].tag;
            //0 Para tile vacía. 1 Para penalizador y 2 para obstaculo.
            switch (tag)
            {
                case "Nave":
                    return 2;
                case "Construccion":
                    return 0;
            }
        }
        else if (colliders.Length == 2) return 2;

        //Si llegamos hasta aquí es porque no hay collider pero hay tile.
        string tilename = tiles.GetTile(posicion).name;

        if (tilename.Contains("Planeta")) return 0;
        else if (tilename.Contains("Nebulosa")) return 1;
        else if (tilename.Contains("Estrella")) { return 2; }

        //[En un futuro abrá que "marcar" como 2 las tiles alrededor de una estrella.

        return -1;
    }
}