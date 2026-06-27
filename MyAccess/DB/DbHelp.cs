using System;
using System.Data;
using System.Data.Common;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Diagnostics;

namespace MyAccess.DB
{
    public abstract class DbHelp : IDisposable, IDbHelp
    {
        protected DbConnection mConn;
        protected string mConnString;
        protected DbTransaction mDbTrans;
        public bool IsTrans()
        {
            return mDbTrans != null;
        }
        private int _addIdx;
        /// <summary>
        /// 当前参数集
        /// </summary>
        private LinkedList<DbParameter> mDbParamters;
        private HashSet<string> mParamterHash;
        protected DbHelp AddDbParameter(DbParameter p)
        {
            if (!mParamterHash.Contains(p.ParameterName))
            {
                mDbParamters.AddLast(p);
                mParamterHash.Add(p.ParameterName);
            }
            return this;
        }


        ~DbHelp()
        {
            Dispose(false);
        }

        private void Dispose(bool disposing)
        {
            Close();
            if (disposing)
            {
                GC.SuppressFinalize(this);
            }
        }

        public void Dispose()
        {
            Dispose(true);
        }
        /// <summary>
        /// 创建兼容处理
        /// </summary>
        /// <returns></returns>
        public abstract ICompatible CreateCompatible(SqlBuilder sql);
        public ICompatible CreateCompatible()
        {
            return CreateCompatible(new SqlBuilder(this));
        }
        public DbHelp(string connectionStr)
        {
            _addIdx = 0;
            mConnString = connectionStr;
            mDbParamters = new LinkedList<DbParameter>();
            mParamterHash = new HashSet<string>();
        }
        /// <summary>
        /// 自动生成参数名
        /// </summary>
        /// <returns></returns>
        private string AutoGenerateParamName()
        {
            string paramName = "PARAM_AUTO_" + _addIdx;
            ++_addIdx;
            return paramName;
        }
        public void ClearDbParamters()
        {
            mDbParamters.Clear();
            mParamterHash.Clear();
        }
        private void InitParamters(DbCommand command)
        {
            foreach (DbParameter p in mDbParamters)
            {
                command.Parameters.Add(p);
            }
        }


