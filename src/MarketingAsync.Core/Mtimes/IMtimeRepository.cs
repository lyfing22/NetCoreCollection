﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MarketingAsync.Act;

namespace MarketingAsync.Mtimes
{
    public interface IMtimeRepository : IMongoRepository<Mtime>
    {

        void updateData(Analysis analysi);



        void InsertMany(List<Analysis> analysis);

    }
}
