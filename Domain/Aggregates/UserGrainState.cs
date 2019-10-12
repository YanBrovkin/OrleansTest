using System.Collections.Generic;

namespace Domain.Aggregates
{
    public class UserGrainState
    {
        public IList<string> Events { get; } = new List<string>();
    }
}
