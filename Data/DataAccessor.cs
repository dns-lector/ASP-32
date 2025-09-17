using ASP_32.Data.Entities;

namespace ASP_32.Data
{
    public class DataAccessor(DataContext dataContext)
    {
        private readonly DataContext _dataContext = dataContext;

        public IEnumerable<ProductGroup> GetProductGroups()
        {
            return _dataContext
                    .ProductGroups
                    .Where(g => g.DeletedAt == null)
                    .AsEnumerable();
        }
    }
}
