using AgeRanger.Web.Application.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AgeRanger.Web.Application.Validators
{
    public class ValidatorFactory
    {
        private static IAgeRangerDataValidator _instance = null;
        public static IAgeRangerDataValidator Instance
        {
            get
            {
                if (_instance == null) _instance = new AgeRangerDefaultValidator();
                return _instance;
            }
        }
    }
}