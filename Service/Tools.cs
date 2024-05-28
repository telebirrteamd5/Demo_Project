using System.Security.Cryptography;
using System.Text;

namespace Demo_Project.Service
{
    public class Tools
    {
        public static string createTimeStamp()
        {
            DateTimeOffset now = (DateTimeOffset)DateTime.UtcNow;
            return now.ToUnixTimeSeconds().ToString();
        }
        public static string createNonceStr()
        {
            string[] chars = { "0", "1", "2", "3", "4", "5", "6", "7", "8", "9", "a", "b", "c", "d", "e", "f", "g", "h", "i", "j", "k", "l", "m", "n", "o", "p", "q", "r", "s", "t", "u", "v", "w", "x", "y", "z" };
            string str = "";
            for (int i = 0; i < 32; i++)
            {
                Random random = new Random();
                int index = random.Next(32);
                str = str + chars[index];
            }
            return str;
        }
        public static string sign(Dictionary<string,object> request)
        {
            string private_key = Config.Config.private_key;
            string[] exclude_fields = { "sign", "sign_type", "header", "refund_info", "openType", "raw_request" };
            string[] requestJoin = {};
            Dictionary<string, object>.KeyCollection keys = request.Keys;
            foreach (string key in keys)
            {
                if (key.Contains(exclude_fields[0]) || key.Contains(exclude_fields[1]) || key.Contains(exclude_fields[2]) || key.Contains(exclude_fields[3]) || key.Contains(exclude_fields[4]) || key.Contains(exclude_fields[5]))
                {
                    continue;
                }
                if (key == "biz_content")
                {
                    Dictionary<string,string> biz_content = (Dictionary<string, string>)request["biz_content"];
                    Dictionary<string, string>.KeyCollection biz_keys = biz_content.Keys;

                    foreach (string k in biz_keys)
                    {
                        requestJoin =requestJoin.Append(k + "=" + biz_content[k]).ToArray();
                    }
                }
                else
                {
                    requestJoin = requestJoin.Append(key + "=" + request[key]).ToArray();
                }
            }
            Array.Sort(requestJoin);
            int index = 0; 
            string[] temp = requestJoin.ToArray();
            foreach (string key in temp)
            {
                Console.WriteLine(key + " index " + index);
                index++;
            }
            if (temp[7]!=null && temp[8]!=null && temp[7].Contains("payee_identifier_type") && temp[8].Contains("payee_identifier"))
            {
            temp[7] = requestJoin[8];
            temp[8] = requestJoin[7];
            }
            else
            {
                return "payee_identifier _type and payee_identifier error check the position and do the swap accordingly!";
            }
            string sorted = string.Join("&", temp);
            string signed = SignTool(sorted,private_key);
            return signed;
            

        }
        public static string SignTool(string data, string privateKey)
        {
            using (var rsa = RSA.Create())
            {
                //byte[] privateKeyBytes = Convert.FromBase64String(privateKey);
                rsa.ImportFromPem(Config.Config.private_key.ToCharArray());
                var dataToSign = Encoding.UTF8.GetBytes(data);
                var signature = rsa.SignData(dataToSign, HashAlgorithmName.SHA256, RSASignaturePadding.Pss);
                return Convert.ToBase64String(signature);
            }
        }
    }
}
