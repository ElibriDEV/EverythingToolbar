using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Threading;
using System.Windows;
using EverythingToolbar.Data;
using EverythingToolbar.Helpers;
using EverythingToolbar.Properties;
using Peter;

namespace EverythingToolbar.Search
{
    public class AiSearchResultProvider : IItemsProvider<SearchResult>
    {
        private readonly SearchResult _aiResult;
        private string _currentQuery;
        private string _pendingQuery;
        private readonly DispatcherTimer _searchTimer;
        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public bool IsBusy => false;

        public AiSearchResultProvider()
        {
            _aiResult = new SearchResult
            {
                IsAiResult = true,
                AiResponse = Resources.AIWaiting
            };

            _searchTimer = new DispatcherTimer { Interval = System.TimeSpan.FromSeconds(3) };
            _searchTimer.Tick += (s, e) => SearchNow();
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
            _searchTimer.Stop();
            _pendingQuery = query;

            if (string.IsNullOrEmpty(_pendingQuery))
            {
                _currentQuery = "";
                _aiResult.AiResponse = Resources.AIWaiting;
            }
            else
            {
                _searchTimer.Start();
            }
        }

        public void SearchNow()
        {
            _searchTimer.Stop();

            if (_pendingQuery == _currentQuery)
                return;

            _currentQuery = _pendingQuery;

            if (string.IsNullOrEmpty(_currentQuery))
            {
                _aiResult.AiResponse = Resources.AIWaiting;
                return;
            }

            _aiResult.AiResponse = Resources.AIThinking;
            Task.Run(async () =>
            {
                var response = await AIClient.GetResponse(_currentQuery);
                _aiResult.AiResponse = response;
            });
        }
    }
}
