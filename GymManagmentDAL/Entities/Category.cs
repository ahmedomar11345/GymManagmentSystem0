using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymManagmentDAL.Entities
{
    public class Category : BaseEntity
    {
        public string CategoryName { get; set; } = null!;
        public ICollection<Session> sessions { get; set; } = null!;
    }
}
