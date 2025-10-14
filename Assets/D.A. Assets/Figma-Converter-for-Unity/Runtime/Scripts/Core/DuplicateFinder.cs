using DA_Assets.DAI;
using DA_Assets.Extensions;
using DA_Assets.FCU.Extensions;
using DA_Assets.FCU.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DA_Assets.FCU
{
    [Serializable]
    public class DuplicateFinder : MonoBehaviourLinkerRuntime<FigmaConverterUnity>
    {
        /// <summary>
        /// Prevents duplication of downloaded sprites.
        /// </summary>
        public async Task SetDuplicateFlags(List<FObject> fobjects)
        {
            var fobjectsByFrame = fobjects.GroupBy(x => x.Data.RootFrame);

            Dictionary<SyncData, List<int>> hashedNoDubsInFrames = new Dictionary<SyncData, List<int>>();

            foreach (var item in fobjectsByFrame)
            {
                var noDubsInFrame = item
                    .GroupBy(x => x.Data.Hash)
                    .Select(x => x.First().Data.Hash);

                hashedNoDubsInFrames.Add(item.Key, noDubsInFrame.ToList());
            }

            await Task.Run(() =>
            {
                List<int> hashesDuplicates = hashedNoDubsInFrames.GetDuplicates().ToList();

                Parallel.ForEach(fobjects, fobject =>
                {
                    if (hashesDuplicates.Contains(fobject.Data.Hash))
                    {
                        fobject.Data.IsMutual = true;
                    }
                });

                int[] hashDuplicates = fobjects
                        .GroupBy(x => x.Data.Hash)
                        .SelectMany(g => g.Skip(1))
                        .Select(x => x.Data.Hash)
                        .ToArray();

                Parallel.ForEach(fobjects, fobject =>
                {
                    if (hashDuplicates.Contains(fobject.Data.Hash))
                    {
                        fobject.Data.IsDuplicate = true;
                    }
                });
            }, monoBeh.GetToken(TokenType.Import));  
        }
    }
}