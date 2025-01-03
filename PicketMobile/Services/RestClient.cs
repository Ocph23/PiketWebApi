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

                var error = JsonSerializer.Deserialize<ErrorMessage>(content, Helper.JsonOption);
                if (error != null)
                    return string.IsNullOrEmpty(error.Message) ? error.Detail : error.Message;
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
        public static async Task<T> GetResultAsync<T>(this HttpResponseMessage response)
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

    public class Error
    {
        public string Code { get; set; }
        public string Description { get; set; }
        public int Type { get; set; }
        public int NumericType { get; set; }
    }


    public class ErrorMessage
    {
        public string Type { get; set; }
        public string Title { get; set; }
        public string Detail { get; set; }
        public string Message { get; set; }
        public string Instance { get; set; }
        public int Status { get; set; }
        public string TraceId { get; set; }
        public IEnumerable<Error> Errors { get; set; }
        public object Exception { get; set; }
    }


}
