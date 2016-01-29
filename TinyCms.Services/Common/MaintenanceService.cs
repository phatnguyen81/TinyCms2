using System;
using System.Linq;
using TinyCms.Core;
using TinyCms.Core.Data;
using TinyCms.Core.Domain.Common;
using TinyCms.Data;

namespace TinyCms.Services.Common
{
    /// <summary>
    ///     Maintenance service
    /// </summary>
    public class MaintenanceService : IMaintenanceService
    {
        #region Ctor

        /// <summary>
        ///     Ctor
        /// </summary>
        /// <param name="dataProvider">Data provider</param>
        /// <param name="dbContext">Database Context</param>
        /// <param name="commonSettings">Common settings</param>
        public MaintenanceService(IDataProvider dataProvider, IDbContext dbContext,
            CommonSettings commonSettings)
        {
            _dataProvider = dataProvider;
            _dbContext = dbContext;
            _commonSettings = commonSettings;
        }

        #endregion

        #region Fields

        private readonly IDataProvider _dataProvider;
        private readonly IDbContext _dbContext;
        private readonly CommonSettings _commonSettings;

        #endregion

        #region Methods

        /// <summary>
        ///     Get the current ident value
        /// </summary>
        /// <typeparam name="T">Entity</typeparam>
        /// <returns>Integer ident; null if cannot get the result</returns>
        public virtual int? GetTableIdent<T>() where T : BaseEntity
        {
            if (_commonSettings.UseStoredProceduresIfSupported && _dataProvider.StoredProceduredSupported)
            {
                //stored procedures are enabled and supported by the database
                var tableName = _dbContext.GetTableName<T>();
                var result = _dbContext.SqlQuery<decimal>(string.Format("SELECT IDENT_CURRENT('[{0}]')", tableName));
                return Convert.ToInt32(result.FirstOrDefault());
            }

            //stored procedures aren't supported
            return null;
        }

        /// <summary>
        ///     Set table ident (is supported)
        /// </summary>
        /// <typeparam name="T">Entity</typeparam>
        /// <param name="ident">Ident value</param>
        public virtual void SetTableIdent<T>(int ident) where T : BaseEntity
        {
            if (_commonSettings.UseStoredProceduresIfSupported && _dataProvider.StoredProceduredSupported)
            {
                //stored procedures are enabled and supported by the database.

                var currentIdent = GetTableIdent<T>();
                if (currentIdent.HasValue && ident > currentIdent.Value)
                {
                    var tableName = _dbContext.GetTableName<T>();
                    _dbContext.ExecuteSqlCommand(string.Format("DBCC CHECKIDENT([{0}], RESEED, {1})", tableName, ident));
                }
            }
            else
            {
                throw new Exception("Stored procedures are not supported by your database");
            }
        }

        #endregion
    }
}