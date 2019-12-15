using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Windows.Storage;

namespace Well_Podcast.Common
{
    public class DatabaseHelper
    {
        public static T Deserialize<T>(string json)
        {
            var _Bytes = Encoding.Unicode.GetBytes(json);
            using (MemoryStream _Stream = new MemoryStream(_Bytes))
            {
                var _Serializer = new DataContractJsonSerializer(typeof(T));
                return (T)_Serializer.ReadObject(_Stream);
            }
        }

        public static string Serialize(object instance)
        {
            using (MemoryStream _Stream = new MemoryStream())
            {
                var _Serializer = new DataContractJsonSerializer(instance.GetType());
                _Serializer.WriteObject(_Stream, instance);
                _Stream.Position = 0;
                using (StreamReader _Reader = new StreamReader(_Stream))
                { return _Reader.ReadToEnd(); }
            }
        }

        public static string SerializeObject<T>(T dataObject)
        {
            if (dataObject == null)
            {
                return string.Empty;
            }
            try
            {
                using (StringWriter stringWriter = new System.IO.StringWriter())
                {
                    var serializer = new XmlSerializer(typeof(T));
                    serializer.Serialize(stringWriter, dataObject);
                    return stringWriter.ToString();
                }
            }
            catch
            {
                return string.Empty;
            }
        }

        public static T DeserializeObject<T>(string xml)
             where T : new()
        {
            if (string.IsNullOrEmpty(xml))
            {
                return new T();
            }
            try
            {
                using (var stringReader = new StringReader(xml))
                {
                    var serializer = new XmlSerializer(typeof(T));
                    return (T)serializer.Deserialize(stringReader);
                }
            }
            catch
            {
                return new T();
            }
        }

        public static async Task<Windows.Storage.StorageFolder> GetFolder(string fileName)
        {
            var folder = await ApplicationData.Current.RoamingFolder.CreateFolderAsync(fileName, CreationCollisionOption.OpenIfExists);
            return folder;
        }

        public static async Task<Windows.Storage.StorageFile> GetFile(Windows.Storage.StorageFolder storageFolder, string fileName)
        {
            var file = await storageFolder.CreateFileAsync(fileName, CreationCollisionOption.OpenIfExists);
            return file;
        }

        public static async Task<bool> isFolderPresent(string fileName)
        {
            //        try { StorageFile file = await ApplicationData.Current.LocalFolder.GetFileAsync(fileName); }
            //catch { return false; }
            //return true;
            var item = await ApplicationData.Current.RoamingFolder.TryGetItemAsync(fileName);
            return item != null;
        }

        public static async Task<bool> isFilePresent(string fileName)
        {
            //        try { StorageFile file = await ApplicationData.Current.LocalFolder.GetFileAsync(fileName); }
            //catch { return false; }
            //return true;
            var item = await ApplicationData.Current.LocalFolder.TryGetItemAsync(fileName);
            return item != null;
        }

        public static async Task<string> ReadLocalFileAsync(string path)
        {
            try
            {
                var file = await Windows.Storage.StorageFile.GetFileFromApplicationUriAsync(new Uri(path));
                var properties = await file.GetBasicPropertiesAsync();
                var stream = await file.OpenAsync(Windows.Storage.FileAccessMode.Read);
                var reader = new Windows.Storage.Streams.DataReader(stream.GetInputStreamAt(0));
                await reader.LoadAsync((uint)properties.Size);
                return reader.ReadString(reader.UnconsumedBufferLength);
            }
            catch { return null; }
        }

        public static async Task<string> ReadTextFileAsync(string path)
        {
            var folder = ApplicationData.Current.LocalFolder;
            var file = await folder.GetFileAsync(path);
            return await FileIO.ReadTextAsync(file);
        }

        public static async void WriteTotextFileAsync(string fileName, string contents)
        {
            var folder = ApplicationData.Current.LocalFolder;
            var file = await folder.CreateFileAsync(fileName, CreationCollisionOption.ReplaceExisting);
            await FileIO.WriteTextAsync(file, contents);
        }

        public static bool ContainsKey(string keyName)
        {
            return ApplicationData.Current.LocalSettings.Values.ContainsKey(keyName);
        }

        public static void SaveSettings(string key, object contents)
        {
            ApplicationData.Current.LocalSettings.Values[key] = contents;
        }

        public static object LoadSettings(string key)
        {
            var settings = ApplicationData.Current.LocalSettings;
            return settings.Values[key];
        }

        public async static Task<string> GetJsonString(string url)
        {
            var client = new Windows.Web.Http.HttpClient();
            Windows.Web.Http.HttpResponseMessage response = await client.GetAsync(new Uri(url));
            var jsonString = await response.Content.ReadAsStringAsync();
            return jsonString;
        }

        public static string ScrubHtml(string value)
        {
            string step2 = "";
            if (value != null)
            {
                var step1 = System.Text.RegularExpressions.Regex.Replace(value, @"<[^>]+>|&nbsp;", "").Trim();
                step2 = System.Text.RegularExpressions.Regex.Replace(step1, @"\s{2,}", " ");
            }
            return step2;
        }
    }
}
