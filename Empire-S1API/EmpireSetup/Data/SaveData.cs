using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Empire.EmpireSetup.Data
{
    public class GlobalSaveData
    {
        public bool UncNelsonCartelIntroDone;
        public int TotalDealsCompleted;
        public GlobalSaveData()
        {
            UncNelsonCartelIntroDone = false;
            TotalDealsCompleted = 0;
        }
    }
}