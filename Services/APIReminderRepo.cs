

using Reminder_WPF.Models;
using Reminder_WPF.Utilities;

using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Threading.Tasks;


namespace Reminder_WPF.Services
{   
    
    public class APIReminderRepo 
    {
        private readonly string? _token;
        private readonly string? _url;
        private readonly int _port;

        public APIReminderRepo(string url, string token, int port = 80)
        {
            _token = token;
            _url = url;
            _port = port;
        }

        public async Task<Result<List<Reminder>>> GetRemindersAsync()
        {
            try
            {
                using (var client = GetClient())
                {
                    var result = await client.GetAsync("Reminders");
                    result.EnsureSuccessStatusCode();
                    var data = await result.Content.ReadFromJsonAsync<List<Reminder>>();
                    return Result.Ok(data?? new List<Reminder>());
                }
            }
            catch (HttpRequestException e)
            {
                
                return Result.Fail<List<Reminder>>(e.Message);
            }

        }

        public async Task<Result<Reminder?>> GetReminderByIdAsync(int id)
        {
            try
            {
                using var client = GetClient();
                var result = await client.GetAsync($"reminders/{id}");
                result.EnsureSuccessStatusCode();
                var reminder = await result.Content.ReadFromJsonAsync<Reminder?>();
                return Result.Ok(reminder);
            }
            catch (HttpRequestException e)
            {
                
                return Result.Fail<Reminder?>(e.Message);
            }
        }

        public async Task<Result<Reminder?>> AddReminderAsync(Reminder item)
        {
            try
            {
                using var client = GetClient();
                var result = await client.PostAsJsonAsync<Reminder>($"reminders", item);
                result.EnsureSuccessStatusCode();
                var reminder = await result.Content.ReadFromJsonAsync<Reminder>();
                return Result.Ok(reminder);              
               
            }
            catch (HttpRequestException e)
            {
                
                return Result.Fail<Reminder?>(e.Message);
            }

        }

        public async Task<Result> DeleteReminderAsync(Reminder item)
        {
            try
            {
                using var client = GetClient();
                var result = await client.DeleteAsync($"reminders/{item.id}");
                result.EnsureSuccessStatusCode();
                return Result.Ok();
            }
            catch (HttpRequestException e)
            {
               
                return Result.Fail(e.Message);
            }
        }

        public async Task<Result> UpdateReminderAsync(Reminder item)
        {
            try
            {
                using var client = GetClient();
                var result = await client.PutAsJsonAsync($"reminders/{item.id}",item);
                result.EnsureSuccessStatusCode();
                return Result.Ok();
            }
            catch (HttpRequestException e)
            {
               
                return Result.Fail(e.Message);
            }
        }

        
        private HttpClient GetClient()
        {
            var client = new HttpClient();
            client.BaseAddress = new Uri($"{_url}:{_port}");
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", _token);
            return client;
        }

        public static async Task<string?> GetToken(string _username, string _password, string url, int port = 80 )
        {
            using (var client = new HttpClient()) 
            {
                client.BaseAddress = new Uri($"{url}:{port}");
                var login = new LoginModel { UserName = _username, password = _password };
                var r = await client.PostAsJsonAsync<LoginModel>("Account/login", login);
                if (r.IsSuccessStatusCode)
                {
                    var token = await r.Content.ReadFromJsonAsync<LoginResponse>();
                    if(token != null)
                    {
                        return token.token;
                    }
                } 
                return null;                   
            }
        }
    }
}
