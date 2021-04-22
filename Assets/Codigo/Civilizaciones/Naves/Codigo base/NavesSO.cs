using System.Collections.Generic;
using UnityEngine;

public enum TipoDeNave { CazaEstandar, Interceptor, Acorazado, CañonLaser, Titan, NaveDeLogistica, CentralDeApoyo, Fundadora }
public enum Caracteristicas {  Maniobras, Estrategia, Hostilidad, Camuflaje, Escudos, Cadena, ContraataqueLetal, Reparar, Construccion }

public enum Mejoras { CañonesDobles, MotoresWarp, BlindajeMejorado, TanqueGrande } //Se usará una lista de esto para registrar que mejora ya tiene equipada la nave.
public enum Debilidades { Capturable } 

public abstract class NavesSO : ScriptableObject
{
    public TipoDeNave TipoDeNave_;
    public List<Caracteristicas> Caracteristicas_ = new List<Caracteristicas>();
    public List<Debilidades> Debilidades_ = new List<Debilidades>();
    public string Nombre;
    public int Vida;
    public int Ataque;
    [Range(1,10)]
    public float Defenza;
    [Range(1,90)]
    public int Esquive;
    public int Movilidad;
    public int Alcanze;
    public int Costo;
    
    public SpritesNave Sprites;

    public string GetDescripcion()
    {
        string Descripcion = "";
        switch (TipoDeNave_)
        {
            case TipoDeNave.CazaEstandar:
                Descripcion = "Unidad básica militar.";
                break;
            case TipoDeNave.Interceptor:
                Descripcion = "Movilidad alta y con daño estándar.";
                break;
            case TipoDeNave.Acorazado:
                Descripcion = "Unidad enfocada a la defenza.";
                break;
            case TipoDeNave.CañonLaser:
                Descripcion = "Ataca a largo alcanze.";
                break;
            case TipoDeNave.Titan:
                Descripcion = "Super nave.";
                break;
            case TipoDeNave.NaveDeLogistica:
                Descripcion = "Apoya a las naves aliadas.";
                break;
            case TipoDeNave.CentralDeApoyo:
                Descripcion = "Apoyo de élite en el campo de batalla.";
                break;
            case TipoDeNave.Fundadora:
                Descripcion = "Nave para la exploración y la contrucción.";
                break;
        }
        return Descripcion;
    }
}