using System;

namespace Starship.Core.Patch {
    public class PatchHunk {
        
        public PatchHunk() {
            Text = string.Empty;
        }

        public string Text { get; set; }

        public bool IsAdded { get; set; }
    }
}