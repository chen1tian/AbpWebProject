using AbpWebProject.Domain.Domains;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Data;
using Volo.Abp.EntityFrameworkCore;

namespace AbpWebProject.EntityFramework
{    
    [ConnectionStringName("Default")]    
    public class AbpWebProjectDbContext : AbpDbContext<AbpWebProjectDbContext>
    {
        public AbpWebProjectDbContext(DbContextOptions<AbpWebProjectDbContext> options) : base(options)
        {
        }

        /// <summary>
        /// 产品
        /// </summary>
        public DbSet<Product> Product { get; set; }
    }
}
