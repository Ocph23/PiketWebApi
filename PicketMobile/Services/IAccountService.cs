﻿using SharedModel.Requests;
using SharedModel.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace PicketMobile.Services
{
    internal interface IAccountService
    {
        Task<bool> Login(string username, string password);

        Task Logout();
    }



    internal class AccountService : IAccountService
    {
        public async Task<bool> Login(string username, string password)
        {
            try
            {
                using var client = new RestClient();
                var response = await client.PostAsJsonAsync("/api/auth/login", new LoginRequest(username, password));
                if (response.IsSuccessStatusCode)
                {
                    var stringContent = await response.Content.ReadAsStringAsync();
                    AuthenticateResponse? data = JsonSerializer.Deserialize<AuthenticateResponse>(stringContent);
                    if (data != null) {
                        Preferences.Set("token", data.Token);
                        Preferences.Set("user", data.UserName);
                        Preferences.Set("email", data.Email);
                        Preferences.Set("roles", JsonSerializer.Serialize(data.roles));
                        Application.Current.MainPage = new AppShell();
                        return true;
                    }
                }
                throw new SystemException(); ;
            }
            catch (Exception)
            {
                throw new SystemException("Maaf, Anda Tidak Memiliki Akses !");
            }
        }

        public Task Logout()
        {
            Preferences.Set("token", null);
            Preferences.Set("user", null);
            Preferences.Set("email", null);
            Preferences.Set("roles", null);
            Application.Current.MainPage = new AppShell();
            return Task.CompletedTask;
        }
    }


}