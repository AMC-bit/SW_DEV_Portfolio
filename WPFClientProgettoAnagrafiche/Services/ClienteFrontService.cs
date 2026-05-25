using ProgettoAnagrafiche.Models;
// shared project dtos + shared project api class
using ProgettoAnagrafiche.Models.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
// grab the api service for making calls
using WPFClientProgettoAnagrafiche.Services.Api;

namespace WPFClientProgettoAnagrafiche.Services
{
    public class ClienteFrontService
    {

        private ApiService apiService;
        private string Endpoint = "api/clienti";

        public ClienteFrontService(ApiService apiService)
        {
            this.apiService = apiService ?? throw new ArgumentNullException(nameof(apiService));
        }

        // GET ALL
        // give me a list of all anagrafica clienti and respective wrappers
        public async Task<ApiResponse<List<AnagraficaCliente>>> GetAllClientiAsync()
            {
            try
                {
                // do this
                var response = await apiService.GetAsync<List<AnagraficaCliente>>(Endpoint);

                // if the response is null, throw exception
                if (response == null)
                    {
                    throw new Exception("API response is null");
                    }

                // if the response failed to begin with, throw exception
                if (!response.Success)
                    {
                    throw new Exception(response.Message ?? "Failed to retrieve clienti from API");
                    }

                // if the response is fine but the data is null, exception
                if (response.Data == null)
                    {
                    throw new Exception("API returned success but data is null");
                    }

                // otherwise,return the response (with wrapper)
                return response;
                }

            // catch network error and miscellaneous unexpected stuff
            catch (HttpRequestException ex)
                {
                throw new Exception($"Network error while retrieving clienti: {ex.Message}", ex);
                }
            catch (Exception ex)
                {
                throw new Exception($"Unexpected error while retrieving clienti: {ex.Message}", ex);
                }
            }

        // GET ONE

        public async Task<ApiResponse<AnagraficaCliente>> GetClienteByIDAsync(string id)
        {
            try
            {
                // do this
                var response = await apiService.GetAsync<AnagraficaCliente>($"{Endpoint}/{id}");

                if (response == null)
                {
                    throw new Exception("API response is null");
                }
                if (!response.Success)
                {
                    throw new Exception(response.Message ?? $"Failed to retrieve cliente with ID {id} from API");
                }
                if (response.Data == null)
                {
                    throw new Exception($"API response data is null for cliente with ID {id}");
                }
                return response;
            }
            catch (Exception ex)
            {
                throw new Exception($"An error occurred while retrieving cliente with ID {id}: {ex.Message}", ex);
            }
        }

        // POST
        public async Task<ApiResponse<AnagraficaCliente>> CreateClienteAsync(AnagraficaCliente newClient)
        {
            try
            {
                // take in the dto I feed you and post it to the api, return the response as a ClienteDTO
                var response = await apiService.PostAsync<AnagraficaCliente>(Endpoint, newClient);

                if (response == null)
                {
                    throw new Exception("API response is null");
                }
                if (!response.Success)
                {
                    throw new Exception(response.Message ?? "Failed to create cliente via API");
                }
                //if (response.Data == null)
                //{
                //    throw new Exception("API response data is null after creating cliente");
                //}
                return response;
            }
            catch (Exception ex)
            {
                throw new Exception($"An error occurred while creating cliente: {ex.Message}", ex);
            }
        }

        // PUT

        public async Task<ApiResponse<AnagraficaCliente>> UpdateClienteAsync(int id, AnagraficaCliente updatedClient)
        {
            try
            {
                // no more dto, directly get the entity wrapped in its response
                var response = await apiService.PutAsync<AnagraficaCliente>($"{Endpoint}/{id}", updatedClient);

                // if there's no response, exception
                if (response == null)
                {
                    throw new Exception("API response is null");
                }
                // if it failed, other excpt
                if (!response.Success)
                {
                    throw new Exception(response.Message ?? $"Failed to update cliente with ID {id} via API");
                }

                // otherwise ok, return the response&wrap
                return response;
            }
            catch (Exception ex)
            {
                throw new Exception($"An error occurred while updating cliente with ID {id}: {ex.Message}", ex);
            }
        }

        // no more bool
        public async Task<ApiResponse> DeleteClienteAsync(int id)
        {
            try
            {
                string endpoint = $"{Endpoint}/{id}";
                var response = await apiService.DeleteAsync(endpoint);

                // failed, except
                if (!response.Success)
                {
                    throw new Exception($"Failed to delete cliente with ID {id} via API");
                }

                // else return with wrapper
                return response;
            }
            catch (Exception ex)
            {
                throw new Exception($"An error occurred while deleting cliente with ID {id}: {ex.Message}", ex);
            }

        }
    }
}