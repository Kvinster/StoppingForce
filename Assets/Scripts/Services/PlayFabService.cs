using UnityEngine;

using System.Collections.Generic;

using SF.Services.Exceptions;

using PlayFab;
using PlayFab.ClientModels;
using RSG;

namespace SF.Services {
	public static class PlayFabService {
		const string LevelLeaderboardStatisticNameTemplate = "Level_{0}_Leaderboard";

		static bool _isLoginInProgress;

		public static bool IsLoggedIn { get; private set; }

		public static string PlayFabId { get; private set; }

		public static void TryLogin() {
			if ( _isLoginInProgress || IsLoggedIn ) {
				return;
			}
			PlayFabClientAPI.LoginWithCustomID(
				new LoginWithCustomIDRequest {
					CustomId      = SystemInfo.deviceUniqueIdentifier,
					CreateAccount = true
				}, OnLogin, OnLoginError);
			_isLoginInProgress = true;
		}

		public static IPromise TrySendScore(int levelIndex, int score) {
			if ( !IsLoggedIn ) {
				return Promise.Rejected(new NotLoggedInException());
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
			IsLoggedIn         = true;
			PlayFabId          = loginResult.PlayFabId;
		}

		static void OnLoginError(PlayFabError error) {
			Debug.LogErrorFormat("PlayFabService.OnLoginError: code '{0}', message '{1}'", error.Error.ToString(),
				error.ErrorMessage);
			_isLoginInProgress = false;
			IsLoggedIn         = false;
		}
	}
}
