using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class AccionesUI : MonoBehaviour
{
    public static void Reparar()
    {
        LogicaNave Logica = AdministradorDeUI.CurrentSelect.GetComponent<LogicaNave>();
        Logica.Info.EstadoAtaque = Ataque.Agotado;
        Logica.Info.MovimientoDisponible = 0;

        //Cura la nave. Esto agotará el en todos los clientes.
        Logica.CmdAlterarVida(Logica.transform, 5);

        //Esto hace que se "recarguen" las acciones disponibles. 
        //[Seguramente esté mal y tenga que dejarlo como el de "Colonizar"]
        Seleccionar.SeleccionarNave(Logica.gameObject);
    }




    public static void Colonizar(){
        LogicaNave Logica = AdministradorDeUI.CurrentSelect.GetComponent<LogicaNave>();
        Logica.Info.EstadoAtaque = Ataque.Agotado;
        Logica.Info.MovimientoDisponible = 0;
        
        //TilePos:
        Vector2Int CellPos = (Vector2Int)singletonKevin.mapa.grid_.WorldToCell(Logica.transform.position);

        SmartBehaviour.local.Logica.Colonizar(CellPos);

        //Esto hace que se "recarguen" las acciones disponibles.
        MarcarSeleccion.ReiniciarSeleccion();
    }
    public static bool CondicionalColonizar(){
        Vector3Int CellPos = singletonKevin.mapa.grid_.WorldToCell(AdministradorDeUI.CurrentSelect.transform.position);
        if(Mapa.tileMap.HasTile(CellPos) && Mapa.tileMap.GetTile(CellPos).name.Contains("Planeta")) return true;
        else return false;
    }




    public static void Fundadora(){
        Planetas planeta = AdministradorDeUI.CurrentSelect.GetComponent<Planetas>();
        
        SmartBehaviour.local.Logica.CrearNave(SmartBehaviour.local.Civilizacion.Fundadora, planeta.transform.position);

        //Esto hace que se "recarguen" las acciones disponibles.
        MarcarSeleccion.ReiniciarSeleccion();
    }
    public static bool CondicionalNaves(Vector3 Posicion){

        //Si ya hay una nave sobre esa contrucción no se puede spawnear otra nave ahí.
        Collider2D[] colisiones = Physics2D.OverlapCircleAll(Posicion, 0.1f);

        switch(colisiones.GetUpperBound(0) + 1){
            case 0:
            print("No mames viejon tengo miedo");
            return true;
            case 1:
                if(colisiones[0].tag.Contains("Nave")) return false;
                else return true;

            case 2:
                if(colisiones[0].tag.Contains("Nave") || colisiones[1].tag.Contains("Nave")) return false;
                else return true;

            default:
            print("Ea culero checa");
            return false;
        }

    }
    
}