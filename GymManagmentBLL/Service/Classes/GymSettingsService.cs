using GymManagmentBLL.Service.Interfaces;
using GymManagmentDAL.Entities;
using GymManagmentDAL.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymManagmentBLL.Service.Classes
{
    public class GymSettingsService : IGymSettingsService
    {
        private readonly IUnitOfWork _unitOfWork;
        private static GymSettings? _cachedSettings;

        public GymSettingsService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<GymSettings> GetSettingsAsync()
        {
            if (_cachedSettings != null) return _cachedSettings;

            var settingsList = await _unitOfWork.GetRepository<GymSettings>().GetAllAsync();
            var settings = settingsList.FirstOrDefault();
            if (settings == null)
            {
                settings = new GymSettings();
                await _unitOfWork.GetRepository<GymSettings>().AddAsync(settings);
                await _unitOfWork.SaveChangesAsync();
            }

            _cachedSettings = settings;
            return settings;
        }

        public async Task UpdateSettingsAsync(GymSettings settings)
        {
            var existing = await GetSettingsAsync();
            
            existing.GymName = settings.GymName;
            existing.Phone = settings.Phone;
            existing.Address = settings.Address;
            existing.Email = settings.Email;
            existing.FooterText = settings.FooterText;
            existing.Currency = settings.Currency;

            _unitOfWork.GetRepository<GymSettings>().Update(existing);
            await _unitOfWork.SaveChangesAsync();
            
            // Invalidate cache
            _cachedSettings = existing;
        }
    }
}
