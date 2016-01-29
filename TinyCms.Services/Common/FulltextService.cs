using System;
using System.Linq;
using TinyCms.Core.Data;
using TinyCms.Core.Domain.Common;
using TinyCms.Data;

namespace TinyCms.Services.Common
{
    /// <summary>
    ///     Full-Text service
    /// </summary>
    public class FulltextService : IFulltextService
    {
        #region Ctor

        /// <summary>
        ///     Ctor
        /// </summary>
        /// <param name="dataProvider">Data provider</param>
        /// <param name="dbContext">Database Context</param>
        /// <param name="commonSettings">Common settings</param>
        public FulltextService(IDataProvider dataProvider, IDbContext dbContext,
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
        ///     Gets value indicating whether Full-Text is supported
        /// </summary>
        /// <returns>Result</returns>
        public virtual bool IsFullTextSupported()
        {
            if (_commonSettings.UseStoredProceduresIfSupported && _dataProvider.StoredProceduredSupported)
            {
                //stored procedures are enabled and supported by the database. 
                var result = _dbContext.SqlQuery<int>("EXEC [FullText_IsSupported]");
                return result.FirstOrDefault() > 0;
            }

            //stored procedures aren't supported
            return false;
        }

        /// <summary>
        ///     Enable Full-Text support
        /// </summary>
        public virtual void EnableFullText()
        {
            if (_commonSettings.UseStoredProceduresIfSupported && _dataProvider.StoredProceduredSupported)
            {
                //stored procedures are enabled and supported by the database.
                _dbContext.ExecuteSqlCommand("EXEC [FullText_Enable]", true);
            }
            else
            {
                throw new Exception("Stored procedures are not supported by your database");
            }
        }

        /// <summary>
        ///     Disable Full-Text support
        /// </summary>
        public virtual void DisableFullText()
        {
            if (_commonSettings.UseStoredProceduresIfSupported && _dataProvider.StoredProceduredSupported)
            {
                //stored procedures are enabled and supported by the database.
                _dbContext.ExecuteSqlCommand("EXEC [FullText_Disable]", true);
            }
            else
            {
                throw new Exception("Stored procedures are not supported by your database");
            }
        }

        #endregion
    }
}