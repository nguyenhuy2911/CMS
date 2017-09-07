using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using Orchard.ContentManagement;
using Orchard.ContentManagement.Records;
using Orchard.Environment.Cache;
using Orchard.Layers.Handlers;
using Orchard.Layers.Indexes;
using Orchard.Layers.Models;
using YesSql;

namespace Orchard.Layers.Services
{
	public class LayerService : ILayerService
	{
		private readonly IMemoryCache _memoryCache;
		private readonly ISession _session;
		private readonly ISignal _signal;

		private const string LayersCacheKey = "LayersDocument";

		public LayerService(
			ISignal signal,
			ISession session,
			IMemoryCache memoryCache)
		{
			_signal = signal;
			_session = session;
			_memoryCache = memoryCache;
		}

		public async Task<LayersDocument> GetLayersAsync()
		{
			LayersDocument layers;

			if (!_memoryCache.TryGetValue(LayersCacheKey, out layers))
			{
				layers = await _session.Query<LayersDocument>().FirstOrDefaultAsync();

				if (layers == null)
				{
					lock (_memoryCache)
					{
						if (!_memoryCache.TryGetValue(LayersCacheKey, out layers))
						{
							layers = new LayersDocument();

							_session.Save(layers);
							_memoryCache.Set(LayersCacheKey, layers);
						}
					}
				}
				else
				{
					_memoryCache.Set(LayersCacheKey, layers);
				}
			}

			return layers;
		}

		public Task<IEnumerable<LayerMetadata>> GetLayerWidgetsAsync(Expression<Func<ContentItemIndex, bool>> predicate)
		{
			return _memoryCache.GetOrCreateAsync("Orchard.Layers:Layers" + predicate.ToString(), async entry =>
			{
				entry.AddExpirationToken(_signal.GetToken(LayerMetadataHandler.LayerChangeToken));

				var allWidgets = await _session
				    .Query<ContentItem, LayerMetadataIndex>()
				    .With(predicate)
				    .ListAsync();

				return (IEnumerable<LayerMetadata>)allWidgets
					.Select(x => x.As<LayerMetadata>())
					.Where(x => x != null)
					.OrderBy(x => x.Position)
					.ToList();
			});			
		}

		public async Task UpdateAsync(LayersDocument layers)
		{
			var existing = await _session.Query<LayersDocument>().FirstOrDefaultAsync();

			existing.Layers = layers.Layers;
			_session.Save(existing);
			_memoryCache.Set(LayersCacheKey, existing);
		}
	}
}
