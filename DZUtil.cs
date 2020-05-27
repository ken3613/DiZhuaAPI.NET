using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace DiZhuaAPI.NET
{
    public class DZUtil
    {
        public string JWT { get; }
        public DZUtil(string jwt)
        {
            this.JWT = jwt;
        }


        
        /// <summary>
        /// ABCddd
        /// </summary>
        /// <param name="mobile"></param>
        /// <returns></returns>
        public static async Task<bool> VerificationAsync(string mobile)
        {
            string url = "https://apis-ff.zaih.com/flash-auth/v2/mobile/verification";
            var client = new HttpClient();
            client.DefaultRequestHeaders.Add("User-Agent", "ios dizhuaApp 1.9.4");
            client.DefaultRequestHeaders.Add("Host", "apis-ff.zaih.com");
            client.DefaultRequestHeaders.Add("Authorization", "Basic aW9zOmtvcWVyMjFvaGFmZG8xNDA5YXNkZmU=");
            var requestClass = new
            {
                mobile = mobile
            };
            string requestJson = JsonConvert.SerializeObject(requestClass);
            HttpContent requestContent = new StringContent(requestJson);
            requestContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");
            var response = await client.PostAsync(url, requestContent);
            if (response.IsSuccessStatusCode)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// 请求登录
        /// </summary>
        /// <param name="mobile">手机号</param>
        /// <param name="code">验证码</param>
        /// <returns>JWT Token</returns>
        public async static Task<string> LoginAsync(string mobile, string code)
        {
            string url = "https://apis-ff.zaih.com/flash-auth/v2/oauth/jwt";
            var client = new HttpClient();
            client.DefaultRequestHeaders.Add("User-Agent", "ios dizhuaApp 1.9.4");
            client.DefaultRequestHeaders.Add("Host", "apis-ff.zaih.com");
            client.DefaultRequestHeaders.Add("Authorization", "Basic aW9zOmtvcWVyMjFvaGFmZG8xNDA5YXNkZmU=");
            var requestClass = new
            {
                username = mobile,
                password = code,
                auth_approach = "mobile",
                grant_type = "password"
            };
            string requestJson = JsonConvert.SerializeObject(requestClass);
            HttpContent requestContent = new StringContent(requestJson);
            requestContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");
            var response = await client.PostAsync(url, requestContent);
            if (response.IsSuccessStatusCode)
            {
                var responseJson = response.Content.ReadAsStringAsync().Result;
                return JsonConvert.DeserializeObject<JObject>(responseJson)["access_token"].ToString();
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// 获取爪友列表
        /// </summary>
        /// <returns>爪友列表</returns>
        public List<Friend> GetRelationships()
        {
            string url = "https://apis-ff.zaih.com/flash-whisper/v3/relationships/all";
            var client = new HttpClient();
            client.DefaultRequestHeaders.Add("User-Agent", "ios dizhuaApp 1.9.4");
            client.DefaultRequestHeaders.Add("Host", "apis-ff.zaih.com");
            client.DefaultRequestHeaders.Add("Authorization", String.Format("JWT {0}", JWT));
            List<Friend> friends_list = new List<Friend>();
            var response = client.GetAsync(url).Result;
            var json = response.Content.ReadAsStringAsync().Result;
            var friends = JsonConvert.DeserializeObject<JArray>(json);
            foreach (var friend in friends)
            {
                var fd = new Friend
                {
                    Close_grade = friend["close_grade"].ToObject<float>(),
                    User_id = friend["to_user"]["user_id"].ToString(),
                    Nickname = friend["to_user"]["nickname"].ToString()
                };
                friends_list.Add(fd);
            }
            return friends_list;
        }
    }
}
