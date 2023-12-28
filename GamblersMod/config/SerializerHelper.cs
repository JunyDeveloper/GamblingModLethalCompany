using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace GamblersMod.config
{
    internal class SerializerHelper
    {
        public static byte[] GetSerializedSettings<T>(T valToSerialize)
        {
            MemoryStream stream = new MemoryStream();
            BinaryFormatter formatter = new BinaryFormatter();

            try
            {
                formatter.Serialize(stream, valToSerialize);
            }
            catch (SerializationException e)
            {
                Plugin.mls.LogError("Config serialization failed: " + e.Message);
            }
            byte[] bytes = stream.ToArray();
            stream.Close();

            return bytes;
        }

        public static T GetDeserializedSettings<T>(byte[] settingsAsBytes)
        {
            MemoryStream stream = new MemoryStream();
            stream.Write(settingsAsBytes, 0, settingsAsBytes.Length);
            stream.Seek(0, SeekOrigin.Begin);
            BinaryFormatter formatter = new BinaryFormatter();
            object o;
            try
            {
                o = formatter.Deserialize(stream);
                stream.Close();
                return (T)o;
            }
            catch (SerializationException e)
            {
                Plugin.mls.LogError("Config deserialization failed: " + e.Message);
            }

            stream.Close();
            return default;
        }
    }
}
