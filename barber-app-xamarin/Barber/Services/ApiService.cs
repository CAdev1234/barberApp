using System;
using System.Text;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.IO;
using System.Net.Http.Headers;
using Plugin.Connectivity;

namespace Barber.Services
{
    class ApiService
    {
        //public const string SERVER_HOST = "http://192.168.109.71:80";
        public const string SERVER_HOST = "http://barbers.amz.mpo.mybluehost.me";
        public static async Task<T> GetOneWithPost<T, T1>(T1 data, string action)
        {
            var client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", SharedService.GetUser().Token);
            
            var json = JsonConvert.SerializeObject(data);
            var dataConverted = new StringContent(json, Encoding.UTF8, "application/json");
            var url = $"{SERVER_HOST}/api/{action}";
            var content = "";

            try
            {
                if(CrossConnectivity.Current.IsConnected)
                {
                    var response = await client.PostAsync(url, dataConverted);
                    content = await response.Content.ReadAsStringAsync();
                    var obj = JsonConvert.DeserializeObject<T>(content);
                    System.Diagnostics.Debug.WriteLine("===content===" + content);
                    return obj;
                }
                else
                {
                    HandleToast.Toast("Please check your internet connection.");
                    return default(T);
                }
            }
            catch (Exception e)
            {
                var error = e.Message;
                System.Diagnostics.Debug.WriteLine("===URL===" + url);
                System.Diagnostics.Debug.WriteLine("===json===" + json);
                System.Diagnostics.Debug.WriteLine("===content===" + content);
                System.Diagnostics.Debug.WriteLine("===error===" + error);
                return default(T);
            }
        }

        public static async Task<T> GetOneWithGet<T>(string action)
        {
            var client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", SharedService.GetUser().Token);
            var url = $"{SERVER_HOST}/api/{action}";
            var content = "";
            try
            {
                if (CrossConnectivity.Current.IsConnected)
                {
                    var response = await client.GetAsync(url);
                    content = await response.Content.ReadAsStringAsync();
                    var obj = JsonConvert.DeserializeObject<T>(content);
                    return obj;
                }
                else
                {
                    HandleToast.Toast("Please check your internet connection.");
                    return default(T);
                }
            }
            catch (Exception e)
            {
                var error = e.Message;
                System.Diagnostics.Debug.WriteLine("===URL===" + url);
                System.Diagnostics.Debug.WriteLine("===content===" + content);
                System.Diagnostics.Debug.WriteLine("===error===" + error);
                return default(T);
            }
        }

        public static async Task<T> uploadPhotoWithPost<T>(string action, string filename)
        {
            var client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", SharedService.GetUser().Token);
            var url = $"{SERVER_HOST}/api/{action}";

            try
            {
                if (CrossConnectivity.Current.IsConnected)
                {
                    var upfilebytes = File.ReadAllBytes(filename);

                    MultipartFormDataContent content = new MultipartFormDataContent();
                    ByteArrayContent baContent = new ByteArrayContent(upfilebytes);
                    content.Add(baContent, "File", "uploadImage.png");

                    var response = await client.PostAsync(url, content);
                    var strAsyncContent = await response.Content.ReadAsStringAsync();
                    var obj = JsonConvert.DeserializeObject<T>(strAsyncContent);
                    return obj;
                }
                else
                {
                    HandleToast.Toast("Please check your internet connection.");
                    return default(T);
                }
            }
            catch (Exception e)
            {
                string error = e.Message;
                return default(T);
            }
        }
    }
}