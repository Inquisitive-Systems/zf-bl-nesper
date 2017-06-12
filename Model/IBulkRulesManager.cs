using System.Collections.Generic;

namespace ZF.BL.Nesper.Model
{
    public interface IBulkRulesManager
    {
        void LoadAll(Dictionary<string, string> idEplDictionary, bool exitOnFailure = false);
        void UnloadAll();
    }
}