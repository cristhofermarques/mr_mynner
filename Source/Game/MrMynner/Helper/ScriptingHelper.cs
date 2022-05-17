using System;
using System.Collections.Generic;
using FlaxEngine;

namespace MrMynner
{
    public static partial class Helper
    {
        public static T GetOrAddScriptToActor<T>(Actor act) where T : Script
        {
            T script;
            if(act.TryGetScript<T>(out script))
                return script;
            else
                return act.AddScript<T>();
        }
    }
}
