using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public enum TipoDeInteraccion { Atacar, Reparar, ContraAtacar }

public class Interacciones : ScriptableObject
{
    static float BonoDeDefenza = 1.4f;

    public static void Interactuar(InfoDeNave NaveSeleccionada, InfoDeNave NaveObjetivo, TipoDeInteraccion Interaccion)
    {
        if (Interaccion == TipoDeInteraccion.Atacar) singletonKevin.mapa.StartCoroutine(Atacar(NaveSeleccionada, NaveObjetivo, false));
        else if (Interaccion == TipoDeInteraccion.ContraAtacar) singletonKevin.mapa.StartCoroutine(Atacar(NaveSeleccionada, NaveObjetivo, true));
    }


    static IEnumerator Atacar(InfoDeNave NaveAtacante, InfoDeNave NaveObjetivo, bool ContraAtacando)
    {

        #region Instanciar y mover laser.
        GameObject Laser = new GameObject(); //Instancia un prefab
        Laser.transform.position = NaveAtacante.transform.position; //Posicion
        Laser.transform.localScale = new Vector3(.25f, 1.25f, 1);//Escala
        //Imagen
        Laser.AddComponent<SpriteRenderer>().sprite = NaveAtacante.TipoDeNaveSO.Sprites.Disparo;
        //Angulo
        Vector3 direccion = NaveObjetivo.transform.position - NaveAtacante.transform.position;
        //Aquí cambiar direccion de la nave al disparar.
        float angle = Mathf.Atan2(direccion.y, direccion.x) * Mathf.Rad2Deg;
        Quaternion rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        Laser.transform.rotation = rotation;
        #region "Retroceso" de disparo.
        LeanTween.move(NaveAtacante.gameObject, (-direccion.normalized)/6 + NaveAtacante.transform.position, .1f).setEase(LeanTweenType.easeShake);
        #endregion
        Vector3Int NaveAtacanteEnGrid = singletonKevin.mapa.grid_.WorldToCell(NaveAtacante.transform.position);
        Vector3Int NaveObjetivoEnGrid = singletonKevin.mapa.grid_.WorldToCell(NaveObjetivo.transform.position);

        NaveAtacante.Logica.CambiarDireccion(NaveAtacanteEnGrid, NaveObjetivoEnGrid);

        Vector3 NaveObjetivoPos = NaveObjetivo.transform.position;

        while (Vector3.Distance(NaveObjetivoPos, Laser.transform.position) > .15f)
        {
            Laser.transform.position += (direccion.normalized * 0.2f);
            yield return new WaitForFixedUpdate();
        }
        Destroy(Laser); //Desde que inicia hasta aquí, son visuales.
        #endregion

        #region Lógica.
        

        //Después de atacar, si la nave no tiene "Estrategia" entonces ya no le quedará movimiento.
        if (!NaveAtacante.TipoDeNaveSO.Caracteristicas_.Contains(Caracteristicas.Estrategia)) NaveAtacante.MovimientoDisponible = 0;


        //Continuamos, si la nave sigue existiendo.
        if (NaveObjetivo == null) yield break;
        //Calcular daño.
        int Daño = NaveAtacante.TipoDeNaveSO.Ataque - Mathf.FloorToInt(Mathf.Clamp(NaveObjetivo.TipoDeNaveSO.Defenza * BonoDeDefenza, 1, 999));
        //Si se tiene autoridad sobre el objeto.
        if(NaveObjetivo.hasAuthority) //Aplica daño.
        NaveObjetivo.Logica.CmdAlterarVida(NaveAtacante.transform,  -Daño);


        //Si estamos ContraAtacando, fuera de aquí.
        if (ContraAtacando) yield break; 
        //Si no, espera para que el enemigo contraataque. 
        else yield return new WaitForSecondsRealtime(.5f);

        //Si la nave a muerto, fuera.
        if (NaveObjetivo == null) yield break; 
        //Iniciamos el coontraataque (de la nave enemiga).
        if(NaveObjetivo.hasAuthority)                                                                   //[[Creo que todo el resto del codigo debería "depender" de este "if"]]
        NaveObjetivo.Logica.CmdInteractuar(NaveAtacante.gameObject, TipoDeInteraccion.ContraAtacar);

        //Esperar para luego volver a seleccionar la nave si queda movimiento o ataque disponible.
        yield return new WaitForSecondsRealtime(.5f);
        
        //Si la nave atacante tiene movimiento y hay otra nave enemiga al alcance, selecciona de nuevo.
        if (NaveAtacante.MovimientoDisponible > 0 || MostrarNodos.NavesEnRango(NaveAtacante, NaveAtacante.TipoDeNaveSO.Alcanze).Count == 0)
            NaveAtacante.Logica.MostrarNodos();
        else if (NaveAtacante.MovimientoDisponible <= 0)
            MarcarSeleccion.ReiniciarSeleccion();
        
        #endregion
    }
}