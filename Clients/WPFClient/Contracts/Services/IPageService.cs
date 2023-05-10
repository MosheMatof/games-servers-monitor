using System.Windows.Controls;

namespace WPFClient.Contracts.Services;

public interface IPageService
{
    Type GetPageType(string key);

    Page GetPage(string key);
}
