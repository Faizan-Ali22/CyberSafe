using DA_Assets.DAI;
using System;
using System.Collections.Generic;
using System.Threading;

namespace DA_Assets.FCU
{
    [Serializable]
    public class CancellationTokenController : MonoBehaviourLinkerRuntime<FigmaConverterUnity>
    {
        private Dictionary<TokenType, CancellationTokenSource> _tokens = new Dictionary<TokenType, CancellationTokenSource>();

        internal CancellationTokenSource GetToken(TokenType type)
        {
            if (_tokens.ContainsKey(type))
            {
                return _tokens[type];
            }

            return null;
        }

        public CancellationTokenSource CreateNew(TokenType type)
        {
            Cancel(type);
            CancellationTokenSource newCts = new CancellationTokenSource();
            _tokens[type] = newCts;
            return newCts;
        }

        public void Cancel(TokenType type)
        {
            if (_tokens.ContainsKey(type))
            {
                CancellationTokenSource cts = _tokens[type];
                if (cts != null && !cts.IsCancellationRequested)
                {
                    cts.Cancel();
                }
            }
        }

        public void CancelAll()
        {
            foreach (CancellationTokenSource cts in _tokens.Values)
            {
                if (cts != null && !cts.IsCancellationRequested)
                {
                    cts.Cancel();
                }
            }
        }
    }

    public enum TokenType
    {
        Import,
        Prefab,
        Other
    }
}