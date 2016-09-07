using AgeRanger.Web.Application.DataLayer.Implementations;
using AgeRanger.Web.Application.Interfaces;
using AgeRanger.Web.Application.Validators;
using log4net;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace AgeRanger.Web.Application.DataLayer
{
    public class AgeRangerDataFactory
    {
        private static ILog logger = LogManager.GetLogger(typeof(AgeRangerDataFactory));

        private static string ConnectionString
        {
            get
            {
                return ConfigurationManager.ConnectionStrings["AgeRangerDBConnectionString"].ConnectionString;
            }
        }

        private static DBType SelectedDBType
        {
            get
            {
                string dbTypeString = ConfigurationManager.AppSettings["DBType"];
                DBType temp;

                if (!Enum.TryParse<DBType>(dbTypeString, out temp))
                {
                    string errorMessage = string.Format("Unknown database type '{0}' selected in appsetting key 'DBType'.", dbTypeString);
                    logger.Fatal(errorMessage);
                    throw new ApplicationException(errorMessage);
                }

                return temp;
            }
        }

        private static IAgeRangerData _instance = null;
        public static IAgeRangerData Instance
        {
            get
            {
                if (_instance == null)
                {
                    switch(SelectedDBType)
                    {
                        case DBType.SQLServer:
                            _instance = new AgeRangerDataSQLServer(ConnectionString, ValidatorFactory.Instance);
                            break;
                        case DBType.SQLite:
                            _instance = new AgeRangerDataSQLite(ConnectionString, ValidatorFactory.Instance);
                            break;
                        default:
                            string errorMessage = string.Format("Unsupported database type '{0}'.", SelectedDBType);
                            logger.Fatal(errorMessage);
                            throw new ApplicationException(errorMessage);
                    }
                }
                    
                return _instance;
            }
        }
    }

    enum DBType
    {
        SQLite,
        SQLServer
    }
}