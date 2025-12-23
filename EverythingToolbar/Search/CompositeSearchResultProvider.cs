using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using EverythingToolbar.Data;
using Peter;

namespace EverythingToolbar.Search
{
    public class CompositeSearchResultProvider : IItemsProvider<SearchResult>
    {
        private readonly IItemsProvider<SearchResult> _aiProvider;
        private readonly IItemsProvider<SearchResult> _fileProvider;

        public event PropertyChangedEventHandler PropertyChanged;

        public CompositeSearchResultProvider(IItemsProvider<SearchResult> aiProvider, IItemsProvider<SearchResult> fileProvider)
        {
            _aiProvider = aiProvider;
            _fileProvider = fileProvider;
            _aiProvider.PropertyChanged += OnSubProviderPropertyChanged;
            _fileProvider.PropertyChanged += OnSubProviderPropertyChanged;
        }

        private void OnSubProviderPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(IItemsProvider<SearchResult>.IsBusy))
            {
                OnPropertyChanged(nameof(IsBusy));
            }
        }

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public bool IsBusy => _aiProvider.IsBusy || _fileProvider.IsBusy;

        public async Task<int> FetchCount(int pageSize, bool isAsync)
        {
            var aiCount = await _aiProvider.FetchCount(pageSize, isAsync);
            var fileCount = await _fileProvider.FetchCount(pageSize, isAsync);
            return aiCount + fileCount;
        }

        public async Task<IList<SearchResult>> FetchRange(int startIndex, int pageSize, bool isAsync)
        {
            var aiCount = await _aiProvider.FetchCount(pageSize, isAsync);
            var results = new List<SearchResult>();

            if (startIndex < aiCount)
            {
                results.AddRange(await _aiProvider.FetchRange(startIndex, 1, isAsync));
                pageSize--;
                if (pageSize <= 0)
                {
                    return results;
                }
                startIndex = 0;
            }
            else
            {
                startIndex -= aiCount;
            }

            results.AddRange(await _fileProvider.FetchRange(startIndex, pageSize, isAsync));

            return results;
        }
    }
}
