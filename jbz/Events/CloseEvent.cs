using System.Collections.Generic;
using Microsoft.Practices.Prism.Events;

namespace Jbz.Events
{
    public class CloseEvent : CompositePresentationEvent<KeyValuePair<int, int>>
    {
    }
}