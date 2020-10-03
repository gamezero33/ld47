using UnityEngine;

public class ReadOnlyAttribute : PropertyAttribute
{
    public string precision;
	public string style;

    public ReadOnlyAttribute (string _precision = "")
    {
		style = "Label";
        precision = _precision;
    }
	public ReadOnlyAttribute ( string _style, string _precision )
	{
		style = _style;
		precision = _precision;
	}
}
