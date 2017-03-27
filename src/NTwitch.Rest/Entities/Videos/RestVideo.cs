﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Model = NTwitch.Rest.API.Video;

namespace NTwitch.Rest
{
    public class RestVideo : RestEntity<string>
    {
        /// <summary> The channel associated with this video </summary>
        public RestSimpleChannel Channel { get; private set; }
        /// <summary> The date and time this video will become viewable </summary>
        public DateTime? ViewableAt { get; private set; }
        /// <summary> The date and time this video was created </summary>
        public DateTime CreatedAt { get; private set; }
        /// <summary> The date and time this video was published at </summary>
        public DateTime PublishedAt { get; private set; }
        /// <summary> The tags associated with this video </summary>
        public string[] Tags { get; private set; }
        /// <summary>  </summary>
        public string Viewable { get; private set; }
        /// <summary>  </summary>
        public string BroadcastType { get; private set; }
        /// <summary> The description provided for this video </summary>
        public string Description { get; private set; }
        /// <summary> The raw html description provided for this video </summary>
        public string DescriptionHtml { get; private set; }
        /// <summary> The game displayed in this video </summary>
        public string Game { get; private set; }
        /// <summary> The language used in this video </summary>
        public string Language { get; private set; }
        /// <summary>  </summary>
        public string Status { get; private set; }
        /// <summary> The title of this video </summary>
        public string Title { get; private set; }
        /// <summary> The url to this video's page </summary>
        public string Url { get; private set; }
        /// <summary> The id of the broadcast associated with this video </summary>
        public uint BroadcastId { get; private set; }
        /// <summary> The length in seconds of this video </summary>
        public uint Length { get; private set; }
        /// <summary> The total number of views this video has received </summary>
        public uint Views { get; private set; }
        /// <summary> Segments of the video muted by twitch </summary>
        public IReadOnlyCollection<VideoSegment> MutedSegments { get; private set; }
        /// <summary> Available qualities for this video </summary>
        public IReadOnlyDictionary<string, float> Qualities { get; private set; }
        /// <summary> The urls for this video's preview images </summary>
        public IReadOnlyDictionary<string, string> Previews { get; private set; }
        /// <summary> Available resolutions for this video </summary>
        public IReadOnlyDictionary<string, string> Resolutions { get; private set; }
        /// <summary> The urls for this video's thumbnail images </summary>
        public IReadOnlyDictionary<string, VideoThumbnail> Thumbnails { get; private set; }

        internal RestVideo(BaseRestClient client, string id) 
            : base(client, id) { }

        internal static RestVideo Create(BaseRestClient client, Model model)
        {
            var entity = new RestVideo(client, model.Id);
            entity.Update(model);
            return entity;
        }

        internal virtual void Update(Model model)
        {
            Channel = new RestSimpleChannel(Client, model.Channel.Id);
            Channel.Update(model.Channel);
            BroadcastId = model.BroadcastId;
            BroadcastType = model.BroadcastType;
            CreatedAt = model.CreatedAt;
            Description = model.Description;
            DescriptionHtml = model.Description;
            Qualities = model.Fps;
            Game = model.Game;
            Language = model.Language;
            Length = model.Length;
            MutedSegments = model.MutedSegments.ToArray();
            Previews = model.PreviewImages;
            PublishedAt = model.PublishedAt;
            Resolutions = model.Resolutions;
            Status = model.Status;
            Tags = model.Tags.Split(' ');
            Thumbnails = model.Thumbnails;
            Title = model.Title;
            Url = model.Url;
            Viewable = model.Viewable;
            ViewableAt = model.ViewableAt;
            Views = model.Views;
        }

        /// <summary> Update this video's properties </summary>
        public async Task UpdateAsync()
        {
            var entity = await Client.RestClient.GetVideoAsync(Id).ConfigureAwait(false);
            Update(entity);
        }
    }
}
