using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymManagmentBLL.Service.Interfaces
{
    public interface IQRCodeService
    {
        byte[] GenerateQRCode(string text);
        string GenerateQRCodeBase64(string text);
    }
}
