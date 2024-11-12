using PicketMobile.Views;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace PicketMobile.Services
{
    public class RestClient : HttpClient
    {
        public static string DeviceToken { get; set; }

        public RestClient()//:base(DependencyService.Get<Helpers.IHTTPClientHandlerCreationService>().GetInsecureHandler())
        {
            string _server = "https://picket.ocph23.tech";
            this.BaseAddress = new Uri(_server);
            this.DefaultRequestHeaders.TryAddWithoutValidation("Content-Type", "application/json; charset=utf-8");
            var token = Preferences.Get("token", null);
            if (token != null)
            {
                SetToken(token);
            }
        }

        public RestClient(string apiUrl)
        {
            this.BaseAddress = new Uri(apiUrl);

        }


        public void SetToken(string token)
        {
            if (token != null)
            {
                this.DefaultRequestHeaders.TryAddWithoutValidation("Authorization", token);
                this.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }
        }

     
        public StringContent GenerateHttpContent(object data)
        {
            var json = JsonSerializer.Serialize(data);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            return content;
        }


        public async Task<string> Error(HttpResponseMessage response)
        {
            try
            {
                if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                    return $"'{response.RequestMessage.RequestUri.LocalPath}'  Not Found";

                if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    Application.Current.MainPage = new LoginPage();
                    return $"'{response.RequestMessage.RequestUri.LocalPath}'  Anda Tidak Memiliki Akses !";
                }

                var content = await response.Content.ReadAsStringAsync();
                if (string.IsNullOrEmpty(content))
                    throw new SystemException();

                if (content.Contains("message"))
                {
                    var error = JsonSerializer.Deserialize<ErrorMessage>(content, Helper.JsonOption);
                    return error.Message;
                }
                else if (content.Contains("tools.ietf"))
                {
                    var errors = JsonSerializer.Deserialize<ErrorMessages>(content, Helper.JsonOption);
                    return errors.Title;
                }
                return content;
            }
            catch (Exception)
            {
                return "Maaf Terjadi Kesalahan, Silahkan Ulangi Lagi Nanti";
            }
        }
    }




    public static class RestServiceExtention
    {

        public static async Task<T> GetResult<T>(this HttpResponseMessage response)
        {
            try
            {
                string stringContent = await response.Content.ReadAsStringAsync();
                if (string.IsNullOrEmpty(stringContent))
                    return default;
                var result = JsonSerializer.Deserialize<T>(stringContent, Helper.JsonOption);
                return result;
            }
            catch (Exception ex)
            {
                throw new SystemException(ex.Message);
            }
        }

    }



    public class ErrorMessage
    {
        public string Message { get; set; }
    }

    public class Errors
    {
        public List<string> Email { get; set; }
    }

    public class ErrorMessages
    {
        public string Type { get; set; }
        public string Title { get; set; }
        public int Status { get; set; }
        public string TraceId { get; set; }
        public Errors Errors { get; set; }
    }


}
