using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using TRMDesktopUI.Library.Models;

namespace TRMDesktopUI.Library.Api
{
  public class APIHelper : IAPIHelper
  {
    private HttpClient apiClient { get; set; }
    private ILoggedInUserModel _loggedInuser;

    public APIHelper(ILoggedInUserModel loggedInUser)
    {
      IntializeClient();
      _loggedInuser = loggedInUser;
    }

    private void IntializeClient()
    {
      string api = ConfigurationManager.AppSettings["api"];

      apiClient = new HttpClient();
      apiClient.BaseAddress = new Uri(api);
      apiClient.DefaultRequestHeaders.Accept.Clear();
      apiClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
    }

    public async Task<AuthenticatedUser> Authenticate(string username, string password)
    {
      var data = new FormUrlEncodedContent(new[]
      {
        new KeyValuePair<string, string>("grant_type", "password"),
        new KeyValuePair<string, string>("username", username),
        new KeyValuePair<string, string>("password", password),
      });

      using (HttpResponseMessage response = await apiClient.PostAsync("/Token", data))
      {
        if (response.IsSuccessStatusCode)
        {
          var result = await response.Content.ReadAsAsync<AuthenticatedUser>();
          return result;
        }
        else
        {
          throw new Exception(response.ReasonPhrase);
        }
      }
    }

    public async Task GetLoggedInUserInfo(string token)
    {
      apiClient.DefaultRequestHeaders.Clear();
      apiClient.DefaultRequestHeaders.Accept.Clear();
      apiClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
      apiClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");

      using (HttpResponseMessage response = await apiClient.GetAsync("/api/User"))
      {
        if (response.IsSuccessStatusCode)
        {
          var result = await response.Content.ReadAsAsync<LoggedInUserModel>();
          _loggedInuser.CreatedDate = result.CreatedDate;
          _loggedInuser.EmailAddress = result.EmailAddress;
          _loggedInuser.FirstName = result.FirstName;
          _loggedInuser.LastName = result.LastName;
          _loggedInuser.Id = result.Id;
          _loggedInuser.Token = token;

          //return result;
        }
        else
        {
          throw new Exception(response.ReasonPhrase);
        }
      }

    }

  }
}
