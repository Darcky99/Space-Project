using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;


public static class UIMetodosPaneles
{
    static TextMeshProUGUI NombreText = singletonKevin.AdminUI.NombreDeLaSeleccion;
    static TextMeshProUGUI DescripcionText = singletonKevin.AdminUI.DescripcionEnPanel;

    public static void CambiarInfoDePanelInferior(InfoParaPanelInferior Info)
    {
        if(Info == null)
        {
            NombreText.text = ""; DescripcionText.text = ""; return;
        }

        NombreText.text = Info.Nombre;
        DescripcionText.text = Info.Descripcion;
        //En un futuro mostrar imagen y acciones.
    }

    public static Vector3Int DeScreenPosAGrid(Vector2 ScreenPos, int Z)
    {
        Grid grid = singletonKevin.mapa.grid_;
        Vector2 MousePos = Input.mousePosition;
        Vector3 PosWorld = Camera.main.ScreenToWorldPoint(MousePos); PosWorld = new Vector3(PosWorld.x, PosWorld.y, 0); //Pasa de SceenPos a WorldPos.
        Vector3Int PosGrid = grid.WorldToCell(PosWorld);
        PosGrid = new Vector3Int(PosGrid.x, PosGrid.y, Z);
        return PosGrid;
    }
}




public class InfoParaPanelInferior
{
    public string Nombre { get; set; }
    public string Descripcion { get; set; }
    public bool Autoridad { get; set; }
    public Sprite Imagen { get; set; }

    public InfoParaPanelInferior() { Nombre = ""; Descripcion = ""; Autoridad = false; }

    public InfoParaPanelInferior(string nombre, string descripcion)
    {
        Nombre = nombre; Descripcion = descripcion;
    }

    public InfoParaPanelInferior(string nombre, string descripcion, Sprite imagen)
    {
        Nombre = nombre; Descripcion = descripcion; Imagen = imagen;
    }
    
}