        public virtual void BeginTran()
        {
            if (Equals(mDbTrans, null))
            {
                Open();
                mDbTrans = mConn.BeginTransaction();
            }
        }
        public virtual void Commit()
        {
            try
            {
                if (!Equals(mDbTrans, null))
                {
                    mDbTrans.Commit();
                    mDbTrans = null;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                Close();
            }
        }
        public virtual void RollBack()
        {
            try
            {
                if (!Equals(mDbTrans, null))
                {
                    mDbTrans.Rollback();
                    mDbTrans = null;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                Close();
            }
        }
        public void Open()
        {
            if (mConn == null)
            {
                mConn = CreateConnection();
                mConn.ConnectionString = mConnString;
            }
            if (mConn.State == ConnectionState.Closed)
            {
                mConn.Open();
            }
        }

        public T DoCommand<T>(T docommand) where T : IDoCommand
        {
            if (!Equals(mDbTrans, null))
            {
                try
                {
                    DbCommand command = CreateCommand();
                    command.Connection = mConn;
                    if (mDbTrans != null)
                    {
                        command.Transaction = mDbTrans;
                    }
                    InitParamters(command);
                    docommand.Excute(new ExcuteParam
                    {
                        Command = command,
                        Db = this,
                        Sql = null
                    });
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
                finally
                {
                    ClearDbParamters();
                }
            }
            else
            {
                Open();
                try
                {
                    DbCommand command = CreateCommand();
                    command.Connection = mConn;
                    if (mDbTrans != null)
                    {
                        command.Transaction = mDbTrans;
                    }
                    InitParamters(command);
                    docommand.Excute(new ExcuteParam
                    {
                        Command = command,
                        Db = this,
                        Sql = null
                    });
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
                finally
                {
                    ClearDbParamters();
                }
            }
            return docommand;
        }

        /// <summary>
        /// 执行Sql操作
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql"></param>
        /// <returns></returns>
        public T DoCommand<T>(SqlBuilder sql) where T : IDoCommand, new()
        {
            T docommand = new T();
            if (!Equals(mDbTrans, null))
            {
                try
                {
                    DbCommand command = CreateCommand();
                    command.Connection = mConn;
                    if (mDbTrans != null)
                    {
                        command.Transaction = mDbTrans;
                    }
                    InitParamters(command);
                    docommand.Excute(new ExcuteParam
                    {
                        Command = command,
                        Db = this,
                        Sql = sql
                    });
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
                finally
                {
                    ClearDbParamters();
                }
            }
            else
            {
                Open();
                try
                {
                    DbCommand command = CreateCommand();
                    command.Connection = mConn;
                    if (mDbTrans != null)
                    {
                        command.Transaction = mDbTrans;
                    }
                    InitParamters(command);
                    docommand.Excute(new ExcuteParam
                    {
                        Command = command,
                        Db = this,
                        Sql = sql
                    });
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
                finally
                {
                    ClearDbParamters();
                }
            }
            return docommand;
        }




        public void Close()
        {
            if (mDbTrans != null)
            {
                return;
            }
            if (!Equals(mConn, null))
            {
                try
                {
                    if (mConn.State == ConnectionState.Open)
                    {
                        mConn.Close();
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
                mConn = null;
            }

        }


        #region 异步操作
        public virtual async Task BeginTranAsync()
        {
            if (Equals(mDbTrans, null))
            {
                await OpenAsync();
                mDbTrans = await mConn.BeginTransactionAsync();
            }
        }
        public virtual async Task CommitAsync()
        {
            try
            {
                if (!Equals(mDbTrans, null))
                {
                    await mDbTrans.CommitAsync();
                    mDbTrans = null;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                await CloseAsync();
            }
        }

        public virtual async Task RollBackAsync()
        {
            try
            {
                if (!Equals(mDbTrans, null))
                {
                    await mDbTrans.RollbackAsync();
                    mDbTrans = null;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                await CloseAsync();
            }
        }


        public async Task OpenAsync()
        {
            if (mConn == null)
            {
                mConn = CreateConnection();
                mConn.ConnectionString = mConnString;
            }
            if (mConn.State == ConnectionState.Closed)
            {
                await mConn.OpenAsync();
            }
        }
        public async Task<T> DoCommandAsync<T>(T docommand) where T : IDoCommand
        {
            if (!Equals(mDbTrans, null))
            {
                try
                {
                    DbCommand command = CreateCommand();
                    command.Connection = mConn;
                    if (mDbTrans != null)
                    {
                        command.Transaction = mDbTrans;
                    }
                    InitParamters(command);
                    await docommand.ExcuteAsync(new ExcuteParam
                    {
                        Command = command,
                        Db = this,
                        Sql = null
                    });
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
                finally
                {
                    ClearDbParamters();
                }
            }
            else
            {
                await OpenAsync();
                try
                {
                    DbCommand command = CreateCommand();
                    command.Connection = mConn;
                    if (mDbTrans != null)
                    {
                        command.Transaction = mDbTrans;
                    }
                    InitParamters(command);
                    await docommand.ExcuteAsync(new ExcuteParam
                    {
                        Command = command,
                        Db = this,
                        Sql = null
                    });
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
                finally
                {
                    ClearDbParamters();
                }
            }
            return docommand;
        }
        public async Task<T> DoCommandAsync<T>(SqlBuilder sql) where T : IDoCommand, new()
        {
            T docommand = new T();
            if (!Equals(mDbTrans, null))
            {
                try
                {
                    DbCommand command = CreateCommand();
                    command.Connection = mConn;
                    if (mDbTrans != null)
                    {
                        command.Transaction = mDbTrans;
                    }
                    InitParamters(command);
                    await docommand.ExcuteAsync(new ExcuteParam
                    {
                        Command = command,
                        Db = this,
                        Sql = sql
                    });
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
                finally
                {
                    ClearDbParamters();
                }
            }
            else
            {
                await OpenAsync();
                try
                {
                    DbCommand command = CreateCommand();
                    command.Connection = mConn;
                    if (mDbTrans != null)
                    {
                        command.Transaction = mDbTrans;
                    }
                    InitParamters(command);
                    await docommand.ExcuteAsync(new ExcuteParam
                    {
                        Command = command,
                        Db = this,
                        Sql = sql
                    });
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
                finally
                {
                    ClearDbParamters();
                }
            }
            return docommand;
        }

        public async Task CloseAsync()
        {
            if (mDbTrans != null)
            {
                return;
            }
            if (!Equals(mConn, null))
            {
                try
                {
                    if (mConn.State == ConnectionState.Open)
                    {
                        await mConn.CloseAsync();
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
                mConn = null;
            }
        }

        #endregion


        public string AddParam(string param, object value)
        {
            return AutoDbParam(param, value, ParameterDirection.Input);
        }
        public string AddParam(object value)
        {
            return AutoDbParam(AutoGenerateParamName(), value, ParameterDirection.Input);
        }
        /// <summary>
        /// C#类型转数据库类型
        /// </summary>
        /// <param name="tp"></param>
        /// <returns></returns>
        protected abstract string AutoDbParam(string name, object val, ParameterDirection direct);

        protected abstract DbConnection CreateConnection();
        protected abstract DbCommand CreateCommand();
    }
}
