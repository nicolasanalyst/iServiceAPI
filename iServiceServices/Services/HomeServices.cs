using iServiceRepositories.Repositories;
using iServiceRepositories.Repositories.Models;
using iServiceServices.Services.Models;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Extensions.Configuration;
using MySqlX.XDevAPI.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iServiceServices.Services
{
    public class HomeModel
    {
        public Appointment? NextAppointment { get; set; }
        public UserInfo? Establishment { get; set; }
        public UserInfo? Client { get; set; }
        public List<EstablishmentCategory>? Categories { get; set; }
        public int? TotalAppointments { get; set; }
        public int? TotalServicesActives { get; set; }
        public List<MonthlyReport> MonthlyReports { get; set; }

        public HomeModel()
        {
            NextAppointment = new Appointment();
            Categories = new List<EstablishmentCategory>();
            MonthlyReports = new List<MonthlyReport>();
        }
    }
    public class HomeServices
    {
        private readonly UserInfoService _userInfoService;
        private readonly AppointmentRepository _appointmentRepository;
        private readonly EstablishmentCategoryRepository _establishmentCategoryRepository;
        private readonly ServiceRepository _serviceRepository;
        public HomeServices(IConfiguration configuration)
        {
            _userInfoService = new UserInfoService(configuration);
            _appointmentRepository = new AppointmentRepository(configuration);
            _establishmentCategoryRepository = new EstablishmentCategoryRepository(configuration);
            _serviceRepository = new ServiceRepository(configuration);
        }

        public async Task<Result<HomeModel>> GetAsync(int userId)
        {
            try
            {
                var home = new HomeModel();
                var result = await _userInfoService.GetUserInfoByUserId(userId);

                if (result.IsSuccess)
                {
                    var userInfo = result.Value;
                    var role = userInfo.UserRole;
                    if (role?.UserRoleId == 2)
                    {
                        home.Establishment = userInfo;
                        home.TotalAppointments = await _appointmentRepository.GetCountByDateAsync(userInfo.UserProfile.UserProfileId, DateTime.Now);
                        home.NextAppointment = await _appointmentRepository.GetNextAppointmentEstablishmentAsync(userId);
                        home.TotalServicesActives = await _serviceRepository.GetCountActivesAsync(userInfo.UserProfile.UserProfileId);
                        if (home.NextAppointment != null)
                        {
                            var client = await _userInfoService.GetUserInfoByUserProfileId(home.NextAppointment.ClientUserProfileId);
                            home.Client = client.Value;
                        }
                        home.MonthlyReports = await _appointmentRepository.GetMonthlyReportsAsync(home.Establishment.UserProfile.UserProfileId);
                    }
                    if (role?.UserRoleId == 3)
                    {
                        home.Client = userInfo;
                        home.NextAppointment = await _appointmentRepository.GetNextAppointmentClientAsync(userInfo.UserProfile.UserProfileId);
                        home.TotalAppointments = await _appointmentRepository.GetCountByDateAsync(userInfo.UserProfile.UserProfileId, DateTime.Now);
                        if (home.NextAppointment != null)
                        {
                            var establishment = await _userInfoService.GetUserInfoByUserProfileId(home.NextAppointment.EstablishmentUserProfileId);
                            home.Establishment = establishment.Value;
                        }
                    }
                    home.Categories = await _establishmentCategoryRepository.GetAsync();
                    return Result<HomeModel>.Success(home);
                }
                return Result<HomeModel>.Failure($"Falha ao obter os dados.");
            }
            catch (Exception ex)
            {
                return Result<HomeModel>.Failure($"Falha ao obter os dados: {ex.Message}");
            }
        }

        public async Task<List<Appointment>> GetAllByDateAsync(int userProfileId, DateTime date)
        {
            return await _appointmentRepository.GetAllByDateAsync(userProfileId, date);
        }

        public async Task<int> GetCountByDateAsync(int userProfileId, DateTime date)
        {
            return await _appointmentRepository.GetCountByDateAsync(userProfileId, date);
        }

        public async Task<Appointment> GetNextAppointmentEstablishmentAsync(int userProfileId)
        {
            return await _appointmentRepository.GetNextAppointmentEstablishmentAsync(userProfileId);
        }

        public async Task<Appointment> GetNextAppointmentClientAsync(int userProfileId)
        {
            return await _appointmentRepository.GetNextAppointmentClientAsync(userProfileId);
        }
    }
}
