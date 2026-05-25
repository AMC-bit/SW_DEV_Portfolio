using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// grab the api service for making calls
using WPFClientProgettoAnagrafiche.Services.Api;

// shared project dtos + shared project api class
using ProgettoAnagrafiche.Models.DTO;
using ProgettoAnagrafiche.Models;

namespace WPFClientProgettoAnagrafiche.Services
{
    public class ContattoFrontService
    {

        private ApiService apiService;
        private string Endpoint = "api/contatti";

        // store my errors for later display
        public List<string> Errors = new List<string>();

        public ContattoFrontService(ApiService apiService)
        {
            this.apiService = apiService ?? throw new ArgumentNullException(nameof(apiService));
        }


        // same as clientefrontservice

        // GET ALL
        // grab all contatti and return them as a list of ContattoDTO
        public async Task<ApiResponse<List<AnagraficaContatto>>> GetAllContattiAsync()
        {
            try
            {
                // do this
                var response = await apiService.GetAsync<List<AnagraficaContatto>>(Endpoint);

                // if the response is null, throw exception
                if (response == null)
                {
                    throw new Exception("API response is null");
                }

                // if the response failed to begin with, throw exception
                if (!response.Success)
                {
                    throw new Exception(response.Message ?? "Failed to retrieve contatti from API");
                }

                // if the response is fine but the data is null, exception
                if (response.Data == null)
                {
                    throw new Exception("API response data is null");
                }

                // otherwise,return the data (list of ContattoDTO)
                return response;
            }

            // last catch for anything unexpected
            catch (Exception ex)
            {
                throw new Exception($"An error occurred while retrieving contatti: {ex.Message}", ex);
            }
        }

        // GET ONE

        public async Task<ApiResponse<AnagraficaContatto>> GetContattoByIDAsync(string id)
        {
            try
            {
                // do this
                var response = await apiService.GetAsync<AnagraficaContatto>($"{Endpoint}/{id}");
                if (response == null)
                {
                    throw new Exception("API response is null");
                }
                if (!response.Success)
                {
                    throw new Exception(response.Message ?? $"Failed to retrieve contatto with ID {id} from API");
                }
                if (response.Data == null)
                {
                    throw new Exception($"API response data is null for contatto with ID {id}");
                }
                return response;
            }
            catch (Exception ex)
            {
                throw new Exception($"An error occurred while retrieving contatto with ID {id}: {ex.Message}", ex);
            }
        }

        // POST
        public async Task<ApiResponse<AnagraficaContatto>> CreateContattoAsync(AnagraficaContatto newContatto)
        {
            try
            {
                // take in the dto I feed you and post it to the api, return the response as a ContattoDTO
                var response = await apiService.PostAsync<AnagraficaContatto>(Endpoint, newContatto);

                if (response == null)
                {
                    throw new Exception("API response is null");
                }
                if (!response.Success)
                {
                    throw new Exception(response.Message ?? "Failed to create contatto via API");
                }
                //if (response.Data == null)
                //{
                //    throw new Exception("API response data is null after creating contatto");
                //}
                return response;
            }
            catch (Exception ex)
            {
                throw new Exception($"An error occurred while creating contatto: {ex.Message}", ex);
            }
        }

        // PUT

        public async Task<ApiResponse<AnagraficaContatto>> UpdateContattoAsync(int id, AnagraficaContatto updatedContatto)
        {
            try
            {
                // send id to modify + updated entity, return anagraficacontatto
                var response = await apiService.PutAsync<AnagraficaContatto>($"{Endpoint}/{id}", updatedContatto);

                // various failures
                if (response == null)
                {
                    throw new Exception("API response is null");
                }
                if (!response.Success)
                {
                    throw new Exception(response.Message ?? $"Failed to update contatto with ID {id} via API");
                }

                // else ok, return resp & wrap
                return response;
            }
            catch (Exception ex)
            {
                throw new Exception($"An error occurred while updating contatto with ID {id}: {ex.Message}", ex);
            }
        }


        // no more bool needed
        public async Task<ApiResponse> DeleteContattoAsync(int id)
        {
            try
            {
                string endpoint = $"{Endpoint}/{id}";
                var response = await apiService.DeleteAsync(endpoint);

                if (!response.Success)
                {
                    throw new Exception($"Failed to delete contatto with ID {id} via API");
                }

                // in all other cases, pop back response
                return response;
            }
            catch (Exception ex)
            {
                throw new Exception($"An error occurred while deleting contatto with ID {id}: {ex.Message}", ex);
            }

        }

        public async Task<ApiResponse<bool>> ConvertContattoAsync(int id)
        {
            try
            {
                // make sure the id is above 0 / valid
                if (id <= 0)
                {
                    throw new ArgumentException("Contatto ID must be greater than 0", nameof(id));
                }

                // post to convert
                var endpoint = $"{Endpoint}/{id}/convert";

                // get back a bool y/n succeeded
                var response = await apiService.ConvertAsync(endpoint);

                if (!response.Success)
                {
                    throw new Exception($"Failed to convert contatto with ID {id} via API");
                }

                return response;
            }
            catch (Exception ex)
            {
                throw new Exception($"An error occurred while converting contatto with ID {id}: {ex.Message}", ex);
            }

        }
    }
}