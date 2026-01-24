using GymManagmentDAL.Entities.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymManagmentDAL.Entities
{
    public class Trainer : GymUser
    {
        // hierdate ==> createdat of baseentity
        public Specialities Specialies { get; set; }

        #region Trainer - Session
        public ICollection<Session> Sessions { get; set; } = null!; 
        #endregion
    }
}
