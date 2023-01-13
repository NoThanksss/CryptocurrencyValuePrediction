using Microsoft.EntityFrameworkCore;

namespace CryptocurrencyValuePrediction.Repositories
{
    public class PredictionModelErrorRepository
    {
        private ApplicationContext _context;
        public PredictionModelErrorRepository(ApplicationContext context) 
        {
            _context= context;
        }

        public async Task AddEntityAsync(PredictionModelError model)
        {
            try
            {
                await _context.PredictionModelErrors.AddAsync(model);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex) 
            {
                Console.WriteLine(ex.ToString());
            }
        }

        public IEnumerable<PredictionModelError> GetAllEntities()
        {
            try
            {
                return _context.PredictionModelErrors.ToList();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return Enumerable.Empty<PredictionModelError>();   
            }
        }

        public async Task DeleteEntityAsync(Guid Id)
        {
            var entityToDelete = await _context.PredictionModelErrors.FirstOrDefaultAsync(x => x.ID == Id);
            _context.Entry<PredictionModelError>(entityToDelete).State = EntityState.Deleted;

            await _context.SaveChangesAsync();
        }

        public async Task<PredictionModelError> GetById(Guid Id)
        {
            return await _context.PredictionModelErrors.FirstAsync(x => x.ID == Id);
        }
    }
}
