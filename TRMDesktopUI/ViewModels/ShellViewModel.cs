using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Caliburn.Micro;
using TRMDesktopUI.EventModels;
using TRMDesktopUI.Library.Api;
using TRMDesktopUI.Library.Models;

namespace TRMDesktopUI.ViewModels
{
  public class ShellViewModel : Conductor<object>, IHandle<LogOnEvent>
  {

    private IEventAggregator _events;
    private ILoggedInUserModel _user;
    private IAPIHelper _apiHelper;

    public ShellViewModel(IEventAggregator events,
      ILoggedInUserModel user, IAPIHelper apiHelper)
    {
      _events = events;
      
      _user = user;
      _apiHelper = apiHelper;


      _events.SubscribeOnPublishedThread(this);

      ActivateItemAsync(IoC.Get<LoginViewModel>());
    }

    public bool IsLoggedIn
    {
      get
      {
        bool output = false;

        if (string.IsNullOrWhiteSpace(_user.Token) == false)
        {
          output = true;
        }

        return output;
      }
    }

    public void ExitApplication()
    {
      TryCloseAsync();
    }

    public async void UserManagement()
    {
      await ActivateItemAsync(IoC.Get<UserDisplayViewModel>());
    }

    public async void LogOut()
    {
      _user.ResetUserModel();
      _apiHelper.LogoffUser();
      await ActivateItemAsync(IoC.Get<LoginViewModel>());
      NotifyOfPropertyChange(() => IsLoggedIn);
    }

    //public void Handle(LogOnEvent message)
    //{
    //  //throw new NotImplementedException();
    //  ActivateItem(_salesVM);
    //  NotifyOfPropertyChange(() => IsLoggedIn);
    //}

    public async Task HandleAsync(LogOnEvent message, CancellationToken cancellationToken)
    {
      await ActivateItemAsync(IoC.Get<SalesViewModel>(), cancellationToken);
      NotifyOfPropertyChange(() => IsLoggedIn);
    }
  }
}
