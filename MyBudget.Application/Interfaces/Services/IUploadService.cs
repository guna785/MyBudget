using MyBudget.Application.Requests;

namespace MyBudget.Application.Interfaces.Services
{
    public interface IUploadService
    {
        string UploadAsync(UploadRequest request);
    }
}
