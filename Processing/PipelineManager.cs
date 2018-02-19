using System.Collections.Generic;
using System.Threading.Tasks;

namespace Starship.Core.Processing {
    public class PipelineManager {

        public PipelineManager() {
            Pipelines = new List<Pipeline>();
        }

        public void Add(Pipeline pipeline) {
            Pipelines.Add(pipeline);
        }

        public async Task Process(object input) {
            foreach (var pipeline in Pipelines) {

                if (input == null) {
                    return;
                }

                input = await pipeline.Process(input);
            }
        }

        private List<Pipeline> Pipelines { get; set; }
    }
}