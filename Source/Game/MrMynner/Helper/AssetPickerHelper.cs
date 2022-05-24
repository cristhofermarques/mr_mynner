using System;
using System.Collections.Generic;
using FlaxEngine;

namespace MrMynner
{
    public static partial class Helper
    {
        public static T PickAsset<T>(params string[] keys) where T : Asset
        {   
            if(keys == null || keys.Length < 1){return null;}

            JsonAsset jsonAsset = Engine.GetCustomSettings("picker_data");
            if(jsonAsset == null){return null;}

            PickData data = jsonAsset.CreateInstance<PickData>();
            if(data == null){return null;}

            for(int keyIdx = 0; keyIdx < keys.Length - 1; keyIdx++)
            {
                if(data.assets.ContainsKey(keys[keyIdx]))
                {
                    Asset subAsset = data.assets[keys[keyIdx]];
                    if(subAsset == null){ return null;}

                    if(subAsset is JsonAsset)
                    {
                        JsonAsset subJsonAsset = subAsset as JsonAsset;
                        PickData subData = subJsonAsset.CreateInstance<PickData>();
                        if(subData == null){return null;}

                        data = subData;
                    }
                }
            }

            if(data.assets.ContainsKey(keys[keys.Length - 1]))
            {
                Asset subAsset = data.assets[keys[keys.Length - 1]];
                if(subAsset == null){return null;}
                if(subAsset is T){return subAsset as T;}
            }

            return null;
        }
    }

    public class PickData
    {
        public Dictionary<string, Asset> assets;
    }
}
