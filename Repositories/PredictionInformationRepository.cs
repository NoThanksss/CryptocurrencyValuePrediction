using Microsoft.EntityFrameworkCore;

namespace CryptocurrencyValuePrediction.Repositories
{
    public class PredictionInformationRepository
    {
        private ApplicationContext _context;
        public PredictionInformationRepository(ApplicationContext context)
        {
            _context = context;
        }

        public async Task AddEntityAsync(PredictionInformation model)
        {
            try
            {
                await _context.Predictions.AddAsync(model);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        public IEnumerable<PredictionInformation> GetAllEntities()
        {
            try
            {
                return _context.Predictions.ToList();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return Enumerable.Empty<PredictionInformation>();
            }
        }

        public async Task DeleteEntityAsync(Guid Id)
        {
            var entityToDelete = await _context.Predictions.FirstOrDefaultAsync(x => x.ID == Id);
            _context.Entry<PredictionInformation>(entityToDelete).State = EntityState.Deleted;

            await _context.SaveChangesAsync();
        }

        public async Task<PredictionInformation> GetById(Guid Id)
        {
            return await _context.Predictions.FirstAsync(x => x.ID == Id);
        }
    }
}
