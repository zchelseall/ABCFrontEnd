using System;
using System.Text;
using System.Security.Cryptography;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Net;
using Newtonsoft.Json;

namespace ABCSolutionsWPF
{
    class QueryParams
    {
        [JsonProperty("func")]
        public string Function { get; set; }
        [JsonProperty("params")]
        public List<string> Parameters { get; set; }
    }


    class InvokeParams
    {
        [JsonProperty("func")]
        public string Function { get; set; }
        [JsonProperty("params")]
        public List<string> Parameters { get; set; }
        [JsonProperty("account")]
        public string Account { get; set; }

    }

    class ReturnValue
    {
        public Boolean success { get; set; }
        public List<string> payloads { get; set; }
    }


    public class BlockChainClient
    {
        public BlockChainClient(string ChainName, string ChainCodeName, string APIKey)
        {
            ChainName_ = ChainName;
            ChainCodeName_ = ChainCodeName;
            APIKey_ = APIKey;
        }

        public static HttpWebResponse CreatePostHttpResponse(string url, byte[] body, int timeout, CookieCollection cookies)
        {
            HttpWebRequest request = null;
            if (url.StartsWith("https", StringComparison.OrdinalIgnoreCase))
            {
                request = WebRequest.Create(url) as HttpWebRequest;
            }
            else
            {
                request = WebRequest.Create(url) as HttpWebRequest;
            }
            request.Method = "POST";
            request.ContentType = "application/json";

            //设置代理UserAgent和超时
            //request.UserAgent = userAgent;
            //request.Timeout = timeout; 

            if (cookies != null)
            {
                request.CookieContainer = new CookieContainer();
                request.CookieContainer.Add(cookies);
            }
            //发送POST数据  
            using (Stream stream = request.GetRequestStream())
            {
                stream.Write(body, 0, body.Length);
            }

            string[] values = request.Headers.GetValues("Content-Type");
            return request.GetResponse() as HttpWebResponse;

        }

        public string sendTranscript(string account, string key, string value)
        {
            var url = string.Format("https://baas.ink.plus/public-api/call/{0}/{1}/invoke?apikey={2}",
                ChainName_, ChainCodeName_, APIKey_);
            var param = new InvokeParams();
            param.Account = account;
            param.Parameters = new List<String> { key, value };
            // param.Function = "newTranscript";
            param.Function = "addRecord";

            var body = Encoding.ASCII.GetBytes(JsonConvert.SerializeObject(param));
            var s = Encoding.ASCII.GetString(body);
            var response = CreatePostHttpResponse(url, body, 1, null);
            var responseString = new StreamReader(response.GetResponseStream()).ReadToEnd();
            return responseString;
        }

        public string queryTranscript(string account, string key)
        {
            var url = string.Format("https://baas.ink.plus/public-api/call/{0}/{1}/query?apikey={2}",
                ChainName_, ChainCodeName_, APIKey_);
            var param = new QueryParams();
            // param.Parameters = new List<String> { account, key };
            // param.Function = "queryTranscript";
            param.Parameters = new List<String> { key };
            param.Function = "queryRecord";

            var jsonString = JsonConvert.SerializeObject(param);
            var body = Encoding.ASCII.GetBytes(jsonString);
            var response = CreatePostHttpResponse(url, body, 1, null);
            string responseString = new StreamReader(response.GetResponseStream()).ReadToEnd();
            var ret = (ReturnValue)JsonConvert.DeserializeObject(responseString, typeof(ReturnValue));
            return ret.payloads[0];
        }

        private static BlockChainClient instance = null;

        public static BlockChainClient GetInstance()
        {
            if (instance == null)
            {
                instance = new BlockChainClient("acdb", "Asset5", "5ae3f829292d37001232430a");
            }
            return instance;
        }

        private string ChainName_;
        private string ChainCodeName_;
        private string APIKey_;
    }

    public class Crypto
    {
        public static byte[] pad(byte[] data)
        {
            int pad_length = (data.Length + 15) / 16 * 16;
            byte[] pad_data = new byte[pad_length];
            data.CopyTo(pad_data, 0);
            return pad_data;
        }

        public static string Encode(byte[] data, out string skey)
        {
            var aes = Aes.Create();
            byte[] key = aes.Key.Take(16).ToArray();
            skey = Convert.ToBase64String(key);
            byte[] iv = aes.IV.Take(16).ToArray();
            byte[] pad_data = pad(data);
            List<byte> cipher;

            using (RijndaelManaged Aes128 = new RijndaelManaged())
            {
                Aes128.BlockSize = 128;
                Aes128.KeySize = 128;
                Aes128.Mode = CipherMode.CFB;
                Aes128.FeedbackSize = 128;
                Aes128.Padding = PaddingMode.None;
                Aes128.Key = key;
                Aes128.IV = iv;

                using (var encryptor = Aes128.CreateEncryptor())
                using (var msEncrypt = new MemoryStream())
                using (var csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                using (var bw = new BinaryWriter(csEncrypt, Encoding.UTF8))
                {
                    bw.Write(pad_data);
                    bw.Close();

                    cipher = iv.ToList();
                    cipher.AddRange(msEncrypt.ToArray().Take(data.Length));
                    Console.WriteLine("{0} {1}", data.Count(), cipher.Count());

                    return System.Convert.ToBase64String(cipher.ToArray());
                }
            }
        }

        public static byte[] Decode(string bcipher, string bkey)
        {
            byte[] cipher = Convert.FromBase64String(bcipher);
            byte[] iv = cipher.Take(16).ToArray();
            cipher = cipher.Skip(16).ToArray();
            byte[] key = Convert.FromBase64String(bkey);
            using (RijndaelManaged Aes128 = new RijndaelManaged())
            {
                Aes128.BlockSize = 128;
                Aes128.KeySize = 128;
                Aes128.Mode = CipherMode.CFB;
                Aes128.FeedbackSize = 128;
                Aes128.Padding = PaddingMode.None;

                Aes128.Key = key;
                Aes128.IV = iv;

                using (var decryptor = Aes128.CreateDecryptor())
                using (var msEncrypt = new MemoryStream())
                using (var csEncrypt = new CryptoStream(msEncrypt, decryptor, CryptoStreamMode.Write))
                using (var bw = new BinaryWriter(csEncrypt, Encoding.UTF8))
                {
                    bw.Write(pad(cipher));
                    bw.Close();

                    return msEncrypt.ToArray().Take(cipher.Length).ToArray();
                }
            }
        }

        static void fakemain()
        {
            byte[] data = Encoding.UTF8.GetBytes("0123456789abcdefggg");

            Console.WriteLine("original " + BitConverter.ToString(data));
            string bkey;
            string bcipher = Encode(data, out bkey);

            Console.WriteLine("{0} {1}", bcipher, bkey);

            data = Decode(bcipher, bkey);
            Console.WriteLine("original " + BitConverter.ToString(data));
        }
    }
}