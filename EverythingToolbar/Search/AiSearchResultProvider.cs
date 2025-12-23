using System.Collections.Generic;
using System.Threading.Tasks;
using EverythingToolbar.Data;
using EverythingToolbar.Helpers;
using EverythingToolbar.Properties;
using Peter;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace EverythingToolbar.Search
{
    public class AiSearchResultProvider : IItemsProvider<SearchResult>
    {
        private readonly SearchResult _aiResult;
        private string _currentQuery;
        private bool _isBusy;

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public bool IsBusy
        {
            get => _isBusy;
            private set
            {
                if (_isBusy != value)
                {
                    _isBusy = value;
                    OnPropertyChanged();
                }
            }
        }

        public AiSearchResultProvider()
        {
            _aiResult = new SearchResult
            {
                IsAiResult = true,
                AiResponse = Resources.AIWaiting
            };
        }

        public Task<int> FetchCount(int pageSize, bool isAsync)
        {
            return Task.FromResult(1);
        }

        public Task<IList<SearchResult>> FetchRange(int startIndex, int pageSize, bool isAsync)
        {
            // The AI result is always at index 0. If startIndex is 0 and pageSize is > 0, return the result.
            if (startIndex == 0 && pageSize > 0)
            {
                return Task.FromResult<IList<SearchResult>>(new List<SearchResult> { _aiResult });
            }
            return Task.FromResult<IList<SearchResult>>(new List<SearchResult>());
        }

        public void SetQuery(string query)
        {
            if (query == _currentQuery)
                return;

            _currentQuery = query;

            if (string.IsNullOrEmpty(query))
            {
                _aiResult.AiResponse = Resources.AIWaiting;
                return;
            }

            _aiResult.AiResponse = Resources.AIThinking;
            IsBusy = true;
            Task.Run(async () =>
            {
                var response = await AIClient.GetResponse(_currentQuery);
                _aiResult.AiResponse = response;
                IsBusy = false;
            });
        }
    }
}
