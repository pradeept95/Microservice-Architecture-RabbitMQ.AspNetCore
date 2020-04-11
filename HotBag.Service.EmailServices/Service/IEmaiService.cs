using System.Threading.Tasks;

namespace HotBag.Service.EmailServices.Service
{
    public interface IEmaiService
    {
        Task ProcessDefault();
        Task ProcessExtended();
    }
}