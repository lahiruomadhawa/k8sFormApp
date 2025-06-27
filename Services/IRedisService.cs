namespace k8sFormApp.Services
{
    public interface IRedisService
    {
        Task AddPersonAsync(string personJson);
    }
}
