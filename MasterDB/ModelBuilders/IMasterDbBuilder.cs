using Microsoft.EntityFrameworkCore;

namespace MasterDB.ModelBuilders
{
    internal interface IMasterDbBuilder
    {
        void Build(ModelBuilder modelBuilder);
    }
}
