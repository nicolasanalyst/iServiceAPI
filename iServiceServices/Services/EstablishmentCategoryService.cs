using iServiceRepositories.Repositories;
using iServiceRepositories.Repositories.Models;
using iServiceServices.Services.Models;
using Microsoft.Extensions.Configuration;
using Org.BouncyCastle.Asn1.Ocsp;

namespace iServiceServices.Services
{
    public class EstablishmentCategoryService
    {
        private readonly EstablishmentCategoryRepository _establishmentCategoryRepository;

        public EstablishmentCategoryService(IConfiguration configuration)
        {
            _establishmentCategoryRepository = new EstablishmentCategoryRepository(configuration);
        }

        public async Task<Result<List<EstablishmentCategory>>> GetAllEstablishmentCategories()
        {
            try
            {
                var establishmentCategories = await _establishmentCategoryRepository.GetAsync();
                return Result<List<EstablishmentCategory>>.Success(establishmentCategories);
            }
            catch (Exception ex)
            {
                return Result<List<EstablishmentCategory>>.Failure($"Falha ao obter as categorias de estabelecimento: {ex.Message}");
            }
        }

        public async Task<Result<EstablishmentCategory>> GetEstablishmentCategoryById(int categoryId)
        {
            try
            {
                var establishmentCategory = await _establishmentCategoryRepository.GetByIdAsync(categoryId);

                if (establishmentCategory == null)
                {
                    return Result<EstablishmentCategory>.Failure("Categoria de estabelecimento não encontrada.");
                }

                return Result<EstablishmentCategory>.Success(establishmentCategory);
            }
            catch (Exception ex)
            {
                return Result<EstablishmentCategory>.Failure($"Falha ao obter a categoria de estabelecimento: {ex.Message}");
            }
        }

        public async Task<Result<EstablishmentCategory>> AddEstablishmentCategory(EstablishmentCategory categoryModel)
        {
            try
            {
                var newCategory = await _establishmentCategoryRepository.InsertAsync(categoryModel);

                if (newCategory != null)
                {
                    if (categoryModel.File != null)
                    {
                        var path = await new FtpServices().UploadFileAsync(categoryModel.File, "icons", $"icon_{newCategory.EstablishmentCategoryId}.png");

                        var image = await _establishmentCategoryRepository.UpdateIconAsync(newCategory.EstablishmentCategoryId, path);
                        newCategory.Icon = path;
                    }
                }

                return Result<EstablishmentCategory>.Success(newCategory);
            }
            catch (Exception ex)
            {
                return Result<EstablishmentCategory>.Failure($"Falha ao inserir a categoria de estabelecimento: {ex.Message}");
            }
        }

        public async Task<Result<EstablishmentCategory>> UpdateEstablishmentCategory(EstablishmentCategory category)
        {
            try
            {
                var updatedCategory = await _establishmentCategoryRepository.UpdateAsync(category);

                if (updatedCategory != null)
                {
                    if (category.File != null)
                    {
                        var path = await new FtpServices().UploadFileAsync(category.File, "icons", $"icon_{category.EstablishmentCategoryId}.png");

                        var image = await _establishmentCategoryRepository.UpdateIconAsync(category.EstablishmentCategoryId, path);
                        updatedCategory.Icon = path;
                    }
                }

                return Result<EstablishmentCategory>.Success(updatedCategory);
            }
            catch (Exception ex)
            {
                return Result<EstablishmentCategory>.Failure($"Falha ao atualizar a categoria de estabelecimento: {ex.Message}");
            }
        }

        public async Task<Result<bool>> SetActiveStatus(int categoryId, bool isActive)
        {
            try
            {
                await _establishmentCategoryRepository.SetActiveStatusAsync(categoryId, isActive);
                return Result<bool>.Success(true);
            }
            catch (Exception ex)
            {
                return Result<bool>.Failure($"Falha ao definir o status ativo da categoria de estabelecimento: {ex.Message}");
            }
        }

        public async Task<Result<bool>> SetDeletedStatus(int categoryId, bool isDeleted)
        {
            try
            {
                await _establishmentCategoryRepository.SetDeletedStatusAsync(categoryId, isDeleted);
                return Result<bool>.Success(true);
            }
            catch (Exception ex)
            {
                return Result<bool>.Failure($"Falha ao definir o status excluído da categoria de estabelecimento: {ex.Message}");
            }
        }
    }
}
