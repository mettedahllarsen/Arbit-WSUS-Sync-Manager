using WSUSLowAPI.Models;

namespace WSUSLowAPI.Repositories
{
    public interface IUpdateDataRepository
    {
        List<UpdateData> GetAll();
    }
}
