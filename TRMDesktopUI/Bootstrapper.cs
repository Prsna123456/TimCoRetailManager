using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using TRMDesktopUI.ViewModels;
using System.Runtime;

namespace TRMDesktopUI
{
  public class Bootstrapper : BootstrapperBase
  {
    private SimpleContainer _container;
    public Bootstrapper()
    {
      Initialize();
    }

    protected override void Configure()
    {
      _container = new SimpleContainer();

      //_container.Instance(_container);
      _container
        .PerRequest<ShellViewModel>();

      _container
        .Singleton<IWindowManager, WindowManager>()
        .Singleton<IEventAggregator, EventAggregator>();

      GetType().Assembly.GetTypes()
        .Where(type => type.IsClass)
        .Where(type => type.Name.EndsWith("ViewModel"))
        .ToList()
        .ForEach(viewModelType => _container.RegisterInstance(
          viewModelType, viewModelType.ToString(), viewModelType));
    }

    protected override void OnStartup(object sender, StartupEventArgs e)
    {
      //base.OnStartup(sender, e);
      DisplayRootViewFor<ShellViewModel>();
    }

    protected override IEnumerable<object> GetAllInstances(Type service)
    {
      return _container.GetAllInstances(service);
    }

    protected override object GetInstance(Type service, string key)
    {
      return _container.GetInstance(service, key);
    }

    protected override void BuildUp(object instance)
    {
      _container.BuildUp(instance);
    }

    //protected override IEnumerable<Assembly> SelectAssemblies()
    //{
    //  return new[] {
    //    Assembly.GetExecutingAssembly()
    //  };
    //}
  }
}
