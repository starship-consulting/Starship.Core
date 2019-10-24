using System;
using System.Collections.Generic;
using System.Linq;
using Starship.Core.Extensions;

namespace Starship.Core.Patch {
    public static class PatchReader {

        public static List<PatchHunk> Read(string patch) {
            var hunks = new List<PatchHunk>();

            if(!patch.Contains("@@")) {
                return hunks;
            }
            
            var index = 0;
            var lines = patch.Split(new [] { "\n" }, StringSplitOptions.RemoveEmptyEntries);
            var adding = lines.Where(each => each.StartsWith("+")).Select(each => new PatchHunk { Text = each.Substring(1), IsAdded = true });
            var removing = lines.Where(each => each.StartsWith("-")).Select(each => new PatchHunk { Text = each.Substring(1), IsAdded = false });

            /*
            var adding = patch.GetOccurancesOf("\n+").Select(each => new { Index = each, Adding = true });
            var removing = patch.GetOccurancesOf("\n-").Select(each => new { Index = each, Adding = false });
            var matches = adding.Concat(removing).OrderBy(each => each.Index).ToList();

            foreach(var match in matches) {
                var text = string.Empty;
                var offset = match.Index + 3;

                if(index == matches.Count-1) {
                    text = patch.Substring(offset);
                }
                else {
                    var length = matches[index+1].Index - offset;

                    if(length > 0) {
                        text = patch.Substring(offset, length);
                    }
                }


                text = text.Trim();

                if(!text.IsEmpty()) {

                    var hunk = new PatchHunk {
                        Text = text,
                        IsAdded = match.Adding
                    };

                    hunks.Add(hunk);
                }
                
                index += 1;
            }
            */

            return adding.Concat(removing).ToList();
        }
    }
}