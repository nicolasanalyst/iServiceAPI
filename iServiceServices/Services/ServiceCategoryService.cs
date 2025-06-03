using iServiceRepositories.Repositories;
using iServiceRepositories.Repositories.Models;
using iServiceServices.Services.Models;
using Microsoft.Extensions.Configuration;

namespace iServiceServices.Services
{
    public class ServiceCategoryService
    {
        private readonly ServiceCategoryRepository _serviceCategoryRepository;

        public ServiceCategoryService(IConfiguration configuration)
        {
            _serviceCategoryRepository = new ServiceCategoryRepository(configuration);
        }

        public async Task<Result<List<ServiceCategory>>> GetAllServiceCategories()
        {
            try
            {
                var serviceCategories = await _serviceCategoryRepository.GetAsync();
                return Result<List<ServiceCategory>>.Success(serviceCategories);
            }
            catch (Exception ex)
            {
                return Result<List<ServiceCategory>>.Failure($"Falha ao obter as categorias de serviço: {ex.Message}");
            }
        }

        public async Task<Result<ServiceCategory>> GetServiceCategoryById(int categoryId)
        {
            try
            {
                var serviceCategory = await _serviceCategoryRepository.GetByIdAsync(categoryId);

                if (serviceCategory == null)
                {
                    return Result<ServiceCategory>.Failure("Categoria de serviço não encontrada.");
                }

                return Result<ServiceCategory>.Success(serviceCategory);
            }
            catch (Exception ex)
            {
                return Result<ServiceCategory>.Failure($"Falha ao obter a categoria de serviço: {ex.Message}");
            }
        }

        public async Task<Result<List<ServiceCategory>>> GetByUserProfileId(int userProfileId)
        {
            try
            {
                var serviceCategory = await _serviceCategoryRepository.GetByEstablishmentUserProfileIdAsync(userProfileId);

                if (serviceCategory == null)
                {
                    return Result<List<ServiceCategory>>.Failure("Categoria de serviço não encontrada.");
                }

                return Result<List<ServiceCategory>>.Success(serviceCategory);
            }
            catch (Exception ex)
            {
                return Result<List<ServiceCategory>>.Failure($"Falha ao obter a categoria de serviço: {ex.Message}");
            }
        }

        public async Task<Result<ServiceCategory>> AddServiceCategory(ServiceCategory categoryModel)
        {
            try
            {
                var newCategory = await _serviceCategoryRepository.InsertAsync(categoryModel);
                return Result<ServiceCategory>.Success(newCategory);
            }
            catch (Exception ex)
            {
                return Result<ServiceCategory>.Failure($"Falha ao inserir a categoria de serviço: {ex.Message}");
            }
        }

        public async Task<Result<ServiceCategory>> UpdateServiceCategory(ServiceCategory category)
        {
            try
            {
                var updatedCategory = await _serviceCategoryRepository.UpdateAsync(category);
                return Result<ServiceCategory>.Success(updatedCategory);
            }
            catch (Exception ex)
            {
                return Result<ServiceCategory>.Failure($"Falha ao atualizar a categoria de serviço: {ex.Message}");
            }
        }

        public async Task<Result<bool>> SetActiveStatus(int categoryId, bool isActive)
        {
            try
            {
                await _serviceCategoryRepository.SetActiveStatusAsync(categoryId, isActive);
                return Result<bool>.Success(true);
            }
            catch (Exception ex)
            {
                return Result<bool>.Failure($"Falha ao definir o status ativo da categoria de serviço: {ex.Message}");
            }
        }

        public async Task<Result<bool>> SetDeletedStatus(int categoryId, bool isDeleted)
        {
            try
            {
                await _serviceCategoryRepository.SetDeletedStatusAsync(categoryId, isDeleted);
                return Result<bool>.Success(true);
            }
            catch (Exception ex)
            {
                return Result<bool>.Failure($"Falha ao definir o status excluído da categoria de serviço: {ex.Message}");
            }
        }
    }
}
