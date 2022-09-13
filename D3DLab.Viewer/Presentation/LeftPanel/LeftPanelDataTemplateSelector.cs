using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace D3DLab.Viewer.Presentation.LeftPanel {
    class LeftPanelDataTemplateSelector : DataTemplateSelector {
        readonly Dictionary<string, DataTemplate> templates;

        public LeftPanelDataTemplateSelector(Dictionary<string, DataTemplate> templates) {
            this.templates = templates;
        }

        public override DataTemplate SelectTemplate(object item, DependencyObject container) {

            return base.SelectTemplate(item, container);
        }
    }
}
