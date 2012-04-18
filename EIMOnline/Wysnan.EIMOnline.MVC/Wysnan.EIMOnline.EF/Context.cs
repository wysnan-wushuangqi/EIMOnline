﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using Wysnan.EIMOnline.Common.Poco;
using Wysnan.EIMOnline.EF.Mapping;

namespace Wysnan.EIMOnline.EF
{
    public class Context : DbContext
    {
        public Context()
            : base()
        {

        }

        public Context(string connectionString)
            : base(connectionString)
        {
            
        }

        DbSet<SecurityUser> SecurityUser { get; set; }
        DbSet<OperateLog> OperateLog { get; set; }
        DbSet<SecurityRole> SecurityRole { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
            modelBuilder.Conventions.Remove<OneToManyCascadeDeleteConvention>();

            modelBuilder.Configurations.Add(new SecurityUserMap());
        }
    }
}
