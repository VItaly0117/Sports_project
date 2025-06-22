using System.Threading.Tasks;

namespace Web_site_analytic_sports.Services
{
    public interface IDataService
    {
        Task SeedWorldCupDataAsync(int year);
    }
}