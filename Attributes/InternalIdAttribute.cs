using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace PdfPlayGround.Attributes
{
    using Helper;

    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
    public sealed class InternalIdAttribute : Attribute
    {
        public int InternalId { get; set; }

        public InternalIdAttribute(int id)
        {
            InternalId = id;
        }
    }

    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false)]
    public sealed class GlobalIdAttribute : Attribute
    {
        public object ToInternalId(object globalId)
        {
            switch (globalId)
            {
                case int globalIdInt: return globalIdInt;
                case byte globalIdByte: return globalIdByte;
                case string globalIdStr:
                    if (int.TryParse(globalIdStr, out int parsedId))
                    {
                        return parsedId;
                    }
                    return ToInternalId(globalIdStr);
                case IEnumerable globalIds:
                    List<object> internalIds = new List<object>();
                    foreach (var id in globalIds)
                    {
                        var internalId = ToInternalId(id);
                        if (internalId != null) internalIds.Add(internalId);
                    }
                    if (internalIds.Count > 0) return internalIds;
                    break;
            }
            return null;
        }

        public static string ToGlobalId(string name, object id) => $"{name}:{id}".ToBase64();

        public static int? ToInternalId(string globalId)
        {
            try
            {
                var raw = globalId.FromBase64().Split(':','_').LastOrDefault();
                return int.Parse(raw);
            }
            catch
            {
                return null;
            }
        }
    }
}
