// tracks SyncVar read/write access when processing NetworkBehaviour,
// to later be replaced by SyncVarAccessReplacer.
using Mono.CecilX;
using System.Collections.Generic;

namespace Mirror.Weaver
{
    // This data is flushed each time - if we are run multiple times in the same process/domain
    public class SyncVarAccessLists
    {
        // setter functions that replace [SyncVar] member variable references. dict<field, replacement>
        public Dictionary<FieldDefinition, MethodDefinition> replacementSetterProperties = new();

        // getter functions that replace [SyncVar] member variable references. dict<field, replacement>
        public Dictionary<FieldDefinition, MethodDefinition> replacementGetterProperties = new();

        // amount of SyncVars per class. dict<className, amount>
        // necessary for SyncVar dirty bits, where inheriting classes start
        // their dirty bits at base class SyncVar amount.
        public Dictionary<string, int> numSyncVars = new();

        public int GetSyncVarStart(string className)
        {
            return numSyncVars.TryGetValue(className, out int value) ? value : 0;
        }

        public void SetNumSyncVars(string className, int num)
        {
            numSyncVars[className] = num;
        }
    }
}
