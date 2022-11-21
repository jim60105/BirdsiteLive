using System;
using System.Linq;
using System.Threading.Tasks;
using BirdsiteLive.Common.Settings;
using BirdsiteLive.DAL.Contracts;
using BirdsiteLive.Domain.Repository;

namespace BirdsiteLive.Services
{
    public interface IAboutPageService
    {
        Task<AboutPageData> GetAboutPageDataAsync();
    }

    public class AboutPageService : IAboutPageService
    {
        private readonly ITwitterUserDal _twitterUserDal;

        private static AboutPageData _aboutPageData;
        private readonly InstanceSettings _instanceSettings;
        private readonly IModerationRepository _moderationRepository;

        #region Ctor
        public AboutPageService(ITwitterUserDal twitterUserDal, InstanceSettings instanceSettings, IModerationRepository moderationRepository)
        {
            _twitterUserDal = twitterUserDal;
            _instanceSettings = instanceSettings;
            _moderationRepository = moderationRepository;
        }
        #endregion

        public async Task<AboutPageData> GetAboutPageDataAsync()
        {
            if (_aboutPageData == null ||
                (DateTime.UtcNow - _aboutPageData.RefreshedTime).TotalMinutes > 15)
            {
                var twitterUserMax = _instanceSettings.MaxUsersCapacity;
                var twitterUserCount = await _twitterUserDal.GetTwitterUsersCountAsync();
                var saturation = (int)((double)twitterUserCount / twitterUserMax * 100);

                _aboutPageData = new AboutPageData
                {
                    RefreshedTime = DateTime.UtcNow,
                    Saturation = saturation,
                    UnlistedUsers = _instanceSettings.UnlistedTwitterAccounts.Length > 0 ? string.Join("\n", _instanceSettings.UnlistedTwitterAccounts.Split(";").Select(i => "<li>" + i + "</li>")) : "(none)",
                    Settings = _instanceSettings,
                    ModerationStatus = new ModerationStatus
                    {
                        Followers = _moderationRepository.GetModerationType(ModerationEntityTypeEnum.Follower),
                        TwitterAccounts = _moderationRepository.GetModerationType(ModerationEntityTypeEnum.TwitterAccount),
                        Repository = _moderationRepository
                    }
                };
            }

            return _aboutPageData;
        }
    }

    public class AboutPageData
    {
        public DateTime RefreshedTime { get; set; }
        public int Saturation { get; set; }
        public string UnlistedUsers { get; set; }
        public InstanceSettings Settings { get; set; }
        public ModerationStatus ModerationStatus { get; set; }
    }

    public class ModerationStatus
    {
        public ModerationTypeEnum Followers { get; set; }
        public ModerationTypeEnum TwitterAccounts { get; set; }
        public IModerationRepository Repository { get; set; }
    }
}