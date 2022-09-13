using System;
using System.Collections.Generic;
using System.IO;

using D3DLab.ECS;
using D3DLab.Plugin;

namespace D3DLab.App.Shell.Plugin {
    public class PluginLoadedObjectDetails : IPluginLoadedObjectDetails {
        public Guid ID { get; }
        public FileInfo FilePath { get; }
        public IEnumerable<ElementTag> VisualObjectTags { get; }

        public PluginLoadedObjectDetails(Guid id, FileInfo filePath, IEnumerable<ElementTag> visualObjectTags) {
            ID= id;
            FilePath = filePath;
            VisualObjectTags = visualObjectTags;
        }
    }
}
