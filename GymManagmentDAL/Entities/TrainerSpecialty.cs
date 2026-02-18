using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymManagmentDAL.Entities
{
    public class TrainerSpecialty : BaseEntity
    {
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        
        public ICollection<Trainer> Trainers { get; set; } = new HashSet<Trainer>();
    }
}
