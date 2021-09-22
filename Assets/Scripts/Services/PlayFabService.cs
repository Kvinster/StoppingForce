using UnityEngine;

using System.Collections.Generic;

using SF.Services.Exceptions;

using PlayFab;
using PlayFab.ClientModels;

using RSG;

namespace SF.Services {
	public static class PlayFabService {
		public const string GuidKey = "_LoginGuid";

		const string LevelLeaderboardStatisticNameTemplate = "Level_{0}_Leaderboard";

		static bool _isLoginInProgress;

		static string Guid {
			get => PlayerPrefs.GetString(GuidKey);
			set => PlayerPrefs.SetString(GuidKey, value);
		}

		public static bool IsLoggedIn => PlayFabClientAPI.IsClientLoggedIn();

		public static string PlayFabId { get; private set; }

		public static string DisplayName { get; private set; }

		public static IPromise TryLogin() {
			if ( _isLoginInProgress || IsLoggedIn ) {
				return Promise.Resolved();
			}
			var promise = new Promise();
			_isLoginInProgress = true;
			if ( string.IsNullOrEmpty(Guid) ) {
				Guid = System.Guid.NewGuid().ToString();
			}
			PlayFabClientAPI.LoginWithCustomID(
				new LoginWithCustomIDRequest {
					CustomId              = Guid,
					CreateAccount         = true,
					InfoRequestParameters = new GetPlayerCombinedInfoRequestParams { GetPlayerProfile = true }
				}, result => {
					OnLogin(result);
					promise.Resolve();
				}, error => {
					OnLoginError(error);
					promise.Reject(new LoginFailException(error.ToString()));
				});
			return promise;
		}

		public static IPromise TryChangeDisplayName(string displayName) {
			if ( !IsLoggedIn ) {
				return Promise.Rejected(new NotLoggedInException());
			}
			var promise = new Promise();
			PlayFabClientAPI.UpdateUserTitleDisplayName(
				new UpdateUserTitleDisplayNameRequest { DisplayName = displayName },
				result => {
					DisplayName = result.DisplayName;
					promise.Resolve();
				}, error => { promise.Reject(new DisplayNameChangeFailException(error.ToString())); });
			return promise;
		}

		public static IPromise TrySendScore(int levelIndex, int score) {
			if ( !IsLoggedIn ) {
				return Promise.Rejected(new NotLoggedInException());
			}
			if ( Application.isEditor ) {
				return Promise.Resolved();
			}
			var promise = new Promise();
			PlayFabClientAPI.UpdatePlayerStatistics(
				new UpdatePlayerStatisticsRequest {
					Statistics = new List<StatisticUpdate> {
						new StatisticUpdate {
							StatisticName = string.Format(LevelLeaderboardStatisticNameTemplate, levelIndex),
							Value         = score
						}
					}
				}, _ => { promise.Resolve(); }, error => {
					Debug.LogErrorFormat("PlayFabService.TrySendScore: failed sending score\n{0}", error);
					promise.Reject(new SendScoreException(error.ToString()));
				});
			return promise;
		}

		public static IPromise<List<PlayerLeaderboardEntry>> GetLeaderboard(int levelIndex) {
			if ( !IsLoggedIn ) {
				return Promise<List<PlayerLeaderboardEntry>>.Rejected(new NotLoggedInException());
			}
			var promise = new Promise<List<PlayerLeaderboardEntry>>();
			PlayFabClientAPI.GetLeaderboard(new GetLeaderboardRequest {
				StatisticName   = string.Format(LevelLeaderboardStatisticNameTemplate, levelIndex),
				MaxResultsCount = 10,
			}, result => { promise.Resolve(result.Leaderboard); }, error => {
				Debug.LogErrorFormat("PlayFabService.GetLeaderboard: failed sending score\n{0}", error);
				promise.Resolve(null);
			});
			return promise;
		}

		static void OnLogin(LoginResult loginResult) {
			Debug.LogFormat("PlayFabService.OnLogin: login successful, id: '{0}'", loginResult.PlayFabId);
			_isLoginInProgress = false;
			PlayFabId          = loginResult.PlayFabId;
			if ( loginResult.InfoResultPayload.PlayerProfile != null ) {
				DisplayName = loginResult.InfoResultPayload.PlayerProfile.DisplayName;
			}
		}

		static void OnLoginError(PlayFabError error) {
			Debug.LogErrorFormat("PlayFabService.OnLoginError: code '{0}', message '{1}'", error.Error.ToString(),
				error.ErrorMessage);
			_isLoginInProgress = false;
		}
	}
}
