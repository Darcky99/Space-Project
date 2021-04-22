using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Comparaciones
{
    public static bool EstaEnLista(Vector2Int Valor, List<Vector2Int> Lista)
    {
        bool EstaEnLista = false;
        foreach (Vector2Int coord in Lista)
            if (coord == Valor) { EstaEnLista = true; break; }
        return EstaEnLista;
    }
}