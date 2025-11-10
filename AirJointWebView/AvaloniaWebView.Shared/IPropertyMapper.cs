using System.Collections.Generic;

namespace AvaloniaWebView.Shared;
public interface IPropertyMapper
{
    IEnumerable<string> GetKeys();
}
