using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using BirdsiteLive.Common.Settings;
using BirdsiteLive.Domain.Tools;

namespace BirdsiteLive.Domain.Repository
{
    public interface IModerationRepository
    {
        ModerationTypeEnum GetModerationType(ModerationEntityTypeEnum type);
        ModeratedTypeEnum CheckStatus(ModerationEntityTypeEnum type, string entity);

        IEnumerable<string> GetWhitelistedFollowers();
        IEnumerable<string> GetBlacklistedFollowers();
        IEnumerable<string> GetWhitelistedAccounts();

        IEnumerable<string> GetBlacklistedAccounts();
    }

    public class ModerationRepository : IModerationRepository
    {
        private readonly Regex[] _followersWhiteListing;
        private readonly Regex[] _followersBlackListing;
        private readonly Regex[] _twitterAccountsWhiteListing;
        private readonly Regex[] _twitterAccountsBlackListing;

        private readonly Dictionary<ModerationEntityTypeEnum, ModerationTypeEnum> _modMode =
            new Dictionary<ModerationEntityTypeEnum, ModerationTypeEnum>();

        private readonly ModerationSettings _settings;

        #region Ctor
        public ModerationRepository(ModerationSettings settings)
        {
            _settings = settings;

            var parsedFollowersWhiteListing = PatternsParser.Parse(settings.FollowersWhiteListing);
            var parsedFollowersBlackListing = PatternsParser.Parse(settings.FollowersBlackListing);
            var parsedTwitterAccountsWhiteListing = PatternsParser.Parse(settings.TwitterAccountsWhiteListing);
            var parsedTwitterAccountsBlackListing = PatternsParser.Parse(settings.TwitterAccountsBlackListing);

            _followersWhiteListing = parsedFollowersWhiteListing
                .Select(x => ModerationRegexParser.Parse(ModerationEntityTypeEnum.Follower, x))
                .ToArray();
            _followersBlackListing = parsedFollowersBlackListing
                .Select(x => ModerationRegexParser.Parse(ModerationEntityTypeEnum.Follower, x))
                .ToArray();
            _twitterAccountsWhiteListing = parsedTwitterAccountsWhiteListing
                .Select(x => ModerationRegexParser.Parse(ModerationEntityTypeEnum.TwitterAccount, x))
                .ToArray();
            _twitterAccountsBlackListing = parsedTwitterAccountsBlackListing
                .Select(x => ModerationRegexParser.Parse(ModerationEntityTypeEnum.TwitterAccount, x))
                .ToArray();

            // Set Follower moderation politic
            if (_followersWhiteListing.Any())
                _modMode.Add(ModerationEntityTypeEnum.Follower, ModerationTypeEnum.WhiteListing);
            else if (_followersBlackListing.Any())
                _modMode.Add(ModerationEntityTypeEnum.Follower, ModerationTypeEnum.BlackListing);
            else
                _modMode.Add(ModerationEntityTypeEnum.Follower, ModerationTypeEnum.None);

            // Set Twitter account moderation politic
            if (_twitterAccountsWhiteListing.Any())
                _modMode.Add(ModerationEntityTypeEnum.TwitterAccount, ModerationTypeEnum.WhiteListing);
            else if (_twitterAccountsBlackListing.Any())
                _modMode.Add(ModerationEntityTypeEnum.TwitterAccount, ModerationTypeEnum.BlackListing);
            else
                _modMode.Add(ModerationEntityTypeEnum.TwitterAccount, ModerationTypeEnum.None);
        }
        #endregion

        public ModerationTypeEnum GetModerationType(ModerationEntityTypeEnum type)
        {
            return _modMode[type];
        }

        public ModeratedTypeEnum CheckStatus(ModerationEntityTypeEnum type, string entity)
        {
            if (_modMode[type] == ModerationTypeEnum.None) return ModeratedTypeEnum.None;

            switch (type)
            {
                case ModerationEntityTypeEnum.Follower:
                    return ProcessFollower(entity);
                case ModerationEntityTypeEnum.TwitterAccount:
                    return ProcessTwitterAccount(entity);
            }

            throw new NotImplementedException($"Type {type} is not supported");
        }
        
        private ModeratedTypeEnum ProcessFollower(string entity)
        {
            var politic = _modMode[ModerationEntityTypeEnum.Follower];

            switch (politic)
            {
                case ModerationTypeEnum.None:
                    return ModeratedTypeEnum.None;
                case ModerationTypeEnum.BlackListing:
                    if (_followersBlackListing.Any(x => x.IsMatch(entity)))
                        return ModeratedTypeEnum.BlackListed;
                    return ModeratedTypeEnum.None;
                case ModerationTypeEnum.WhiteListing:
                    if (_followersWhiteListing.Any(x => x.IsMatch(entity)))
                        return ModeratedTypeEnum.WhiteListed;
                    return ModeratedTypeEnum.None;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private ModeratedTypeEnum ProcessTwitterAccount(string entity)
        {
            var politic = _modMode[ModerationEntityTypeEnum.TwitterAccount];

            switch (politic)
            {
                case ModerationTypeEnum.None:
                    return ModeratedTypeEnum.None;
                case ModerationTypeEnum.BlackListing:
                    if (_twitterAccountsBlackListing.Any(x => x.IsMatch(entity)))
                        return ModeratedTypeEnum.BlackListed;
                    return ModeratedTypeEnum.None;
                case ModerationTypeEnum.WhiteListing:
                    if (_twitterAccountsWhiteListing.Any(x => x.IsMatch(entity)))
                        return ModeratedTypeEnum.WhiteListed;
                    return ModeratedTypeEnum.None;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private char GetSplitChar(string entry)
        {
            var separationChar = '|';
            if (entry.Contains(";")) separationChar = ';';
            else if (entry.Contains(",")) separationChar = ',';

            return separationChar;
        }

        public IEnumerable<string> GetWhitelistedFollowers()
        {
            return _settings.FollowersWhiteListing.Split(GetSplitChar(_settings.FollowersWhiteListing), StringSplitOptions.RemoveEmptyEntries);
        }

        public IEnumerable<string> GetBlacklistedFollowers()
        {
            return _settings.FollowersBlackListing.Split(GetSplitChar(_settings.FollowersBlackListing), StringSplitOptions.RemoveEmptyEntries);
        }

        public IEnumerable<string> GetWhitelistedAccounts()
        {
            return _settings.TwitterAccountsWhiteListing.Split(GetSplitChar(_settings.TwitterAccountsWhiteListing), StringSplitOptions.RemoveEmptyEntries);
        }

        public IEnumerable<string> GetBlacklistedAccounts()
        {
            return _settings.TwitterAccountsBlackListing.Split(GetSplitChar(_settings.TwitterAccountsBlackListing), StringSplitOptions.RemoveEmptyEntries);
        }
    }

    public enum ModerationEntityTypeEnum
    {
        Unknown = 0,
        Follower = 1,
        TwitterAccount = 2
    }

    public enum ModerationTypeEnum
    {
        None = 0,
        BlackListing = 1,
        WhiteListing = 2
    }

    public enum ModeratedTypeEnum
    {
        None = 0,
        BlackListed = 1,
        WhiteListed = 2
    }
}