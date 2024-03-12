using System.Collections.Generic;

public class MailData
{
	public int index;

	public string msgID = string.Empty;

	public string titel = string.Empty;

	public long dateSeconds;

	public string sender = string.Empty;

	public string content = string.Empty;

	public Dictionary<Defined.COST_TYPE, int> dictItems = new Dictionary<Defined.COST_TYPE, int>();

	public int actionReply;

	public bool read;
}
