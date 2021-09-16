using UnityEngine;

using System.Collections.Generic;

using PlayFab;
using PlayFab.ClientModels;

namespace SF.Services {
	public static class PlayFabService {
		const string LevelLeaderboardStatisticNameTemplate = "Level_{0}_Leaderboard";

		static bool _isLoginInProgress;

		public static bool IsLoggedIn { get; private set; }

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

		public static void TrySendScore(int levelIndex, int score) {
			if ( !IsLoggedIn ) {
				return;
			}
			PlayFabClientAPI.UpdatePlayerStatistics(new UpdatePlayerStatisticsRequest {
				Statistics = new List<StatisticUpdate> {
					new StatisticUpdate {
						StatisticName = string.Format(LevelLeaderboardStatisticNameTemplate, levelIndex),
						Value         = score
					}
				}
			}, OnStatisticsUpdateSuccess, OnStatisticsUpdateError);
		}

		static void OnLogin(LoginResult loginResult) {
			Debug.LogFormat("PlayFabService.OnLogin: login successful, id: '{0}'", loginResult.PlayFabId);
			_isLoginInProgress = false;
			IsLoggedIn        = true;
		}

		static void OnLoginError(PlayFabError error) {
			Debug.LogErrorFormat("PlayFabService.OnLoginError: code '{0}', message '{1}'", error.Error.ToString(),
				error.ErrorMessage);
			_isLoginInProgress = false;
			IsLoggedIn        = false;
		}

		static void OnStatisticsUpdateSuccess(UpdatePlayerStatisticsResult result) {
			Debug.LogFormat("PlayFabService.OnStatisticsUpdateSuccess");
		}

		static void OnStatisticsUpdateError(PlayFabError error) {
			Debug.LogErrorFormat("PlayFabService.OnStatisticsUpdateError: code '{0}', message '{1}'",
				error.Error.ToString(), error.ErrorMessage);
		}
	}
}
