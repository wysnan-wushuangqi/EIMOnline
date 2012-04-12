﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;

namespace Wysnan.EIMOnline.Common.Poco
{
    public class SecurityUser : IBaseEntity
    {
        public int ID { get; set; }

        public byte? SystemStatus { get; set; }

        [ConcurrencyCheck]
        [Timestamp]
        public byte[] TimeStamp { get; set; }

        public string UserName { get; set; }

        public string UserLoginID { get; set; }

        public string UserLoginPwd { get; set; }

		public  SecurityUser()
        {
            this.Logs1 = new List<Logs>();
        }

        public virtual ICollection<Logs> Logs1 { get; set; }
		
        public virtual ICollection<SecurityUserRole> SecurityUserRoles { get; set; }
    }
}
