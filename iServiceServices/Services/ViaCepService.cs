using iServiceServices.Services.Models;
using System.Text.Json;

namespace iServiceServices.Services
{
    public class ViaCepService
    {
        public async Task<Result<ViaCep>> GetByCep(string cep)
        {
            try
            {
                var cepFormat = UtilService.CleanString(cep);

                var client = new HttpClient();

                var request = new HttpRequestMessage(HttpMethod.Get, $"https://viacep.com.br/ws/{cepFormat}/json");

                var response = await client.SendAsync(request);

                if (response.IsSuccessStatusCode == false)
                {
                    throw new Exception();
                }

                var jsonResponse = await response.Content.ReadAsStringAsync();

                var viaCep = JsonSerializer.Deserialize<ViaCep>(jsonResponse, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                return Result<ViaCep>.Success(viaCep);
            }
            catch (Exception ex)
            {
                return Result<ViaCep>.Failure($"Falha ao obter os perfis de cliente: {ex.Message}");
            }
        }
    }
}
