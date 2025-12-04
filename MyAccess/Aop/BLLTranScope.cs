using MyAccess.DB;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace MyAccess.Aop
{
    public class BLLTranScope : IDisposable
    {
        private bool _isComplete;
        private long _instanceId;
        private static long IncreaseId = 0;
        public BLLTranScope()
        {
            if (TransAttribute.ThreadDbHelp.Value == null)
            {
                _isComplete = false;
                _instanceId = Interlocked.Increment(ref IncreaseId);
                TransAttribute.ThreadDbHelp.Value = new BLLDbStore()
                {
                    StoreId = _instanceId
                };
            }
        }
        ~BLLTranScope()
        {
            Dispose(false);
        }
        public void Dispose()
        {
            Dispose(true);
        }
        private void Dispose(bool disposing)
        {
            if (_isComplete == false)
            {
                RollBack();
            }
            if (disposing)
            {
                GC.SuppressFinalize(this);
            }
        }
        public void RollBack()
        {
            if (TransAttribute.ThreadDbHelp.Value != null && TransAttribute.ThreadDbHelp.Value.StoreId == _instanceId)
            {
                DbHelp db = TransAttribute.ThreadDbHelp.Value.ThreadDb;
                if (db != null)
                {
                    db.RollBack();
                }
                TransAttribute.ThreadDbHelp.Value = null;
            }
        }

        public void Complete()
        {
            if (TransAttribute.ThreadDbHelp.Value != null && TransAttribute.ThreadDbHelp.Value.StoreId == _instanceId)
            {
                DbHelp db = TransAttribute.ThreadDbHelp.Value.ThreadDb;
                if (db != null)
                {
                    db.Commit();
                    _isComplete = true;
                }
                TransAttribute.ThreadDbHelp.Value = null;
            }
        }
        public async Task CompleteAsync()
        {
            if (TransAttribute.ThreadDbHelp.Value != null && TransAttribute.ThreadDbHelp.Value.StoreId == _instanceId)
            {
                DbHelp db = TransAttribute.ThreadDbHelp.Value.ThreadDb;
                if (db != null)
                {
                    await db.CommitAsync();
                    _isComplete = true;
                }
                TransAttribute.ThreadDbHelp.Value = null;
            }
        }
    }
}
