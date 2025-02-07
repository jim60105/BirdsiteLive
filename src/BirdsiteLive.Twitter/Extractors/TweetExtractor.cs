﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using BirdsiteLive.Common.Settings;
using BirdsiteLive.Twitter.Models;
using Microsoft.Extensions.Logging;
using Tweetinvi.Models;
using Tweetinvi.Models.Entities;

namespace BirdsiteLive.Twitter.Extractors
{
    public interface ITweetExtractor
    {
        ExtractedTweet Extract(ITweet tweet);
    }

    public class TweetExtractor : ITweetExtractor
    {
        private readonly InstanceSettings _instanceSettings;
        private readonly ILogger<TweetExtractor> _logger;

        #region Ctor
        public TweetExtractor(
            InstanceSettings instanceSettings,
            ILogger<TweetExtractor> logger)
        {
            this._instanceSettings = instanceSettings;
            this._logger = logger;
        }
        #endregion

        public ExtractedTweet Extract(ITweet tweet)
        {
            _logger.LogDebug("Extract tweet: {tweet}", tweet);

            var extractedTweet = new ExtractedTweet
            {
                Id = tweet.Id,
                InReplyToStatusId = tweet.InReplyToStatusId,
                InReplyToAccount = tweet.InReplyToScreenName,
                MessageContent = ExtractMessage(tweet),
                Media = ExtractMedia(tweet),
                CreatedAt = tweet.CreatedAt.ToUniversalTime(),
                IsReply = tweet.InReplyToUserId != null,
                IsThread = tweet.InReplyToUserId != null && tweet.InReplyToUserId == tweet.CreatedBy?.Id,
                IsRetweet = tweet.IsRetweet || tweet.QuotedStatusId != null,
                RetweetUrl = ExtractRetweetUrl(tweet),
                IsSensitive = tweet.PossiblySensitive,
                QuoteTweetUrl = tweet.QuotedStatusId != null ? "https://" + _instanceSettings.Domain + "/users/" + tweet.QuotedTweet?.CreatedBy?.ScreenName + "/statuses/" + tweet.QuotedStatusId : null,
                CreatorName = tweet.CreatedBy.UserIdentifier.ScreenName
            };

            return extractedTweet;
        }

        private string ExtractRetweetUrl(ITweet tweet)
        {
            if (tweet.IsRetweet)
            {
                if (tweet.RetweetedTweet != null)
                {
                    var uri = new UriBuilder(tweet.RetweetedTweet.Url);
                    uri.Host = _instanceSettings.TwitterDomain;

                    return uri.Uri.ToString();
                }
                if (tweet.FullText.Contains("https://t.co/"))
                {
                    var retweetId = tweet.FullText.Split(new[] { "https://t.co/" }, StringSplitOptions.RemoveEmptyEntries).Last();
                    return $"https://t.co/{retweetId}";
                }
            }

            return null;
        }

        public string ExtractMessage(ITweet tweet)
        {
            var message = tweet.FullText;
            var tweetUrls = tweet.Media.Select(x => x.URL).Distinct();
            
            if (tweet.IsRetweet && message.StartsWith("RT") && tweet.RetweetedTweet != null)
            {
                message = tweet.RetweetedTweet.FullText;
                tweetUrls = tweet.RetweetedTweet.Media.Select(x => x.URL).Distinct();
            }

            foreach (var tweetUrl in tweetUrls)
            {
                if(tweet.IsRetweet)
                    message = tweet.RetweetedTweet.FullText.Replace(tweetUrl, string.Empty).Trim();
                else 
                    message = message.Replace(tweetUrl, string.Empty).Trim();
            }

            if (tweet.QuotedTweet != null && ! _instanceSettings.EnableQuoteRT)
            {
                message = $"[Quote {{RT}}]{Environment.NewLine}{message}";
            }

            if (tweet.IsRetweet)
            {
                if (tweet.RetweetedTweet != null && !message.StartsWith("RT"))
                    message = $"[{{RT}} @{tweet.RetweetedTweet.CreatedBy.ScreenName}]{Environment.NewLine}{message}";
                else if (tweet.RetweetedTweet != null && message.StartsWith($"RT @{tweet.RetweetedTweet.CreatedBy.ScreenName}:"))
                    message = message.Replace($"RT @{tweet.RetweetedTweet.CreatedBy.ScreenName}:", $"[{{RT}} @{tweet.RetweetedTweet.CreatedBy.ScreenName}]{Environment.NewLine}");
                else
                    message = message.Replace("RT", "[{{RT}}]");
            }

            // Expand URLs
            foreach (var url in tweet.Urls.OrderByDescending(x => x.URL.Length))
            {
                // A bit of a hack
                if (url.ExpandedURL == tweet.QuotedTweet?.Url && _instanceSettings.EnableQuoteRT)
                {
                    url.ExpandedURL = "";
                } else
                {
                    var linkUri = new UriBuilder(url.ExpandedURL);

                    if (linkUri.Host == "twitter.com")
                    {
                        linkUri.Host = _instanceSettings.TwitterDomain;
                        url.ExpandedURL = linkUri.Uri.ToString();
                    }
                }

                message = message.Replace(url.URL, url.ExpandedURL);
            }

            // Hack

            return message;
        }

        public ExtractedMedia[] ExtractMedia(ITweet tweet)
        {
            var media = tweet.Media;
            if (tweet.IsRetweet && tweet.RetweetedTweet != null)
                media = tweet.RetweetedTweet.Media;

            var result = new List<ExtractedMedia>();
            foreach (var m in media)
            {
                var mediaUrl = GetMediaUrl(m);
                var mediaType = GetMediaType(m.MediaType, mediaUrl);
                if (mediaType == null) continue;


                var att = new ExtractedMedia
                {
                    MediaType = mediaType,
                    Url = mediaUrl
                };
                result.Add(att);
            }

            return result.ToArray();
        }

        public string GetMediaUrl(IMediaEntity media)
        {
            switch (media.MediaType)
            {
                case "photo": return media.MediaURLHttps;
                case "animated_gif": return media.VideoDetails.Variants[0].URL;
                case "video": return media.VideoDetails.Variants.OrderByDescending(x => x.Bitrate).First().URL;
                default: return null;
            }
        }

        public string GetMediaType(string mediaType, string mediaUrl)
        {
            switch (mediaType)
            {
                case "photo":
                    var pExt = Path.GetExtension(mediaUrl);
                    switch (pExt)
                    {
                        case ".jpg":
                        case ".jpeg":
                            return "image/jpeg";
                        case ".png":
                            return "image/png";
                    }
                    return null;

                case "animated_gif":
                    var vExt = Path.GetExtension(mediaUrl);
                    switch (vExt)
                    {
                        case ".gif":
                            return "image/gif";
                        case ".mp4":
                            return "video/mp4";
                    }
                    return "image/gif";
                case "video":
                    return "video/mp4";
            }
            return null;
        }
    }
}