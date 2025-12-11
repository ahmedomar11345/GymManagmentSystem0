using GymManagmentDAL.Data.Context;
using GymManagmentDAL.Entities;
using GymManagmentDAL.Reposotories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymManagmentDAL.Reposotories.Classes
{
    internal class HealthRecordRepository : IhealthRecordRepostory
    {
        private readonly GymDBContext _dbContext = new GymDBContext();
        public int Add(HealthRecord healthRecord)
        {
            _dbContext.HealthRecords.Add(healthRecord);
            return _dbContext.SaveChanges();
        }

        public int Delete(int id)
        {
            var healthRecord = _dbContext.HealthRecords.Find(id);
            if (healthRecord is null)
                return 0;
            _dbContext.HealthRecords.Remove(healthRecord);
            return _dbContext.SaveChanges();
        }

        public IEnumerable<HealthRecord> GetAll()=> _dbContext.HealthRecords.ToList();

        public HealthRecord? GetById(int id)=> _dbContext.HealthRecords.Find(id);

        public int Update(HealthRecord healthRecord)
        {
            _dbContext.HealthRecords.Update(healthRecord);
            return _dbContext.SaveChanges();
        }
    }
}
