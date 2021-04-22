using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Partida
{
    public static List<Vector2Int> PlanetasIniciales(int smartBehaviours, int MinDistancia)
    {
        List<Vector2Int> PlanetasPos = new List<Vector2Int>();
        Vector3Int posaa_ = new Vector3Int();

        //Toma una lista de todas las coordenadas de planetas habitables.
        for (int x = 0; x < 100; x++)
        {
            if (PlanetasPos.Count > smartBehaviours)
                break;
            for (int y = 0; y < 100; y++) {
                posaa_ = new Vector3Int(x, y, 0);

                if (Mapa.tileMap.HasTile(posaa_) && Mapa.tileMap.GetSprite(posaa_).name.Contains("p"))
                    PlanetasPos.Add(new Vector2Int(posaa_.x, posaa_.y));
            } }
        //Por cada jugador o bot, añade un planeta que cumpla con una distancia mínima entre ellos.
        //Si ni uno de los planetas cumple con la mínima distancia, selecciona el que "mejor cumpla la condición de minDistancia".
        return PlanetasPos;
    }
}
