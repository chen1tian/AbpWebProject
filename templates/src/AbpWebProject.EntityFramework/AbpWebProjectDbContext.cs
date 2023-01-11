using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Data;
using Volo.Abp.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore.Modeling;

namespace AbpWebProject.EntityFramework
{
    [ConnectionStringName("Default")]
    public class AbpWebProjectDbContext : AbpDbContext<AbpWebProjectDbContext>
    {
        public AbpWebProjectDbContext(DbContextOptions<AbpWebProjectDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }
    }
}
