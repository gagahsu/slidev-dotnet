using EShop.DataAccess.Data;
using EShop.DataAccess.Repository.IRepository;
using EShop.Models;

namespace EShop.DataAccess.Repository;

public class ApplicationUserRepository(ApplicationDbContext db)
    : Repository<ApplicationUser>(db), IApplicationUserRepository
{
}
