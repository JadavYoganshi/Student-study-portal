namespace SSP.Services
{
    public interface IYouTubeService
    {
        string GetMostWatchedVideo(Guid studentId);
        void SetMostWatchedVideo(Guid studentId, string videoId);

    }
}
