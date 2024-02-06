using System.Collections.Generic;

namespace Comm.Model
{
	public class LumenDataMessage
	{
		public string Id { get; set; }
		public IEnumerable<LumensCoordinates> Lumens { get; }

		public LumenDataMessage(string id, IEnumerable<LumensCoordinates> lumens)
		{
			Id = id;
			Lumens = lumens;
		}
	}
}
