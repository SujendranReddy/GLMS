using GLMS.Models;
using System.Net.Http.Json;

namespace GLMS.Services.Api
{
    public class ClientApiService : IClientApiService
    {
        private readonly HttpClient _httpClient;

        public ClientApiService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<List<Client>> GetAllAsync()
        {
            var clients = await _httpClient.GetFromJsonAsync<List<Client>>("api/clients");

            return clients ?? new List<Client>();
        }

        public async Task<Client?> GetByIdAsync(int id)
        {
            return await _httpClient.GetFromJsonAsync<Client>($"api/clients/{id}");
        }

        public async Task<Client?> CreateAsync(Client client)
        {
            var response = await _httpClient.PostAsJsonAsync("api/clients", client);

            if (!response.IsSuccessStatusCode)
            {
                return null;
            }

            return await response.Content.ReadFromJsonAsync<Client>();
        }

        public async Task<bool> UpdateAsync(int id, Client client)
        {
            var response = await _httpClient.PutAsJsonAsync($"api/clients/{id}", client);

            return response.IsSuccessStatusCode;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var response = await _httpClient.DeleteAsync($"api/clients/{id}");

            return response.IsSuccessStatusCode;
        }
    }
}