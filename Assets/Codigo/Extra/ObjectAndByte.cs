using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

public static class ObjectAndByte
{
    public static byte[] ObjectABytes(object objecto)
    {
        if (objecto == null) return null;

        BinaryFormatter bf = new BinaryFormatter();
        MemoryStream Ms = new MemoryStream();

        bf.Serialize(Ms, objecto);
        return Ms.ToArray();
    }

    public static object BytesAObject(byte[] _byteArray)
    {
        BinaryFormatter bf = new BinaryFormatter();
        MemoryStream Ms = new MemoryStream(_byteArray);

        return bf.Deserialize(Ms);
    }

}
