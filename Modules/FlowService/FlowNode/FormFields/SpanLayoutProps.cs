using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlowService.FlowNode.FormFields
{
    public class SpanLayoutProps : BaseProps
    {
        public FormField[] items { get; set; }
        public override void FindTo(FormField field, Func<FormField, bool> func, List<FormField> target)
        {
            foreach (var item in items)
            {
                item.FindTo(func, target);
            }
        }
        public override bool Check(Dictionary<string, string> commitOperates, FormField field, Dictionary<string, object> model, Dictionary<string, string> inputParams, out string msg)
        {
            foreach (var item in items)
            {
                if (!item.Check(commitOperates, model, inputParams, out msg))
                {
                    return false;
                }
            }

            msg = string.Empty;
            return true;
        }
        public override void FieldRelated(FormField field, Dictionary<string, object> model)
        {
            foreach (var item in items)
            {
                item.FieldRelated(model);
            }
        }
    }
}
