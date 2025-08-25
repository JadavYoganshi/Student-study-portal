using Microsoft.AspNetCore.Http;
using System;

namespace SSP.Services
{
    public class YouTubeService : IYouTubeService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public YouTubeService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public string GetMostWatchedVideo(Guid studentId)
        {
            // Session-based approach
            var sessionKey = $"MostWatchedVideo_{studentId}";
            return _httpContextAccessor.HttpContext.Session.GetString(sessionKey);
        }

        public void SetMostWatchedVideo(Guid studentId, string videoId)
        {
            // Save videoId to session
            var sessionKey = $"MostWatchedVideo_{studentId}";
            _httpContextAccessor.HttpContext.Session.SetString(sessionKey, videoId);
        }
    }

}
