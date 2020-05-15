using System.Xml;
using System.Xml.Serialization;
using System.Text;

public class LanguageLookup
{
    [XmlAttribute("id")]
    public int lookupId;

	[XmlText()]
	public string textContent;

	public LanguageLookup() { }
}
