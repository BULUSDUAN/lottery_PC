using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.IO;
using System.Text;

using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace ChinaRailway.Common
{
    public class DataSerializer
    {
        protected readonly Newtonsoft.Json.JsonSerializer serializer;

        public DataSerializer()
        {
            serializer = Newtonsoft.Json.JsonSerializer.CreateDefault();
        }

        public DataSerializer(JsonSerializerSettings settings)
        {
            serializer = Newtonsoft.Json.JsonSerializer.Create(settings);
        }

        public byte[] Serialize<TObject>(TObject obj)
        {
            using (var ms = new MemoryStream())
            using (var sw = new StreamWriter(ms))
            using (var writer = new JsonTextWriter(sw))
            {
                serializer.Serialize(writer, obj);
                writer.Flush();

                return ms.ToArray();
            }
        }

        public byte[] Serialize(Type type, object obj)
        {
            using (var ms = new MemoryStream())
            using (var sw = new StreamWriter(ms))
            using (var writer = new JsonTextWriter(sw))
            {
                serializer.Serialize(writer, obj);
                writer.Flush();

                return ms.ToArray();
            }
        }

        public TObject Deserialize<TObject>(byte[] data)
        {
            using (var ms = new MemoryStream(data))
            using (var sr = new StreamReader(ms))
            using (var reader = new JsonTextReader(sr))
            {
                return serializer.Deserialize<TObject>(reader);
            }
        }

        public object Deserialize(Type type, byte[] data)
        {
            using (var ms = new MemoryStream(data))
            using (var sr = new StreamReader(ms))
            using (var reader = new JsonTextReader(sr))
            {
                return serializer.Deserialize(reader, type);
            }
        }

        public Task<byte[]> SerializeAsync<TObject>(TObject obj)
        {
            var result = new TaskCompletionSource<byte[]>();

            try
            {
                var Result = Serialize<TObject>(obj);
                result.SetResult(Result);
            }
            catch (Exception ex)
            {
                result.SetException(ex);
            }

            return result.Task;
        }

        public Task<byte[]> SerializeAsync(Type type, object obj)
        {
            var result = new TaskCompletionSource<byte[]>();

            try
            {
                var Result = Serialize(type, obj);
                result.SetResult(Result);
            }
            catch (Exception ex)
            {
                result.SetException(ex);
            }

            return result.Task;
        }

        public Task<TObject> DeserializeAsync<TObject>(byte[] data)
        {
            var result = new TaskCompletionSource<TObject>();

            try
            {
                var Result = Deserialize<TObject>(data);
                result.SetResult(Result);
            }
            catch (Exception ex)
            {
                result.SetException(ex);
            }

            return result.Task;
        }

        public Task<object> DeserializeAsync(Type type, byte[] data)
        {
            var result = new TaskCompletionSource<object>();

            try
            {
                var Result = Deserialize(type, data);
                result.SetResult(Result);
            }
            catch (Exception ex)
            {
                result.SetException(ex);
            }

            return result.Task;
        }

        public string SerializeString<TObject>(TObject obj)
        {
            return Encoding.UTF8.GetString(Serialize<TObject>(obj));
        }

        public TObject DeserializeString<TObject>(string data)
        {
            return Deserialize<TObject>(Encoding.UTF8.GetBytes(data));
        }

        public object DeserializeString(Type type, string data)
        {
            return Deserialize(type, Encoding.UTF8.GetBytes(data));
        }

        public Task<string> SerializeStringAsync<TObject>(TObject obj)
        {
            var result = new TaskCompletionSource<string>();

            try
            {
                var Result = SerializeString<TObject>(obj);
                result.SetResult(Result);
            }
            catch (Exception ex)
            {
                result.SetException(ex);
            }

            return result.Task;
        }

        public Task<TObject> DeserializeStringAsync<TObject>(string data)
        {
            var result = new TaskCompletionSource<TObject>();

            try
            {
                var Result = DeserializeString<TObject>(data);
                result.SetResult(Result);
            }
            catch (Exception ex)
            {
                result.SetException(ex);
            }

            return result.Task;
        }

        public Task<object> DeserializeStringAsync(Type type, string data)
        {
            var result = new TaskCompletionSource<object>();

            try
            {
                var Result = DeserializeString(type, data);
                result.SetResult(Result);
            }
            catch (Exception ex)
            {
                result.SetException(ex);
            }

            return result.Task;
        }
    }
}
