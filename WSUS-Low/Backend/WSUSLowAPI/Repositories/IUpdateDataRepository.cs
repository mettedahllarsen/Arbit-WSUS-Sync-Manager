using WSUSLowAPI.Models;

namespace WSUSLowAPI.Repositories
{
    public interface IUpdateDataRepository
    {
        IEnumerable<UpdateData> GetAll();
        string FetchToDb(string? titleFilter);
    }
}
