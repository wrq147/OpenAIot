using AirJointUI.Api;
using AirJointUI.Utils;
using ReactiveUI;
using System;
using System.Threading;
using System.Threading.Tasks;
namespace AirJointUI.ViewModels
{
    /// <summary>
    /// 加载模型
    /// </summary>
    public class LoadViewModel : ViewModelBase
    {
        private bool _finishInit = false;
        private string _tipTxt = string.Empty;
        public string TipTxt
        {
            get => _tipTxt;
            set => this.RaiseAndSetIfChanged(ref _tipTxt, value);
        }
        public LoadViewModel()
        {
        }
        public async Task UpdateProgress()
        {
            try
            {
                //加载进度
                string[] tipArr = new[] { "", ".", "..", "..." };
                int i = 0;
                while (!_finishInit)
                {
                    TipTxt = string.Format(I18NExt.Translate("LoadingTxt"), tipArr[i % tipArr.Length]);
                    _finishInit = await CheckApi.CheckApiReady();
                    ++i;
                    await Task.Delay(100);
                }
                MyVMLocator.Instance.ResetSetView();

                //跳转到主界面
                MyVMLocator.Instance.Window.ContentViewModel = MyVMLocator.Instance.IndexViewModel;
                MyVMLocator.Instance.IndexViewModel.StartInit();
                //刷新缓存
                await DeviceApi.RefreshAllProductCache();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Thread.Sleep(200);
                MyVMLocator.Instance.Window.ContentViewModel = MyVMLocator.Instance.IndexViewModel;
                MyVMLocator.Instance.IndexViewModel.StartInit();
                await DeviceApi.RefreshAllProductCache();
            }

        }
    }
}
